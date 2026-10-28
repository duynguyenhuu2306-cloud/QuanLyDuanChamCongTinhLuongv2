using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model;

namespace DAL
{
    public class NghiPhepRepository : INghiPhepRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        public NghiPhepRepository(IDatabaseHelper databaseHelper) => _databaseHelper = databaseHelper;

        public IEnumerable<NghiPhep> GetAll() => _databaseHelper.Query<NghiPhep>(@"
            SELECT * FROM NghiPhep ORDER BY Ngaybatdau DESC, Manghiphep DESC");

        public NghiPhep GetById(string maNghiPhep) => _databaseHelper.QueryFirstOrDefault<NghiPhep>(@"
            SELECT * FROM NghiPhep WHERE Manghiphep = @Manghiphep", new { Manghiphep = maNghiPhep });

        public IEnumerable<NghiPhep> GetByNguoiDung(string maNguoiDung) => _databaseHelper.Query<NghiPhep>(@"
            SELECT * FROM NghiPhep WHERE Manguoidung = @Manguoidung
            ORDER BY Ngaybatdau DESC, Manghiphep DESC", new { Manguoidung = maNguoiDung });

        public IEnumerable<NghiPhep> GetPending() => _databaseHelper.Query<NghiPhep>(@"
            SELECT * FROM NghiPhep WHERE Trangthai = 'Pending' ORDER BY Ngaybatdau DESC");

        public bool CanReviewerReview(string maNghiPhep, string maNguoiDuyet) =>
            _databaseHelper.ExecuteScalar<int>(@"
                SELECT COUNT(*)
                FROM NghiPhep NP
                WHERE NP.Manghiphep = @Manghiphep
                  AND NP.Trangthai = 'Pending'
                  AND EXISTS (
                    SELECT 1 FROM PhanCong PC
                    INNER JOIN CongViec CV ON CV.Macongviec = PC.Macongviec
                    INNER JOIN DuAn DA ON DA.Maduan = CV.Maduan
                    WHERE PC.Manguoidung = NP.Manguoidung AND DA.Nguoitao = @Nguoiduyet
                  )", new { Manghiphep = maNghiPhep, Nguoiduyet = maNguoiDuyet }) > 0;

        public IEnumerable<NghiPhep> Search(string keyword) => _databaseHelper.Query<NghiPhep>(@"
            SELECT * FROM NghiPhep
            WHERE Manghiphep LIKE @Keyword OR Manguoidung LIKE @Keyword OR Loainghiphep LIKE @Keyword
               OR Trangthai LIKE @Keyword OR Nguoiduyet LIKE @Keyword OR Lydo LIKE @Keyword
            ORDER BY Ngaybatdau DESC", new { Keyword = "%" + (keyword ?? "") + "%" });

        public decimal GetTotalOtHoursForDay(string maNguoiDung, DateTime ngay, string excludeMaNghiPhep = null) =>
            _databaseHelper.ExecuteScalar<decimal>(@"
                SELECT COALESCE(SUM(Sogiolam), 0) FROM NghiPhep
                WHERE Manguoidung = @Manguoidung AND Loainghiphep = 'OT'
                  AND CAST(Ngaybatdau AS date) = CAST(@Ngay AS date)
                  AND Trangthai IN ('Pending', 'Approved')
                  AND (@ExcludeId IS NULL OR Manghiphep <> @ExcludeId)",
                new { Manguoidung = maNguoiDung, Ngay = ngay, ExcludeId = excludeMaNghiPhep });

        public bool HasOverlappingLeave(string maNguoiDung, DateTime ngayBatDau, DateTime ngayKetThuc, string excludeMaNghiPhep = null) =>
            _databaseHelper.ExecuteScalar<int>(@"
                SELECT COUNT(*) FROM NghiPhep
                WHERE Manguoidung = @Manguoidung AND Loainghiphep = 'Leave'
                  AND Trangthai IN ('Pending', 'Approved')
                  AND Ngaybatdau <= @Ngayketthuc AND Ngayketthuc >= @Ngaybatdau
                  AND (@ExcludeId IS NULL OR Manghiphep <> @ExcludeId)",
                new { Manguoidung = maNguoiDung, Ngaybatdau = ngayBatDau, Ngayketthuc = ngayKetThuc, ExcludeId = excludeMaNghiPhep }) > 0;

        public int Create(NghiPhep nghiPhep) => _databaseHelper.Execute(@"
            INSERT INTO NghiPhep (Manghiphep, Manguoidung, Loainghiphep, Ngaybatdau, Ngayketthuc,
                Sogiolam, Lydo, Trangthai, Nguoiduyet, LydoTuChoi)
            VALUES (@Manghiphep, @Manguoidung, @Loainghiphep, @Ngaybatdau, @Ngayketthuc,
                @Sogiolam, @Lydo, @Trangthai, @Nguoiduyet, @LydoTuChoi)", nghiPhep);

        public int Update(NghiPhep nghiPhep) => _databaseHelper.Execute(@"
            UPDATE NghiPhep SET Loainghiphep = @Loainghiphep, Ngaybatdau = @Ngaybatdau,
                Ngayketthuc = @Ngayketthuc, Sogiolam = @Sogiolam, Lydo = @Lydo,
                Trangthai = @Trangthai, Nguoiduyet = @Nguoiduyet, LydoTuChoi = @LydoTuChoi
            WHERE Manghiphep = @Manghiphep", nghiPhep);

        public int Delete(string maNghiPhep) => _databaseHelper.Execute(
            "DELETE FROM NghiPhep WHERE Manghiphep = @Manghiphep", new { Manghiphep = maNghiPhep });

        public int Approve(string maNghiPhep, string nguoiDuyet) => _databaseHelper.Execute(@"
            UPDATE NghiPhep SET Trangthai = 'Approved', Nguoiduyet = @Nguoiduyet, LydoTuChoi = NULL
            WHERE Manghiphep = @Manghiphep AND Trangthai = 'Pending'",
            new { Manghiphep = maNghiPhep, Nguoiduyet = nguoiDuyet });

        public int Reject(string maNghiPhep, string nguoiDuyet, string lyDoTuChoi) => _databaseHelper.Execute(@"
            UPDATE NghiPhep SET Trangthai = 'Rejected', Nguoiduyet = @Nguoiduyet, LydoTuChoi = @LydoTuChoi
            WHERE Manghiphep = @Manghiphep AND Trangthai = 'Pending'",
            new { Manghiphep = maNghiPhep, Nguoiduyet = nguoiDuyet, LydoTuChoi = lyDoTuChoi });
    }
}
