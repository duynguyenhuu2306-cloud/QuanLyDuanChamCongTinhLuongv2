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
                WHERE Machayluong = @Machayluong AND Trangthai = 'Draft'
                  AND NOT EXISTS (SELECT 1 FROM PhieuLuong WHERE PhieuLuong.Machayluong = ChayBangLuong.Machayluong)";

            return _databaseHelper.Execute(sql, chayBangLuong);
        }

        public int Delete(string maChayLuong)
        {
            string sql = @"
                DELETE FROM ChayBangLuong
                WHERE Machayluong = @Machayluong AND Trangthai = 'Draft'
                  AND NOT EXISTS (SELECT 1 FROM PhieuLuong WHERE PhieuLuong.Machayluong = ChayBangLuong.Machayluong)";

            return _databaseHelper.Execute(
                sql,
                new { Machayluong = maChayLuong });
        }

        public int Complete(string maChayLuong)
        {
            string sql = @"
                UPDATE ChayBangLuong
                SET Trangthai = 'Completed'
                WHERE Machayluong = @Machayluong
                  AND Trangthai = 'Draft'
                  AND EXISTS (SELECT 1 FROM PhieuLuong WHERE PhieuLuong.Machayluong = ChayBangLuong.Machayluong)";

            return _databaseHelper.Execute(
                sql,
                new { Machayluong = maChayLuong });
        }

        public int GeneratePayslips(string maChayLuong)
        {
            const string sql = @"
                SET XACT_ABORT ON;
                BEGIN TRANSACTION;

                DECLARE @NgayBatDau date, @NgayKetThuc date, @TrangThai nvarchar(20);
                SELECT @NgayBatDau = Ngaybatdaukyluong,
                       @NgayKetThuc = Ngayketthuckyluong,
                       @TrangThai = Trangthai
                FROM ChayBangLuong WITH (UPDLOCK, HOLDLOCK)
                WHERE Machayluong = @Machayluong;

                IF @NgayBatDau IS NULL OR @TrangThai <> 'Draft'
                BEGIN
                    ROLLBACK TRANSACTION;
                    SELECT 0;
                    RETURN 0;
                END;

                DECLARE @Inserted int;
                ;WITH Employees AS
                (
                    SELECT Manguoidung FROM NguoiDung
                    WHERE Vaitro = 'Employee' AND Trangthai = 1
                ), Hours AS
                (
                    SELECT E.Manguoidung,
                        CAST(ROUND(COALESCE((SELECT SUM(CC.Sogiolam) FROM ChamCong CC
                            WHERE CC.Manguoidung = E.Manguoidung AND CC.Trangthai = 'Approved'
                              AND CC.IsDeleted = 0 AND CC.Ngaylamviec >= @NgayBatDau
                              AND CC.Ngaylamviec <= @NgayKetThuc), 0) / CS.BuocLamTron, 0) * CS.BuocLamTron AS decimal(10,2)) AS TongGio,
                        CAST(ROUND(COALESCE((SELECT SUM(NP.Sogiolam) FROM NghiPhep NP
                            WHERE NP.Manguoidung = E.Manguoidung AND NP.Loainghiphep = 'OT'
                              AND NP.Trangthai = 'Approved' AND NP.Ngaybatdau >= @NgayBatDau
                              AND NP.Ngaybatdau <= @NgayKetThuc), 0) / CS.BuocLamTron, 0) * CS.BuocLamTron AS decimal(10,2)) AS TongOT,
                        CS.Machinhsach, CS.Luonggiocoban, CS.Hesonhanlamthem
                    FROM Employees E
                    OUTER APPLY
                    (
                        SELECT TOP 1 ChinhSachLuong.*,
                            CAST(CASE
                                WHEN Quytaclamtron LIKE N'%15 phút%' THEN 0.25
                                WHEN Quytaclamtron LIKE N'%30 phút%' THEN 0.5
                                ELSE 1.0
                            END AS decimal(5,2)) AS BuocLamTron
                        FROM ChinhSachLuong
                        WHERE Hieulucngay <= @NgayKetThuc
                        ORDER BY Hieulucngay DESC, Machinhsach DESC
                    ) CS
                )
                INSERT INTO PhieuLuong
                    (Maphieuluong, Machayluong, Manguoidung, Machinhsach,
                     Tonggiolam, Tonggiolamthem, Tongluong, Khoantru, Luongnhanduoc, Ngaytao)
                SELECT CONCAT('PL', RIGHT(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''), 18)),
                    @Machayluong, H.Manguoidung, H.Machinhsach, H.TongGio, H.TongOT,
                    CAST(ROUND((H.TongGio * H.Luonggiocoban) +
                        (H.TongOT * H.Luonggiocoban * H.Hesonhanlamthem), 0) AS decimal(12,2)),
                    0,
                    CAST(ROUND((H.TongGio * H.Luonggiocoban) +
                        (H.TongOT * H.Luonggiocoban * H.Hesonhanlamthem), 0) AS decimal(12,2)),
                    GETDATE()
                FROM Hours H
                WHERE H.Machinhsach IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM PhieuLuong PL
                      WHERE PL.Machayluong = @Machayluong AND PL.Manguoidung = H.Manguoidung);

                SET @Inserted = @@ROWCOUNT;
                COMMIT TRANSACTION;
                SELECT @Inserted;";

            return _databaseHelper.ExecuteScalar<int>(sql, new { Machayluong = maChayLuong });
        }
    }
}
