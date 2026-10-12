using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using Model;

namespace BLL
{
    public class NghiPhepBusiness : INghiPhepBusiness
    {
        private readonly INghiPhepRepository _repository;

        public NghiPhepBusiness(INghiPhepRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<NghiPhep> GetAll()
        {
            return _repository.GetAll();
        }

        public NghiPhep GetById(string maNghiPhep)
        {
            return _repository.GetById(maNghiPhep);
        }

        public IEnumerable<NghiPhep> GetByNguoiDung(string maNguoiDung)
        {
            return _repository.GetByNguoiDung(maNguoiDung);
        }

        public IEnumerable<NghiPhep> GetPending()
        {
            return _repository.GetPending();
        }

        public int Create(NghiPhep nghiPhep)
        {
            return _repository.Create(nghiPhep);
        }

        public int Update(NghiPhep nghiPhep)
        {
            return _repository.Update(nghiPhep);
        }

        public int Delete(string maNghiPhep)
        {
            return _repository.Delete(maNghiPhep);
        }

        public int Approve(string maNghiPhep, string nguoiDuyet)
        {
            return _repository.Approve(maNghiPhep, nguoiDuyet);
        }

        public int Reject(string maNghiPhep, string nguoiDuyet)
        {
            return _repository.Reject(maNghiPhep, nguoiDuyet);
        }

        public IEnumerable<NghiPhep> Search(string keyword)
        {
            return _repository.Search(keyword);
        }
    }
}
