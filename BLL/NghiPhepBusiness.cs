using BLL.Interfaces;
using DAL.Interfaces;
using Model;

namespace BLL
{
    public class NghiPhepBusiness : INghiPhepBusiness
    {
        private const decimal HoursPerWorkDay = 8m;
        private const decimal MaximumOtHoursPerDay = 4m;
        private readonly INghiPhepRepository _repository;

        public NghiPhepBusiness(INghiPhepRepository repository) => _repository = repository;

        public IEnumerable<NghiPhep> GetAll() => _repository.GetAll();
        public NghiPhep GetById(string maNghiPhep) => _repository.GetById(maNghiPhep);
        public IEnumerable<NghiPhep> GetByNguoiDung(string maNguoiDung) => _repository.GetByNguoiDung(maNguoiDung);
        public IEnumerable<NghiPhep> GetPending() => _repository.GetPending();
        public bool CanReviewerReview(string maNghiPhep, string maNguoiDuyet) => _repository.CanReviewerReview(maNghiPhep, maNguoiDuyet);
        public IEnumerable<NghiPhep> Search(string keyword) => _repository.Search(keyword);
        public decimal GetTotalOtHoursForDay(string maNguoiDung, DateTime ngay, string excludeMaNghiPhep = null)
            => _repository.GetTotalOtHoursForDay(maNguoiDung, ngay, excludeMaNghiPhep);

        public int Create(NghiPhep nghiPhep)
        {
            if (!PrepareAndValidate(nghiPhep)) return 0;
            nghiPhep.Trangthai = "Pending";
            nghiPhep.Nguoiduyet = null;
            nghiPhep.LydoTuChoi = null;
            return _repository.Create(nghiPhep);
        }

        public int Update(NghiPhep nghiPhep)
        {
            var current = _repository.GetById(nghiPhep.Manghiphep);
            if (current == null || (current.Trangthai != "Pending" && current.Trangthai != "Rejected")) return 0;
            if (!PrepareAndValidate(nghiPhep, nghiPhep.Manghiphep)) return 0;

            nghiPhep.Trangthai = "Pending";
            nghiPhep.Nguoiduyet = null;
            nghiPhep.LydoTuChoi = null;
            return _repository.Update(nghiPhep);
        }

        public int Delete(string maNghiPhep)
        {
            var current = _repository.GetById(maNghiPhep);
            return current == null || (current.Trangthai != "Pending" && current.Trangthai != "Rejected")
                ? 0
                : _repository.Delete(maNghiPhep);
        }

        public int Approve(string maNghiPhep, string nguoiDuyet) => _repository.Approve(maNghiPhep, nguoiDuyet);
        public int Reject(string maNghiPhep, string nguoiDuyet, string lyDoTuChoi) => _repository.Reject(maNghiPhep, nguoiDuyet, lyDoTuChoi);

        private bool PrepareAndValidate(NghiPhep request, string excludeMaNghiPhep = null)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Manguoidung) ||
                string.IsNullOrWhiteSpace(request.Lydo) || request.Ngayketthuc.Date < request.Ngaybatdau.Date)
                return false;

            request.Loainghiphep = request.Loainghiphep?.Trim();
            // SQL schema uses the canonical value "Leave" (see QuanLyDuAn.sql).
            if (request.Loainghiphep == "Leave")
            {
                var workDays = CountWorkDays(request.Ngaybatdau, request.Ngayketthuc);
                if (workDays == 0 || _repository.HasOverlappingLeave(
                        request.Manguoidung, request.Ngaybatdau, request.Ngayketthuc, excludeMaNghiPhep))
                    return false;

                request.Sogiolam = workDays * HoursPerWorkDay;
                return true;
            }

            if (request.Loainghiphep != "OT" || request.Ngaybatdau.Date != request.Ngayketthuc.Date ||
                !request.Sogiolam.HasValue || request.Sogiolam.Value <= 0 || request.Sogiolam.Value > MaximumOtHoursPerDay)
                return false;

            return _repository.GetTotalOtHoursForDay(
                request.Manguoidung, request.Ngaybatdau, excludeMaNghiPhep) + request.Sogiolam.Value <= MaximumOtHoursPerDay;
        }

        private static int CountWorkDays(DateTime start, DateTime end)
        {
            var days = 0;
            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                if (day.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday) days++;
            }
            return days;
        }
    }
}
