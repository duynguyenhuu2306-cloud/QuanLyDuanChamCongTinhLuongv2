using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Model;

namespace DAL.Interfaces
{
    public interface IDuAnRepository
    {
        IEnumerable<DuAn> GetAll();

        DuAn GetById(string maDuAn);

        int Create(DuAn duAn);

        int Update(DuAn duAn);

        int Delete(string maDuAn);

        IEnumerable<DuAn> Search(string keyword);

        // Phân trang
        IEnumerable<DuAn> GetPaged(int pageNumber, int pageSize);

        // Phân trang kết hợp tìm kiếm
        IEnumerable<DuAn> SearchPaged(string keyword, int pageNumber, int pageSize);

        // Tổng số bản ghi
        int Count(string keyword);
    }
}
