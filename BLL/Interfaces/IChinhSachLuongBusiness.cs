using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface IChinhSachLuongBusiness
    {
        IEnumerable<ChinhSachLuong> GetAll();

        ChinhSachLuong GetById(string maChinhSach);

        int Create(ChinhSachLuong chinhSach);

        int Update(ChinhSachLuong chinhSach);

        int Delete(string maChinhSach);

        ChinhSachLuong GetCurrent();

        IEnumerable<ChinhSachLuong> Search(string keyword);
    }
}
