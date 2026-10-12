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
    public class PhanCongRepository : IPhanCongRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public PhanCongRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<PhanCong> GetAll()
        {
            string sql = @"
                SELECT *
                FROM PhanCong
                ORDER BY Ngayphancong DESC";

            return _databaseHelper.Query<PhanCong>(sql);
        }

        public PhanCong GetById(string maPhanCong)
        {
            string sql = @"
                SELECT *
                FROM PhanCong
                WHERE Ma_phancong = @Ma_phancong";

            return _databaseHelper.QueryFirstOrDefault<PhanCong>(
                sql,
                new { Ma_phancong = maPhanCong });
        }

        public IEnumerable<PhanCong> GetByCongViec(string maCongViec)
        {
            string sql = @"
                SELECT *
                FROM PhanCong
                WHERE Macongviec = @Macongviec
                ORDER BY Ngayphancong DESC";

            return _databaseHelper.Query<PhanCong>(
                sql,
                new { Macongviec = maCongViec });
        }

        public IEnumerable<PhanCong> GetByDuAn(string maDuAn)
        {
            string sql = @"
                SELECT PC.*
                FROM PhanCong PC
                INNER JOIN CongViec CV
                    ON PC.Macongviec = CV.Macongviec
                WHERE CV.Maduan = @Maduan
                ORDER BY PC.Ngayphancong DESC";

            return _databaseHelper.Query<PhanCong>(
                sql,
                new { Maduan = maDuAn });
        }

        public bool IsProjectManagerForTask(string maCongViec, string maNguoiDung)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM CongViec CV
                INNER JOIN DuAn DA ON CV.Maduan = DA.Maduan
                WHERE CV.Macongviec = @Macongviec
                  AND DA.Nguoitao = @Manguoidung";

            return _databaseHelper.ExecuteScalar<int>(
                sql,
                new { Macongviec = maCongViec, Manguoidung = maNguoiDung }) > 0;
        }

        public IEnumerable<PhanCong> GetByNguoiDung(string maNguoiDung)
        {
            string sql = @"
                SELECT *
                FROM PhanCong
                WHERE Manguoidung = @Manguoidung
                ORDER BY Ngayphancong DESC";

            return _databaseHelper.Query<PhanCong>(
                sql,
                new { Manguoidung = maNguoiDung });
        }

        public IEnumerable<PhanCong> Search(string keyword)
        {
            string sql = @"
                SELECT PC.*
                FROM PhanCong PC
                INNER JOIN CongViec CV
                    ON PC.Macongviec = CV.Macongviec
                WHERE PC.Ma_phancong LIKE @Keyword
                   OR PC.Macongviec LIKE @Keyword
                   OR PC.Manguoidung LIKE @Keyword
                   OR CV.Maduan LIKE @Keyword
                ORDER BY PC.Ngayphancong DESC";

            return _databaseHelper.Query<PhanCong>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(PhanCong phanCong)
        {
            string sql = @"
                INSERT INTO PhanCong
                (
                    Ma_phancong,
                    Macongviec,
                    Manguoidung,
                    Ngayphancong
                )
                VALUES
                (
                    @Ma_phancong,
                    @Macongviec,
                    @Manguoidung,
                    GETDATE()
                )";

            return _databaseHelper.Execute(sql, phanCong);
        }

        public int Update(PhanCong phanCong)
        {
            string sql = @"
                UPDATE PhanCong
                SET
                    Macongviec = @Macongviec,
                    Manguoidung = @Manguoidung
                WHERE Ma_phancong = @Ma_phancong";

            return _databaseHelper.Execute(sql, phanCong);
        }

        public int Delete(string maPhanCong)
        {
            string sql = @"
                DELETE FROM PhanCong
                WHERE Ma_phancong = @Ma_phancong";

            return _databaseHelper.Execute(
                sql,
                new { Ma_phancong = maPhanCong });
        }

        // ============================
        // PHÂN TRANG
        // ============================
        public IEnumerable<PhanCong> GetPaged(
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            int offset = (pageNumber - 1) * pageSize;

            string sql = @"
        SELECT *
        FROM PhanCong
        ORDER BY Ngayphancong DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

            return _databaseHelper.Query<PhanCong>(
                sql,
                new
                {
                    Offset = offset,
                    PageSize = pageSize
                });
        }

        // ============================
        // TÌM KIẾM + PHÂN TRANG
        // ============================
        public IEnumerable<PhanCong> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            int offset = (pageNumber - 1) * pageSize;

            string sql = @"
        SELECT *
        FROM PhanCong
        WHERE Ma_phancong LIKE @Keyword
           OR Macongviec LIKE @Keyword
           OR Manguoidung LIKE @Keyword
        ORDER BY Ngayphancong DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

            return _databaseHelper.Query<PhanCong>(
                sql,
                new
                {
                    Keyword = "%" + (keyword ?? "") + "%",
                    Offset = offset,
                    PageSize = pageSize
                });
        }

        // ============================
        // ĐẾM TỔNG
        // ============================
        public int Count(string keyword)
        {
            string sql = @"
        SELECT COUNT(*)
        FROM PhanCong
        WHERE Ma_phancong LIKE @Keyword
           OR Macongviec LIKE @Keyword
           OR Manguoidung LIKE @Keyword";

            return _databaseHelper.ExecuteScalar<int>(
                sql,
                new
                {
                    Keyword = "%" + (keyword ?? "") + "%"
                });
        }
    }
}
