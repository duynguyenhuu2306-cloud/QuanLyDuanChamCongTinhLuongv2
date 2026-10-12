using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface IPhieuLuongBusiness
    {
        IEnumerable<PhieuLuong> GetAll();

        PhieuLuong GetById(string maPhieuLuong);

        IEnumerable<PhieuLuong> GetByChayLuong(string maChayLuong);

        IEnumerable<PhieuLuong> GetByNguoiDung(string maNguoiDung);

        int Create(PhieuLuong phieuLuong);

        int Update(PhieuLuong phieuLuong);

        int Delete(string maPhieuLuong);

        IEnumerable<PhieuLuong> Search(string keyword);
    }
}
