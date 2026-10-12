using DAL.Interfaces;
using Model.Auth;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Interfaces
{
    public interface IAuthBusiness
    {
        LoginResponse Login(LoginRequest request);

        Verify2FAResponse Verify2FA(Verify2FARequest request);

        int DoiMatKhau(string maNguoiDung, DoiMatKhauRequest request);

        TokenResponse RefreshAccessToken(string refreshToken);

        int Logout(string refreshToken);
    }
}
