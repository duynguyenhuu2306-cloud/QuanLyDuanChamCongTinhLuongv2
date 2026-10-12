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
    public class PhieuLuongBusiness : IPhieuLuongBusiness
    {
        private readonly IPhieuLuongRepository _repository;

        public PhieuLuongBusiness(IPhieuLuongRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<PhieuLuong> GetAll()
        {
            return _repository.GetAll();
        }

        public PhieuLuong GetById(string maPhieuLuong)
        {
            return _repository.GetById(maPhieuLuong);
        }

        public IEnumerable<PhieuLuong> GetByChayLuong(string maChayLuong)
        {
            return _repository.GetByChayLuong(maChayLuong);
        }

        public IEnumerable<PhieuLuong> GetByNguoiDung(string maNguoiDung)
        {
            return _repository.GetByNguoiDung(maNguoiDung);
        }

        public int Create(PhieuLuong phieuLuong)
        {
            return _repository.Create(phieuLuong);
        }

        public int Update(PhieuLuong phieuLuong)
        {
            return _repository.Update(phieuLuong);
        }

        public int Delete(string maPhieuLuong)
        {
            return _repository.Delete(maPhieuLuong);
        }

        public IEnumerable<PhieuLuong> Search(string keyword)
        {
            return _repository.Search(keyword);
        }
    }
}
