using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL.Interfaces
{
    public interface IAuditLogRepository
    {
        IEnumerable<Audit_Log> GetAll();

        Audit_Log GetById(string maDangKy);

        IEnumerable<Audit_Log> GetByNguoiDung(string maNguoiDung);

        IEnumerable<Audit_Log> Search(string keyword);

        int Create(Audit_Log auditLog);
    }
}
