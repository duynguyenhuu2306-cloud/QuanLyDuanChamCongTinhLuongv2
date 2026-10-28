
using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Auth;
using System;
using System.Collections.Generic;
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
                    _configuration.GetValue<int?>("JwtSettings:RefreshTokenExpirationDays") ?? 30)
            };

            if (_refreshTokenRepository.Create(refreshTokenRecord) <= 0)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Không thể phát hành Refresh Token."
                };
            }

            return new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công.",
                MaNguoiDung = nguoiDung.Manguoidung,
                Tendangnhap = nguoiDung.Tendangnhap,
                VaiTro = nguoiDung.Vaitro,
                Token = accessToken,
                RefreshToken = refreshToken
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

    }
}
