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
    public class CongViecBusiness : ICongViecBusiness
    {
        private readonly ICongViecRepository _repository;

        public CongViecBusiness(ICongViecRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<CongViec> GetAll()
        {
            return _repository.GetAll();
        }

        public CongViec GetById(string maCongViec)
        {
            return _repository.GetById(maCongViec);
        }

        public IEnumerable<CongViec> GetByDuAn(string maDuAn)
        {
            return _repository.GetByDuAn(maDuAn);
        }

        public int Create(CongViec congViec)
        {
            return _repository.Create(congViec);
        }

        public int Update(CongViec congViec)
        {
            return _repository.Update(congViec);
        }

        public int Delete(string maCongViec)
        {
            return _repository.Delete(maCongViec);
        }

        public IEnumerable<CongViec> Search(string keyword)
        {
            return _repository.Search(keyword);
        }

        public IEnumerable<CongViec> GetPaged(
            int pageNumber,
            int pageSize)
        {
            return _repository.GetPaged(
                pageNumber,
                pageSize);
        }

        public IEnumerable<CongViec> SearchPaged(
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
