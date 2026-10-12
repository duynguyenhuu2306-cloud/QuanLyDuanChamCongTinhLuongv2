using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL.Interfaces
{
    public interface IPhanCongRepository
    {
        IEnumerable<PhanCong> GetAll();

        PhanCong GetById(string maPhanCong);

        IEnumerable<PhanCong> GetByCongViec(string maCongViec);

        IEnumerable<PhanCong> GetByDuAn(string maDuAn);

        bool IsProjectManagerForTask(string maCongViec, string maNguoiDung);

        IEnumerable<PhanCong> GetByNguoiDung(string maNguoiDung);

        IEnumerable<PhanCong> Search(string keyword);

        int Create(PhanCong phanCong);

        int Update(PhanCong phanCong);

        int Delete(string maPhanCong);

        IEnumerable<PhanCong> GetPaged(
            int pageNumber,
            int pageSize);

        IEnumerable<PhanCong> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize);

        int Count(string keyword);
    }
}
