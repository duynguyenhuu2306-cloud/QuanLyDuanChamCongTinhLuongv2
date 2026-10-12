using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface INguoiDungBusiness
    {
        IEnumerable<NguoiDung> GetAll();

        NguoiDung GetById(string maNguoiDung);

        NguoiDung GetByUsername(string tenDangNhap);

        int Create(NguoiDung nguoiDung);

        int Update(NguoiDung nguoiDung);

        int Delete(string maNguoiDung);

        IEnumerable<NguoiDung> Search(string keyword);
    }
}
