using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface IChayBangLuongBusiness
    {
        IEnumerable<ChayBangLuong> GetAll();

        ChayBangLuong GetById(string maChayLuong);

        int Create(ChayBangLuong chayBangLuong);

        int Update(ChayBangLuong chayBangLuong);

        int Delete(string maChayLuong);

        int Complete(string maChayLuong);

        int GeneratePayslips(string maChayLuong);

        IEnumerable<ChayBangLuong> Search(string keyword);
    }
}
