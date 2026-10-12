
using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Auth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace BLL
{
    public class AuthBusiness : IAuthBusiness
    {

        private readonly INguoiDungBusiness _nguoiDungBusiness;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        private static readonly ConcurrentDictionary<string, OtpInfo>
            _otpStore = new ConcurrentDictionary<string, OtpInfo>();

        public AuthBusiness(
    INguoiDungBusiness nguoiDungBusiness,
    IConfiguration configuration,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository)
        {
            _nguoiDungBusiness = nguoiDungBusiness;
            _configuration = configuration;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public LoginResponse Login(LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Tendangnhap) ||
                string.IsNullOrEmpty(request.Matkhau))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Dữ liệu đăng nhập không hợp lệ."
                };
            }

            var nguoiDung = _nguoiDungBusiness
                .GetByUsername(request.Tendangnhap);

            if (nguoiDung == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng."
                };
            }

            if (!nguoiDung.Trangthai)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tài khoản đã bị khóa."
                };
            }

            bool passwordCorrect = VerifyPassword(
                request.Matkhau,
                nguoiDung.matkhau);

            if (!passwordCorrect)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng."
                };
            }

            // Tự động mã hóa mật khẩu cũ dạng chữ thường.
            if (!IsBcryptHash(nguoiDung.matkhau))
            {
                nguoiDung.matkhau =
                    BCrypt.Net.BCrypt.HashPassword(request.Matkhau);

                int updateResult = _nguoiDungBusiness.Update(nguoiDung);

                if (updateResult <= 0)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Không thể cập nhật mật khẩu an toàn."
                    };
                }
            }

            // Sinh OTP 6 chữ số bằng bộ sinh số ngẫu nhiên an toàn.
            string otp = RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();

            _otpStore[nguoiDung.Tendangnhap] = new OtpInfo
            {
                OTP = otp,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5)
            };

            Debug.WriteLine(
                $"[2FA] OTP của {nguoiDung.Tendangnhap}: {otp}");

            Console.WriteLine(
                $"[2FA] OTP của {nguoiDung.Tendangnhap}: {otp}");

            return new LoginResponse
            {
                Success = true,
                Require2FA = true,
                Message = "Đăng nhập thành công. Vui lòng nhập mã OTP 2FA.",
                MaNguoiDung = nguoiDung.Manguoidung,
                Tendangnhap = nguoiDung.Tendangnhap
            };
        }

        public TokenResponse RefreshAccessToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Refresh Token không được để trống."
                };
            }

            string tokenHash = HashRefreshToken(refreshToken);

            // Kiểm tra và thu hồi token cũ trong một câu SQL nguyên tử.
            var oldToken =
                _refreshTokenRepository.ConsumeValidToken(tokenHash);

            if (oldToken == null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Refresh Token không hợp lệ, đã hết hạn hoặc đã được sử dụng."
                };
            }

            var nguoiDung = _nguoiDungBusiness.GetById(
                oldToken.Manguoidung);

            if (nguoiDung == null || !nguoiDung.Trangthai)
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại hoặc đã bị khóa."
                };
            }

            string newAccessToken = _tokenService.GenerateAccessToken(
                nguoiDung.Manguoidung,
                nguoiDung.Email ?? "",
                new[] { nguoiDung.Vaitro });

            string newRefreshToken = _tokenService.GenerateRefreshToken();

            var newRecord = new RefreshTokenRecord
            {
                Manguoidung = nguoiDung.Manguoidung,
                TokenHash = HashRefreshToken(newRefreshToken),
                Ngaytao = DateTime.UtcNow,
                Ngayhethan = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<int?>(
                        "JwtSettings:RefreshTokenExpirationDays") ?? 30)
            };

            int inserted = _refreshTokenRepository.Create(newRecord);

            if (inserted <= 0)
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Không thể phát hành Refresh Token mới."
                };
            }

            return new TokenResponse
            {
                Success = true,
                Message = "Làm mới token thành công.",
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        public int Logout(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return 0;

            string tokenHash = HashRefreshToken(refreshToken);

            return _refreshTokenRepository.Revoke(tokenHash);
        }

        private static string HashRefreshToken(string refreshToken)
        {
            byte[] bytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToHexString(bytes);
        }

        public Verify2FAResponse Verify2FA(Verify2FARequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Tendangnhap) ||
                string.IsNullOrWhiteSpace(request.OTP))
            {
                return new Verify2FAResponse
                {
                    Success = false,
                    Message = "Dữ liệu xác thực không hợp lệ."
                };
            }

            if (!_otpStore.TryGetValue(
                request.Tendangnhap,
                out OtpInfo otpInfo))
            {
                return new Verify2FAResponse
                {
                    Success = false,
                    Message = "Không tìm thấy mã OTP."
                };
            }

            if (DateTime.UtcNow >= otpInfo.ExpiredAt)
            {
                _otpStore.TryRemove(request.Tendangnhap, out _);

                return new Verify2FAResponse
                {
                    Success = false,
                    Message = "Mã OTP đã hết hạn."
                };
            }

            if (!string.Equals(
                otpInfo.OTP,
                request.OTP,
                StringComparison.Ordinal))
            {
                return new Verify2FAResponse
                {
                    Success = false,
                    Message = "Mã OTP không chính xác."
                };
            }

            var nguoiDung = _nguoiDungBusiness
                .GetByUsername(request.Tendangnhap);

            if (nguoiDung == null || !nguoiDung.Trangthai)
            {
                _otpStore.TryRemove(request.Tendangnhap, out _);

                return new Verify2FAResponse
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại hoặc đã bị khóa."
                };
            }

            _otpStore.TryRemove(request.Tendangnhap, out _);

            string accessToken = _tokenService.GenerateAccessToken(
                nguoiDung.Manguoidung,
                nguoiDung.Email ?? "",
                new[] { nguoiDung.Vaitro });

            string refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenRecord = new RefreshTokenRecord
            {
                Manguoidung = nguoiDung.Manguoidung,
                TokenHash = HashRefreshToken(refreshToken),
                Ngaytao = DateTime.UtcNow,
                Ngayhethan = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<int?>(
                        "JwtSettings:RefreshTokenExpirationDays") ?? 30)
            };

            int inserted = _refreshTokenRepository.Create(refreshTokenRecord);

            if (inserted <= 0)
            {
                return new Verify2FAResponse
                {
                    Success = false,
                    Message = "Không thể lưu Refresh Token."
                };
            }

            return new Verify2FAResponse
            {
                Success = true,
                Message = "Xác thực 2FA thành công.",
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }

        public int DoiMatKhau(
            string maNguoiDung,
            DoiMatKhauRequest request)
        {
            if (string.IsNullOrWhiteSpace(maNguoiDung) ||
                request == null ||
                string.IsNullOrEmpty(request.MatkhauCu) ||
                string.IsNullOrEmpty(request.MatkhauMoi))
            {
                return 0;
            }

            var nguoiDung = _nguoiDungBusiness
                .GetById(maNguoiDung);

            if (nguoiDung == null || !nguoiDung.Trangthai)
                return 0;

            if (!VerifyPassword(
                request.MatkhauCu,
                nguoiDung.matkhau))
            {
                return 0;
            }

            if (request.MatkhauCu == request.MatkhauMoi)
                return 0;

            nguoiDung.matkhau =
                BCrypt.Net.BCrypt.HashPassword(request.MatkhauMoi);

            return _nguoiDungBusiness.Update(nguoiDung);
        }

        private static bool VerifyPassword(
            string password,
            string storedPassword)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }

            if (IsBcryptHash(storedPassword))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(
                        password,
                        storedPassword);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    return false;
                }
            }

            // Tương thích tài khoản cũ đang lưu mật khẩu chữ thường.
            return string.Equals(
                password,
                storedPassword,
                StringComparison.Ordinal);
        }

        private static bool IsBcryptHash(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                (password.StartsWith("$2a$") ||
                 password.StartsWith("$2b$") ||
                 password.StartsWith("$2y$"));
        }

        private class OtpInfo
        {
            public string OTP { get; set; }
            public DateTime ExpiredAt { get; set; }
        }
    }
}