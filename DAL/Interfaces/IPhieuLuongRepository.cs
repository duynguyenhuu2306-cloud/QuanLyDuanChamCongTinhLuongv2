using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPhieuLuongRepository
    {
        IEnumerable<PhieuLuong> GetAll();

        PhieuLuong GetById(string maPhieuLuong);

        IEnumerable<PhieuLuong> GetByChayLuong(string maChayLuong);

        IEnumerable<PhieuLuong> GetByNguoiDung(string maNguoiDung);

        IEnumerable<PhieuLuong> Search(string keyword);

        int Create(PhieuLuong phieuLuong);

        int Update(PhieuLuong phieuLuong);

        int Delete(string maPhieuLuong);
    }
}
