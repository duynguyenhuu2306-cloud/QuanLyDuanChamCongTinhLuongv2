using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using Model.Model;
using BLL.Interfaces;
namespace BLL
{
    public class DuAnBusiness : IDuAnBusiness
    {
        private readonly IDuAnRepository _repository;

        public DuAnBusiness(IDuAnRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<DuAn> GetAll()
        {
            return _repository.GetAll();
        }

        public DuAn GetById(string maDuAn)
        {
            return _repository.GetById(maDuAn);
        }

        public int Create(DuAn duAn)
        {
            return _repository.Create(duAn);
        }

        public int Update(DuAn duAn)
        {
            return _repository.Update(duAn);
        }

        public int Delete(string maDuAn)
        {
            return _repository.Delete(maDuAn);
        }

        public IEnumerable<DuAn> Search(string keyword)
        {
            return _repository.Search(keyword);
        }

        public IEnumerable<DuAn> GetPaged(
            int pageNumber,
            int pageSize)
        {
            return _repository.GetPaged(
                pageNumber,
                pageSize);
        }

        public IEnumerable<DuAn> SearchPaged(
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
