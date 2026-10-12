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
    public class ChayBangLuongRepository : IChayBangLuongRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public ChayBangLuongRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<ChayBangLuong> GetAll()
        {
            string sql = @"
                SELECT *
                FROM ChayBangLuong
                ORDER BY Ngaybatdaukyluong DESC";

            return _databaseHelper.Query<ChayBangLuong>(sql);
        }

        public ChayBangLuong GetById(string maChayLuong)
        {
            string sql = @"
                SELECT *
                FROM ChayBangLuong
                WHERE Machayluong = @Machayluong";

            return _databaseHelper.QueryFirstOrDefault<ChayBangLuong>(
                sql,
                new { Machayluong = maChayLuong });
        }

        public IEnumerable<ChayBangLuong> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM ChayBangLuong
                WHERE Machayluong LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                ORDER BY Ngaybatdaukyluong DESC";

            return _databaseHelper.Query<ChayBangLuong>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(ChayBangLuong chayBangLuong)
        {
            string sql = @"
                INSERT INTO ChayBangLuong
                (
                    Machayluong,
                    Ngaybatdaukyluong,
                    Ngayketthuckyluong,
                    Trangthai,
                    Ngaytao
                )
                VALUES
                (
                    @Machayluong,
                    @Ngaybatdaukyluong,
                    @Ngayketthuckyluong,
                    @Trangthai,
                    GETDATE()
                )";

            return _databaseHelper.Execute(sql, chayBangLuong);
        }

        public int Update(ChayBangLuong chayBangLuong)
        {
            string sql = @"
                UPDATE ChayBangLuong
                SET
                    Ngaybatdaukyluong = @Ngaybatdaukyluong,
                    Ngayketthuckyluong = @Ngayketthuckyluong,
                    Trangthai = @Trangthai
                WHERE Machayluong = @Machayluong";

            return _databaseHelper.Execute(sql, chayBangLuong);
        }

        public int Delete(string maChayLuong)
        {
            string sql = @"
                DELETE FROM ChayBangLuong
                WHERE Machayluong = @Machayluong";

            return _databaseHelper.Execute(
                sql,
                new { Machayluong = maChayLuong });
        }

        public int Complete(string maChayLuong)
        {
            string sql = @"
                UPDATE ChayBangLuong
                SET Trangthai = 'Completed'
                WHERE Machayluong = @Machayluong";

            return _databaseHelper.Execute(
                sql,
                new { Machayluong = maChayLuong });
        }
    }
}
