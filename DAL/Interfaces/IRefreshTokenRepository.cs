using Model.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        int Create(RefreshTokenRecord token);

        RefreshTokenRecord? ConsumeValidToken(string tokenHash);

        int Revoke(string tokenHash);

        int RevokeAllByUser(string maNguoiDung);
    }
}
