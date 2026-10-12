using BLL.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ChamCongBusiness : IChamCongBusiness
    {
        private readonly IChamCongRepository _repository;

        public ChamCongBusiness(IChamCongRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ChamCong> GetAll()
        {
            return _repository.GetAll();
        }

        public ChamCong GetById(string maBangChamCong)
        {
            return _repository.GetById(maBangChamCong);
        }

        public IEnumerable<ChamCong> GetByNguoiDung(string maNguoiDung)
        {
            return _repository.GetByNguoiDung(maNguoiDung);
        }

        public IEnumerable<ChamCong> GetPending()
        {
            return _repository.GetPending();
        }

        public decimal GetTotalHoursForDay(string maNguoiDung, DateTime ngayLamViec, string excludeMaBangChamCong = null)
        {
            return _repository.GetTotalHoursForDay(maNguoiDung, ngayLamViec, excludeMaBangChamCong);
        }

        public bool IsTaskAssignedToUser(string maCongViec, string maNguoiDung)
        {
            return _repository.IsTaskAssignedToUser(maCongViec, maNguoiDung);
        }

        public bool CanReviewerReview(string maBangChamCong, string maNguoiDuyet)
        {
            return _repository.CanReviewerReview(maBangChamCong, maNguoiDuyet);
        }

        public int Create(ChamCong chamCong)
        {
            if (!IsValidHours(chamCong) || !IsTaskAssignedToUser(chamCong.Macongviec, chamCong.Manguoidung))
                return 0;

            if (GetTotalHoursForDay(chamCong.Manguoidung, chamCong.Ngaylamviec) + chamCong.Sogiolam!.Value > 8)
                return 0;

            chamCong.Trangthai = "Pending";
            chamCong.Nguoiduyet = null;
            chamCong.LydoTuChoi = null;
            chamCong.IsDeleted = false;
            return _repository.Create(chamCong);
        }

        public int Update(ChamCong chamCong)
        {
            var current = _repository.GetById(chamCong.Mabangchamcong);
            if (current == null || (current.Trangthai != "Pending" && current.Trangthai != "Rejected"))
                return 0;

            if (!IsValidHours(chamCong) || !IsTaskAssignedToUser(chamCong.Macongviec, chamCong.Manguoidung))
                return 0;

            if (GetTotalHoursForDay(chamCong.Manguoidung, chamCong.Ngaylamviec, chamCong.Mabangchamcong) + chamCong.Sogiolam!.Value > 8)
                return 0;

            chamCong.Trangthai = "Pending";
            chamCong.Nguoiduyet = null;
            chamCong.LydoTuChoi = null;
            return _repository.Update(chamCong);
        }

        public int Delete(string maBangChamCong)
        {
            var current = _repository.GetById(maBangChamCong);
            if (current == null || (current.Trangthai != "Pending" && current.Trangthai != "Rejected"))
                return 0;

            return _repository.Delete(maBangChamCong);
        }

        public int Approve(string maBangChamCong, string nguoiDuyet)
        {
            return _repository.Approve(maBangChamCong, nguoiDuyet);
        }

        public int Reject(string maBangChamCong, string nguoiDuyet, string lyDoTuChoi)
        {
            return _repository.Reject(maBangChamCong, nguoiDuyet, lyDoTuChoi);
        }

        public IEnumerable<ChamCong> Search(string keyword)
        {
            return _repository.Search(keyword);
        }

        private static bool IsValidHours(ChamCong chamCong)
        {
            return chamCong.Sogiolam.HasValue && chamCong.Sogiolam.Value > 0 && chamCong.Sogiolam.Value <= 8;
        }
    }
}
