using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public AuditLogRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<Audit_Log> GetAll()
        {
            string sql = @"
                SELECT *
                FROM Audit_Log
                ORDER BY Thoigian DESC";

            return _databaseHelper.Query<Audit_Log>(sql);
        }

        public Audit_Log GetById(string maDangKy)
        {
            string sql = @"
                SELECT *
                FROM Audit_Log
                WHERE Madangky = @Madangky";

            return _databaseHelper.QueryFirstOrDefault<Audit_Log>(
                sql,
                new { Madangky = maDangKy });
        }

        public IEnumerable<Audit_Log> GetByNguoiDung(string maNguoiDung)
        {
            string sql = @"
                SELECT *
                FROM Audit_Log
                WHERE Manguoidung = @Manguoidung
                ORDER BY Thoigian DESC";

            return _databaseHelper.Query<Audit_Log>(
                sql,
                new { Manguoidung = maNguoiDung });
        }

        public IEnumerable<Audit_Log> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM Audit_Log
                WHERE Madangky LIKE @Keyword
                   OR Manguoidung LIKE @Keyword
                   OR Hanhdong LIKE @Keyword
                   OR Tentable LIKE @Keyword
                   OR Madulieu LIKE @Keyword
                ORDER BY Thoigian DESC";

            return _databaseHelper.Query<Audit_Log>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(Audit_Log auditLog)
        {
            string sql = @"
                INSERT INTO Audit_Log
                (
                    Madangky,
                    Manguoidung,
                    Hanhdong,
                    Tentable,
                    Madulieu,
                    Thoigian
                )
                VALUES
                (
                    @Madangky,
                    @Manguoidung,
                    @Hanhdong,
                    @Tentable,
                    @Madulieu,
                    GETDATE()
                )";

            return _databaseHelper.Execute(sql, auditLog);
        }
    }
}
