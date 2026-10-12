using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public RefreshTokenRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public int Create(RefreshTokenRecord token)
        {
            const string sql = @"
                INSERT INTO dbo.RefreshToken
                    (Manguoidung, TokenHash, Ngaytao, Ngayhethan)
                VALUES
                    (@Manguoidung, @TokenHash, @Ngaytao, @Ngayhethan);";

            return _databaseHelper.Execute(sql, new
            {
                token.Manguoidung,
                token.TokenHash,
                token.Ngaytao,
                token.Ngayhethan
            });
        }

        public RefreshTokenRecord ? ConsumeValidToken(string tokenHash)
        {
            const string sql = @"
                UPDATE dbo.RefreshToken
                SET Ngaythuhoi = SYSUTCDATETIME()
                OUTPUT
                    inserted.MaRefreshToken,
                    inserted.Manguoidung,
                    inserted.TokenHash,
                    inserted.Ngaytao,
                    inserted.Ngayhethan,
                    inserted.Ngaythuhoi
                WHERE TokenHash = @TokenHash
                  AND Ngaythuhoi IS NULL
                  AND Ngayhethan > SYSUTCDATETIME();";

            return _databaseHelper.QueryFirstOrDefault<RefreshTokenRecord>(
                sql,
                new { TokenHash = tokenHash });
        }

        public int Revoke(string tokenHash)
        {
            const string sql = @"
                UPDATE dbo.RefreshToken
                SET Ngaythuhoi = SYSUTCDATETIME()
                WHERE TokenHash = @TokenHash
                  AND Ngaythuhoi IS NULL;";

            return _databaseHelper.Execute(sql, new { TokenHash = tokenHash });
        }

        public int RevokeAllByUser(string maNguoiDung)
        {
            const string sql = @"
                UPDATE dbo.RefreshToken
                SET Ngaythuhoi = SYSUTCDATETIME()
                WHERE Manguoidung = @Manguoidung
                  AND Ngaythuhoi IS NULL;";

            return _databaseHelper.Execute(
                sql,
                new { Manguoidung = maNguoiDung });
        }
    }
}
