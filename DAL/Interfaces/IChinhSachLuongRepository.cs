using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL.Interfaces
{
    public interface IChinhSachLuongRepository
    {
        IEnumerable<ChinhSachLuong> GetAll();

        ChinhSachLuong GetById(string maChinhSach);

        IEnumerable<ChinhSachLuong> Search(string keyword);

        int Create(ChinhSachLuong chinhSach);

        int Update(ChinhSachLuong chinhSach);

        int Delete(string maChinhSach);

        ChinhSachLuong GetCurrent();
    }
}
