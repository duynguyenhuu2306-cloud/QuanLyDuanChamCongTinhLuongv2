using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL.Interfaces
{
    public interface ICongViecRepository
    {
        IEnumerable<CongViec> GetAll();

        CongViec GetById(string maCongViec);

        IEnumerable<CongViec> GetByDuAn(string maDuAn);

        int Create(CongViec congViec);

        int Update(CongViec congViec);

        int Delete(string maCongViec);

        IEnumerable<CongViec> Search(string keyword);

        // Phân trang
        IEnumerable<CongViec> GetPaged(int pageNumber, int pageSize);

        // Tìm kiếm + phân trang
        IEnumerable<CongViec> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize);

        // Tổng số bản ghi
        int Count(string keyword);
    }
}
