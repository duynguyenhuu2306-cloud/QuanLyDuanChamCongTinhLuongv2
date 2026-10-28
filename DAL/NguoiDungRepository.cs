using DAL.Helper;
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
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public NguoiDungRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<NguoiDung> GetAll()
        {
            string sql = @"
        SELECT
            Manguoidung,
            Tendangnhap,
            matkhau,
            Hoten,
            Email,
            Vaitro,
            Ngaytao,
            Trangthai
        FROM NguoiDung
        ORDER BY Ngaytao DESC";

            return _databaseHelper.Query<NguoiDung>(sql);
        }

        public NguoiDung GetById(string maNguoiDung)
        {
            string sql = @"
        SELECT
            Manguoidung,
            Tendangnhap,
            matkhau,
            Hoten,
            Email,
            Vaitro,
            Ngaytao,
            Trangthai
        FROM NguoiDung
        WHERE Manguoidung = @Manguoidung";

            return _databaseHelper.QueryFirstOrDefault<NguoiDung>(
                sql,
                new
                {
                    Manguoidung = maNguoiDung
                });
        }

        public NguoiDung GetByUsername(string tenDangNhap)
        {
            string sql = @"
        SELECT
            Manguoidung,
            Tendangnhap,
            matkhau,
            Hoten,
            Email,
            Vaitro,
            Ngaytao,
            Trangthai
        FROM NguoiDung
        WHERE Tendangnhap = @Tendangnhap";

            return _databaseHelper.QueryFirstOrDefault<NguoiDung>(
                sql,
                new
                {
                    Tendangnhap = tenDangNhap
                });
        }

        public int Create(NguoiDung nguoiDung)
        {
            string sql = @"
        INSERT INTO NguoiDung
        (
            Manguoidung,
            Tendangnhap,
            matkhau,
            Hoten,
            Email,
            Vaitro,
            Ngaytao,
            Trangthai
        )
        VALUES
        (
            @Manguoidung,
            @Tendangnhap,
            @matkhau,
            @Hoten,
            @Email,
            @Vaitro,
            @Ngaytao,
            @Trangthai
        )";

            return _databaseHelper.Execute(sql, nguoiDung);
        }

        public int Update(NguoiDung nguoiDung)
        {
            string sql = @"
        UPDATE NguoiDung
        SET
            Tendangnhap = @Tendangnhap,
            matkhau = CASE
                WHEN NULLIF(@matkhau, '') IS NULL THEN matkhau
                ELSE @matkhau
            END,
            Hoten = @Hoten,
            Email = @Email,
            Vaitro = @Vaitro,
            Trangthai = @Trangthai
        WHERE Manguoidung = @Manguoidung";

            return _databaseHelper.Execute(sql, nguoiDung);
        }

        public int Delete(string maNguoiDung)
        {
            string sql = @"
        DELETE FROM NguoiDung
        WHERE Manguoidung = @Manguoidung";

            return _databaseHelper.Execute(
                sql,
                new
                {
                    Manguoidung = maNguoiDung
                });
        }

        public IEnumerable<NguoiDung> Search(string keyword)
        {
            string sql = @"
        SELECT
            Manguoidung,
            Tendangnhap,
            matkhau,
            Hoten,
            Email,
            Vaitro,
            Ngaytao,
            Trangthai
        FROM NguoiDung
        WHERE
            Manguoidung LIKE @Keyword
            OR Tendangnhap LIKE @Keyword
            OR Hoten LIKE @Keyword
            OR Email LIKE @Keyword
            OR Vaitro LIKE @Keyword
        ORDER BY Ngaytao DESC";

            return _databaseHelper.Query<NguoiDung>(
                sql,
                new
                {
                    Keyword = "%" + keyword + "%"
                });
        }
    }
}
