using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface IPhanCongBusiness
    {
        IEnumerable<PhanCong> GetAll();

        PhanCong GetById(string maPhanCong);

        IEnumerable<PhanCong> GetByCongViec(string maCongViec);

        IEnumerable<PhanCong> GetByNguoiDung(string maNguoiDung);

        bool IsProjectManagerForTask(string maCongViec, string maNguoiDung);

        int Create(PhanCong phanCong);

        int Update(PhanCong phanCong);

        int Delete(string maPhanCong);

        IEnumerable<PhanCong> Search(string keyword);

        IEnumerable<PhanCong> GetPaged(
            int pageNumber,
            int pageSize);

        // Tìm kiếm + phân trang
        IEnumerable<PhanCong> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize);

        // Tổng số bản ghi
        int Count(string keyword);


    }
}
