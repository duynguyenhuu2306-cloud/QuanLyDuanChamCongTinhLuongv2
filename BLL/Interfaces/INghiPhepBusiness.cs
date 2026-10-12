using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface INghiPhepBusiness
    {
        IEnumerable<NghiPhep> GetAll();

        NghiPhep GetById(string maNghiPhep);

        IEnumerable<NghiPhep> GetByNguoiDung(string maNguoiDung);

        IEnumerable<NghiPhep> GetPending();

        int Create(NghiPhep nghiPhep);

        int Update(NghiPhep nghiPhep);

        int Delete(string maNghiPhep);

        int Approve(string maNghiPhep, string nguoiDuyet);

        int Reject(string maNghiPhep, string nguoiDuyet);

        IEnumerable<NghiPhep> Search(string keyword);
    }
}
