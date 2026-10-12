using BLL.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AuditLogBusiness : IAuditLogBusiness
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogBusiness(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Audit_Log> GetAll()
        {
            return _repository.GetAll();
        }

        public Audit_Log GetById(string maDangKy)
        {
            return _repository.GetById(maDangKy);
        }

        public IEnumerable<Audit_Log> GetByNguoiDung(string maNguoiDung)
        {
            return _repository.GetByNguoiDung(maNguoiDung);
        }

        public int Create(Audit_Log auditLog)
        {
            return _repository.Create(auditLog);
        }

        public IEnumerable<Audit_Log> Search(string keyword)
        {
            return _repository.Search(keyword);
        }
    }
}
