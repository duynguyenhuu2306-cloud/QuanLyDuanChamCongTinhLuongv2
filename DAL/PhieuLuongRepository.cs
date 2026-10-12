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
    public class PhieuLuongRepository : IPhieuLuongRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public PhieuLuongRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<PhieuLuong> GetAll()
        {
            string sql = @"
                SELECT *
                FROM PhieuLuong
                ORDER BY Ngaytao DESC";

            return _databaseHelper.Query<PhieuLuong>(sql);
        }

        public PhieuLuong GetById(string maPhieuLuong)
        {
            string sql = @"
                SELECT *
                FROM PhieuLuong
                WHERE Maphieuluong = @Maphieuluong";

            return _databaseHelper.QueryFirstOrDefault<PhieuLuong>(
                sql,
                new { Maphieuluong = maPhieuLuong });
        }

        public IEnumerable<PhieuLuong> GetByChayLuong(string maChayLuong)
        {
            string sql = @"
                SELECT *
                FROM PhieuLuong
                WHERE Machayluong = @Machayluong
                ORDER BY Ngaytao DESC";

            return _databaseHelper.Query<PhieuLuong>(
                sql,
                new { Machayluong = maChayLuong });
        }

        public IEnumerable<PhieuLuong> GetByNguoiDung(string maNguoiDung)
        {
            string sql = @"
                SELECT *
                FROM PhieuLuong
                WHERE Manguoidung = @Manguoidung
                ORDER BY Ngaytao DESC";

            return _databaseHelper.Query<PhieuLuong>(
                sql,
                new { Manguoidung = maNguoiDung });
        }

        public IEnumerable<PhieuLuong> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM PhieuLuong
                WHERE Maphieuluong LIKE @Keyword
                   OR Machayluong LIKE @Keyword
                   OR Manguoidung LIKE @Keyword
                   OR Machinhsach LIKE @Keyword
                ORDER BY Ngaytao DESC";

            return _databaseHelper.Query<PhieuLuong>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(PhieuLuong phieuLuong)
        {
            string sql = @"
                INSERT INTO PhieuLuong
                (
                    Maphieuluong,
                    Machayluong,
                    Manguoidung,
                    Machinhsach,
                    Tonggiolam,
                    Tonggiolamthem,
                    Tongluong,
                    Khoantru,
                    Luongnhanduoc,
                    Ngaytao
                )
                VALUES
                (
                    @Maphieuluong,
                    @Machayluong,
                    @Manguoidung,
                    @Machinhsach,
                    @Tonggiolam,
                    @Tonggiolamthem,
                    @Tongluong,
                    @Khoantru,
                    @Luongnhanduoc,
                    GETDATE()
                )";

            return _databaseHelper.Execute(sql, phieuLuong);
        }

        public int Update(PhieuLuong phieuLuong)
        {
            string sql = @"
                UPDATE PhieuLuong
                SET
                    Machayluong = @Machayluong,
                    Manguoidung = @Manguoidung,
                    Machinhsach = @Machinhsach,
                    Tonggiolam = @Tonggiolam,
                    Tonggiolamthem = @Tonggiolamthem,
                    Tongluong = @Tongluong,
                    Khoantru = @Khoantru,
                    Luongnhanduoc = @Luongnhanduoc
                WHERE Maphieuluong = @Maphieuluong";

            return _databaseHelper.Execute(sql, phieuLuong);
        }

        public int Delete(string maPhieuLuong)
        {
            string sql = @"
                DELETE FROM PhieuLuong
                WHERE Maphieuluong = @Maphieuluong";

            return _databaseHelper.Execute(
                sql,
                new { Maphieuluong = maPhieuLuong });
        }
    }
}
