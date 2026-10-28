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
    public class CongViecRepository : ICongViecRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public CongViecRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<CongViec> GetAll()
        {
            string sql = @"
                SELECT *
                FROM CongViec
                ORDER BY Dateline ASC";

            return _databaseHelper.Query<CongViec>(sql);
        }

        public CongViec GetById(string maCongViec)
        {
            string sql = @"
                SELECT *
                FROM CongViec
                WHERE Macongviec = @Macongviec";

            return _databaseHelper.QueryFirstOrDefault<CongViec>(
                sql,
                new { Macongviec = maCongViec });
        }

        public IEnumerable<CongViec> GetByDuAn(string maDuAn)
        {
            string sql = @"
                SELECT *
                FROM CongViec
                WHERE Maduan = @Maduan
                ORDER BY Dateline ASC";

            return _databaseHelper.Query<CongViec>(
                sql,
                new { Maduan = maDuAn });
        }

        public IEnumerable<CongViec> GetAssignedToUser(string maNguoiDung) => _databaseHelper.Query<CongViec>(@"
            SELECT CV.*
            FROM CongViec CV
            INNER JOIN PhanCong PC ON PC.Macongviec = CV.Macongviec
            WHERE PC.Manguoidung = @Manguoidung
            ORDER BY CV.Dateline ASC", new { Manguoidung = maNguoiDung });

        public int Create(CongViec congViec)
        {
            string sql = @"
                INSERT INTO CongViec
                (
                    Macongviec,
                    Maduan,
                    Tieude,
                    Mota,
                    Trangthai,
                    Mucdouutien,
                    Ngaybatdau,
                    Dateline,
                    Nguoitao
                )
                VALUES
                (
                    @Macongviec,
                    @Maduan,
                    @Tieude,
                    @Mota,
                    @Trangthai,
                    @Mucdouutien,
                    @Ngaybatdau,
                    @Dateline,
                    @Nguoitao
                )";

            return _databaseHelper.Execute(sql, congViec);
        }

        public int Update(CongViec congViec)
        {
            string sql = @"
                UPDATE CongViec
                SET
                    Maduan = @Maduan,
                    Tieude = @Tieude,
                    Mota = @Mota,
                    Trangthai = @Trangthai,
                    Mucdouutien = @Mucdouutien,
                    Ngaybatdau = @Ngaybatdau,
                    Dateline = @Dateline,
                    Nguoitao = @Nguoitao
                WHERE Macongviec = @Macongviec";

            return _databaseHelper.Execute(sql, congViec);
        }

        public int Delete(string maCongViec)
        {
            string sql = @"
                DELETE FROM CongViec
                WHERE Macongviec = @Macongviec";

            return _databaseHelper.Execute(
                sql,
                new { Macongviec = maCongViec });
        }

        public IEnumerable<CongViec> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM CongViec
                WHERE Macongviec LIKE @Keyword
                   OR Maduan LIKE @Keyword
                   OR Tieude LIKE @Keyword
                   OR Mota LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                   OR Mucdouutien LIKE @Keyword
                ORDER BY Dateline ASC";

            return _databaseHelper.Query<CongViec>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }


        // ============================
        // PHÂN TRANG
        // ============================
        public IEnumerable<CongViec> GetPaged(
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
                FROM CongViec
                ORDER BY Dateline ASC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            return _databaseHelper.Query<CongViec>(
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
        public IEnumerable<CongViec> SearchPaged(
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
                FROM CongViec
                WHERE Macongviec LIKE @Keyword
                   OR Maduan LIKE @Keyword
                   OR Tieude LIKE @Keyword
                   OR Mota LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                   OR Mucdouutien LIKE @Keyword
                ORDER BY Dateline ASC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            return _databaseHelper.Query<CongViec>(
                sql,
                new
                {
                    Keyword = "%" + (keyword ?? "") + "%",
                    Offset = offset,
                    PageSize = pageSize
                });
        }

        // ============================
        // ĐẾM TỔNG BẢN GHI
        // ============================
        public int Count(string keyword)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM CongViec
                WHERE Macongviec LIKE @Keyword
                   OR Maduan LIKE @Keyword
                   OR Tieude LIKE @Keyword
                   OR Mota LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                   OR Mucdouutien LIKE @Keyword";

            return _databaseHelper.ExecuteScalar<int>(
                sql,
                new
                {
                    Keyword = "%" + (keyword ?? "") + "%"
                });
        }
    }
}
