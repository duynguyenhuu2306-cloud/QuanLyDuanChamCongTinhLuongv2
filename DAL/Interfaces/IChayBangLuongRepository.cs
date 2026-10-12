using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL.Interfaces
{
    public interface IChayBangLuongRepository
    {
        IEnumerable<ChayBangLuong> GetAll();

        ChayBangLuong GetById(string maChayLuong);

        IEnumerable<ChayBangLuong> Search(string keyword);

        int Create(ChayBangLuong chayBangLuong);

        int Update(ChayBangLuong chayBangLuong);

        int Delete(string maChayLuong);

        int Complete(string maChayLuong);
    }
}
