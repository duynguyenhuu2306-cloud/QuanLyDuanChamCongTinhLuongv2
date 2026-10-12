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
    public class PhanCongBusiness : IPhanCongBusiness
    {
        private readonly IPhanCongRepository _repository;

        public PhanCongBusiness(IPhanCongRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<PhanCong> GetAll()
        {
            return _repository.GetAll();
        }

        public PhanCong GetById(string maPhanCong)
        {
            return _repository.GetById(maPhanCong);
        }

        public IEnumerable<PhanCong> GetByCongViec(string maCongViec)
        {
            return _repository.GetByCongViec(maCongViec);
        }

        public IEnumerable<PhanCong> GetByNguoiDung(string maNguoiDung)
        {
            return _repository.GetByNguoiDung(maNguoiDung);
        }

        public bool IsProjectManagerForTask(string maCongViec, string maNguoiDung)
        {
            return _repository.IsProjectManagerForTask(maCongViec, maNguoiDung);
        }

        public int Create(PhanCong phanCong)
        {
            return _repository.Create(phanCong);
        }

        public int Update(PhanCong phanCong)
        {
            return _repository.Update(phanCong);
        }

        public int Delete(string maPhanCong)
        {
            return _repository.Delete(maPhanCong);
        }

        public IEnumerable<PhanCong> Search(string keyword)
        {
            return _repository.Search(keyword);
        }

        public IEnumerable<PhanCong> GetPaged(
            int pageNumber,
            int pageSize)
        {
            return _repository.GetPaged(
                pageNumber,
                pageSize);
        }

        public IEnumerable<PhanCong> SearchPaged(
            string keyword,
            int pageNumber,
            int pageSize)
        {
            return _repository.SearchPaged(
                keyword,
                pageNumber,
                pageSize);
        }

        public int Count(string keyword)
        {
            return _repository.Count(keyword);
        }
    }
}
