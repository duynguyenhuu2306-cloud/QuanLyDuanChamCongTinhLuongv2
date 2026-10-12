using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL.Interfaces
{
    public interface IChamCongRepository
    {
        IEnumerable<ChamCong> GetAll();

        ChamCong GetById(string maBangChamCong);

        IEnumerable<ChamCong> GetByNguoiDung(string maNguoiDung);

        IEnumerable<ChamCong> GetPending();

        IEnumerable<ChamCong> Search(string keyword);

        decimal GetTotalHoursForDay(string maNguoiDung, DateTime ngayLamViec, string excludeMaBangChamCong = null);

        bool IsTaskAssignedToUser(string maCongViec, string maNguoiDung);

        bool CanReviewerReview(string maBangChamCong, string maNguoiDuyet);

        int Create(ChamCong chamCong);

        int Update(ChamCong chamCong);

        int Delete(string maBangChamCong);

        int Approve(string maBangChamCong, string nguoiDuyet);

        int Reject(string maBangChamcong, string nguoiDuyet, string lyDoTuChoi);
    }
}
