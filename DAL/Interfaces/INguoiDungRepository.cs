using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface INguoiDungRepository
    {
        // Lấy tất cả người dùng
        IEnumerable<NguoiDung> GetAll();

        // Lấy người dùng theo mã
        NguoiDung GetById(string maNguoiDung);

        // Lấy người dùng theo tên đăng nhập
        NguoiDung GetByUsername(string tenDangNhap);

        // Thêm người dùng
        int Create(NguoiDung nguoiDung);

        // Cập nhật người dùng
        int Update(NguoiDung nguoiDung);

        // Xóa người dùng
        int Delete(string maNguoiDung);

        // Tìm kiếm người dùng
        IEnumerable<NguoiDung> Search(string keyword);
    }
}
