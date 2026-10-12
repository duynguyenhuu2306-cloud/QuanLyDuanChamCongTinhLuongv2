using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BLL.Interfaces
{
    public interface IAuditLogBusiness
    {
        IEnumerable<Audit_Log> GetAll();

        Audit_Log GetById(string maDangKy);

        IEnumerable<Audit_Log> GetByNguoiDung(string maNguoiDung);

        int Create(Audit_Log auditLog);

        IEnumerable<Audit_Log> Search(string keyword);
    }
}
