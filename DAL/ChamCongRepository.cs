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
    public class ChamCongRepository : IChamCongRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public ChamCongRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<ChamCong> GetAll()
        {
            string sql = @"
                SELECT *
                FROM ChamCong
                WHERE IsDeleted = 0
                ORDER BY Ngaylamviec DESC";

            return _databaseHelper.Query<ChamCong>(sql);
        }

        public ChamCong GetById(string maBangChamCong)
        {
            string sql = @"
                SELECT *
                FROM ChamCong
                WHERE Mabangchamcong = @Mabangchamcong AND IsDeleted = 0";

            return _databaseHelper.QueryFirstOrDefault<ChamCong>(
                sql,
                new { Mabangchamcong = maBangChamCong });
        }

        public IEnumerable<ChamCong> GetByNguoiDung(string maNguoiDung)
        {
            string sql = @"
                SELECT *
                FROM ChamCong
                WHERE Manguoidung = @Manguoidung AND IsDeleted = 0
                ORDER BY Ngaylamviec DESC";

            return _databaseHelper.Query<ChamCong>(
                sql,
                new { Manguoidung = maNguoiDung });
        }

        public IEnumerable<ChamCong> GetPending()
        {
            string sql = @"
                SELECT *
                FROM ChamCong
                WHERE Trangthai = 'Pending' AND IsDeleted = 0
                ORDER BY Ngaylamviec DESC";

            return _databaseHelper.Query<ChamCong>(sql);
        }

        public IEnumerable<ChamCong> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM ChamCong
                WHERE IsDeleted = 0
                  AND (Mabangchamcong LIKE @Keyword
                   OR Manguoidung LIKE @Keyword
                   OR Macongviec LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                   OR Nguoiduyet LIKE @Keyword)
                ORDER BY Ngaylamviec DESC";

            return _databaseHelper.Query<ChamCong>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(ChamCong chamCong)
        {
            string sql = @"
                INSERT INTO ChamCong
                (
                    Mabangchamcong,
                    Manguoidung,
                    Macongviec,
                    Ngaylamviec,
                    Sogiolam,
                    Trangthai,
                    Nguoiduyet,
                    LydoTuChoi,
                    IsDeleted,
                    DeletedAt,
                    Ngaytao
                )
                VALUES
                (
                    @Mabangchamcong,
                    @Manguoidung,
                    @Macongviec,
                    @Ngaylamviec,
                    @Sogiolam,
                    @Trangthai,
                    @Nguoiduyet,
                    @LydoTuChoi,
                    0,
                    NULL,
                    GETDATE()
                )";

            return _databaseHelper.Execute(sql, chamCong);
        }

        public int Update(ChamCong chamCong)
        {
            string sql = @"
                UPDATE ChamCong
                SET
                    Manguoidung = @Manguoidung,
                    Macongviec = @Macongviec,
                    Ngaylamviec = @Ngaylamviec,
                    Sogiolam = @Sogiolam,
                    Trangthai = @Trangthai,
                    Nguoiduyet = @Nguoiduyet,
                    LydoTuChoi = @LydoTuChoi
                WHERE Mabangchamcong = @Mabangchamcong AND IsDeleted = 0";

            return _databaseHelper.Execute(sql, chamCong);
        }

        public int Delete(string maBangChamCong)
        {
            string sql = @"
                UPDATE ChamCong
                SET IsDeleted = 1, DeletedAt = GETDATE()
                WHERE Mabangchamcong = @Mabangchamcong AND IsDeleted = 0";

            return _databaseHelper.Execute(
                sql,
                new { Mabangchamcong = maBangChamCong });
        }

        public int Approve(string maBangChamCong, string nguoiDuyet)
        {
            string sql = @"
                UPDATE ChamCong
                SET
                    Trangthai = 'Approved',
                    Nguoiduyet = @Nguoiduyet
                WHERE Mabangchamcong = @Mabangchamcong
                  AND IsDeleted = 0
                  AND Trangthai = 'Pending'";

            return _databaseHelper.Execute(
                sql,
                new
                {
                    Mabangchamcong = maBangChamCong,
                    Nguoiduyet = nguoiDuyet
                });
        }

        public int Reject(string maBangChamCong, string nguoiDuyet, string lyDoTuChoi)
        {
            string sql = @"
                UPDATE ChamCong
                SET
                    Trangthai = 'Rejected',
                    Nguoiduyet = @Nguoiduyet,
                    LydoTuChoi = @LydoTuChoi
                WHERE Mabangchamcong = @Mabangchamcong
                  AND IsDeleted = 0
                  AND Trangthai = 'Pending'";

            return _databaseHelper.Execute(
                sql,
                new
                {
                    Mabangchamcong = maBangChamCong,
                    Nguoiduyet = nguoiDuyet,
                    LydoTuChoi = lyDoTuChoi
                });
        }

        public decimal GetTotalHoursForDay(string maNguoiDung, DateTime ngayLamViec, string excludeMaBangChamCong = null)
        {
            const string sql = @"
                SELECT COALESCE(SUM(Sogiolam), 0)
                FROM ChamCong
                WHERE Manguoidung = @Manguoidung
                  AND CAST(Ngaylamviec AS date) = CAST(@Ngaylamviec AS date)
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR Mabangchamcong <> @ExcludeId)";

            return _databaseHelper.ExecuteScalar<decimal>(sql, new { Manguoidung = maNguoiDung, Ngaylamviec = ngayLamViec, ExcludeId = excludeMaBangChamCong });
        }

        public bool IsTaskAssignedToUser(string maCongViec, string maNguoiDung)
        {
            const string sql = @"
                SELECT COUNT(*) FROM PhanCong
                WHERE Macongviec = @Macongviec AND Manguoidung = @Manguoidung";
            return _databaseHelper.ExecuteScalar<int>(sql, new { Macongviec = maCongViec, Manguoidung = maNguoiDung }) > 0;
        }

        public bool CanReviewerReview(string maBangChamCong, string maNguoiDuyet)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM ChamCong CC
                INNER JOIN CongViec CV ON CC.Macongviec = CV.Macongviec
                INNER JOIN DuAn DA ON CV.Maduan = DA.Maduan
                WHERE CC.Mabangchamcong = @Mabangchamcong
                  AND CC.IsDeleted = 0
                  AND DA.Nguoitao = @Nguoiduyet";
            return _databaseHelper.ExecuteScalar<int>(sql, new { Mabangchamcong = maBangChamCong, Nguoiduyet = maNguoiDuyet }) > 0;
        }
    }
}
