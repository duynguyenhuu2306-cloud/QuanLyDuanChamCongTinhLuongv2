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
    public class NghiPhepRepository : INghiPhepRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public NghiPhepRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<NghiPhep> GetAll()
        {
            string sql = @"
                SELECT *
                FROM NghiPhep
                ORDER BY Ngaybatdau DESC";

            return _databaseHelper.Query<NghiPhep>(sql);
        }

        public NghiPhep GetById(string maNghiPhep)
        {
            string sql = @"
                SELECT *
                FROM NghiPhep
                WHERE Manghiphep = @Manghiphep";

            return _databaseHelper.QueryFirstOrDefault<NghiPhep>(
                sql,
                new { Manghiphep = maNghiPhep });
        }

        public IEnumerable<NghiPhep> GetByNguoiDung(string maNguoiDung)
        {
            string sql = @"
                SELECT *
                FROM NghiPhep
                WHERE Manguoidung = @Manguoidung
                ORDER BY Ngaybatdau DESC";

            return _databaseHelper.Query<NghiPhep>(
                sql,
                new { Manguoidung = maNguoiDung });
        }

        public IEnumerable<NghiPhep> GetPending()
        {
            string sql = @"
                SELECT *
                FROM NghiPhep
                WHERE Trangthai = 'Pending'
                ORDER BY Ngaybatdau DESC";

            return _databaseHelper.Query<NghiPhep>(sql);
        }

        public IEnumerable<NghiPhep> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM NghiPhep
                WHERE Manghiphep LIKE @Keyword
                   OR Manguoidung LIKE @Keyword
                   OR Loainghiphep LIKE @Keyword
                   OR Trangthai LIKE @Keyword
                   OR Nguoiduyet LIKE @Keyword
                ORDER BY Ngaybatdau DESC";

            return _databaseHelper.Query<NghiPhep>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(NghiPhep nghiPhep)
        {
            string sql = @"
                INSERT INTO NghiPhep
                (
                    Manghiphep,
                    Manguoidung,
                    Loainghiphep,
                    Ngaybatdau,
                    Ngayketthuc,
                    Sogiolam,
                    Trangthai,
                    Nguoiduyet
                )
                VALUES
                (
                    @Manghiphep,
                    @Manguoidung,
                    @Loainghiphep,
                    @Ngaybatdau,
                    @Ngayketthuc,
                    @Sogiolam,
                    @Trangthai,
                    @Nguoiduyet
                )";

            return _databaseHelper.Execute(sql, nghiPhep);
        }

        public int Update(NghiPhep nghiPhep)
        {
            string sql = @"
                UPDATE NghiPhep
                SET
                    Manguoidung = @Manguoidung,
                    Loainghiphep = @Loainghiphep,
                    Ngaybatdau = @Ngaybatdau,
                    Ngayketthuc = @Ngayketthuc,
                    Sogiolam = @Sogiolam,
                    Trangthai = @Trangthai,
                    Nguoiduyet = @Nguoiduyet
                WHERE Manghiphep = @Manghiphep";

            return _databaseHelper.Execute(sql, nghiPhep);
        }

        public int Delete(string maNghiPhep)
        {
            string sql = @"
                DELETE FROM NghiPhep
                WHERE Manghiphep = @Manghiphep";

            return _databaseHelper.Execute(
                sql,
                new { Manghiphep = maNghiPhep });
        }

        public int Approve(string maNghiPhep, string nguoiDuyet)
        {
            string sql = @"
                UPDATE NghiPhep
                SET
                    Trangthai = 'Approved',
                    Nguoiduyet = @Nguoiduyet
                WHERE Manghiphep = @Manghiphep";

            return _databaseHelper.Execute(
                sql,
                new
                {
                    Manghiphep = maNghiPhep,
                    Nguoiduyet = nguoiDuyet
                });
        }

        public int Reject(string maNghiPhep, string nguoiDuyet)
        {
            string sql = @"
                UPDATE NghiPhep
                SET
                    Trangthai = 'Rejected',
                    Nguoiduyet = @Nguoiduyet
                WHERE Manghiphep = @Manghiphep";

            return _databaseHelper.Execute(
                sql,
                new
                {
                    Manghiphep = maNghiPhep,
                    Nguoiduyet = nguoiDuyet
                });
        }
    }
}
