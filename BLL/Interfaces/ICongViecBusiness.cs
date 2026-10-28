using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface ICongViecBusiness
    {
        IEnumerable<CongViec> GetAll();

        CongViec GetById(string maCongViec);

        IEnumerable<CongViec> GetByDuAn(string maDuAn);

        IEnumerable<CongViec> GetAssignedToUser(string maNguoiDung);

        int Create(CongViec congViec);

        int Update(CongViec congViec);

        int Delete(string maCongViec);

        IEnumerable<CongViec> Search(string keyword);

        IEnumerable<CongViec> GetPaged(
            int pageNumber,
            int pageSize);

        // Tìm kiếm + phân trang
        IEnumerable<CongViec> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize);

        // Tổng số bản ghi
        int Count(string keyword);
    }
}
