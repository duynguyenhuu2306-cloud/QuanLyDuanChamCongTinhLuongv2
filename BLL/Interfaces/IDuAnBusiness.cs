using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Model;

namespace BLL.Interfaces
{
    public interface IDuAnBusiness
    {
        IEnumerable<DuAn> GetAll();

        DuAn GetById(string maDuAn);

        int Create(DuAn duAn);

        int Update(DuAn duAn);

        int Delete(string maDuAn);

        IEnumerable<DuAn> Search(string keyword);

        IEnumerable<DuAn> GetPaged(int pageNumber, int pageSize);

        // Tìm kiếm + phân trang
        IEnumerable<DuAn> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize);

        // Tổng số bản ghi
        int Count(string keyword);
    }
}
