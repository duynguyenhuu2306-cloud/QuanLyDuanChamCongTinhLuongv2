using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DuAnRepository : IDuAnRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public DuAnRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<DuAn> GetAll()
        {
            string sql = @"
                SELECT *
                FROM DuAn
                ORDER BY Ngaybatdau DESC";

            return _databaseHelper.Query<DuAn>(sql);
        }

        public DuAn GetById(string maDuAn)
        {
            string sql = @"
                SELECT *
                FROM DuAn
                WHERE Maduan = @Maduan";

            return _databaseHelper.QueryFirstOrDefault<DuAn>(
                sql,
                new { Maduan = maDuAn });
        }

        public int Create(DuAn duAn)
        {
            string sql = @"
                INSERT INTO DuAn
                (
                    Maduan,
                    Tenduan,
                    Motada,
                    Ngaybatdau,
                    Ngayketthuc,
                    Trangthai,
                    Nguoitao
                )
                VALUES
                (
                    @Maduan,
                    @Tenduan,
                    @Motada,
                    @Ngaybatdau,
                    @Ngayketthuc,
                    @Trangthai,
                    @Nguoitao
                )";

            return _databaseHelper.Execute(sql, duAn);
        }

        public int Update(DuAn duAn)
        {
            string sql = @"
                UPDATE DuAn
                SET
                    Tenduan = @Tenduan,
                    Motada = @Motada,
                    Ngaybatdau = @Ngaybatdau,
                    Ngayketthuc = @Ngayketthuc,
                    Trangthai = @Trangthai,
                    Nguoitao = @Nguoitao
                WHERE Maduan = @Maduan";

            return _databaseHelper.Execute(sql, duAn);
        }

        public int Delete(string maDuAn)
        {
            string sql = @"
                DELETE FROM DuAn
                WHERE Maduan = @Maduan";

            return _databaseHelper.Execute(
                sql,
                new { Maduan = maDuAn });
        }

        public IEnumerable<DuAn> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM DuAn
                WHERE Maduan LIKE @Keyword
                   OR Tenduan LIKE @Keyword
                   OR Motada LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                ORDER BY Ngaybatdau DESC";

            return _databaseHelper.Query<DuAn>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        // ============================
        // PHÂN TRANG
        // ============================
        public IEnumerable<DuAn> GetPaged(
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
                FROM DuAn
                ORDER BY Ngaybatdau DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            return _databaseHelper.Query<DuAn>(
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
        public IEnumerable<DuAn> SearchPaged(
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
                FROM DuAn
                WHERE Maduan LIKE @Keyword
                   OR Tenduan LIKE @Keyword
                   OR Motada LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                ORDER BY Ngaybatdau DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            return _databaseHelper.Query<DuAn>(
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
                FROM DuAn
                WHERE Maduan LIKE @Keyword
                   OR Tenduan LIKE @Keyword
                   OR Motada LIKE @Keyword
                   OR Trangthai LIKE @Keyword";

            return _databaseHelper.ExecuteScalar<int>(
                sql,
                new
                {
                    Keyword = "%" + (keyword ?? "") + "%"
                });
        }
    }
}
