using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DAL.Interfaces;

namespace BLL
{
    public class ChayBangLuongBusiness : IChayBangLuongBusiness
    {
        private readonly IChayBangLuongRepository _repository;

        public ChayBangLuongBusiness(IChayBangLuongRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ChayBangLuong> GetAll()
        {
            return _repository.GetAll();
        }

        public ChayBangLuong GetById(string maChayLuong)
        {
            return _repository.GetById(maChayLuong);
        }

        public int Create(ChayBangLuong chayBangLuong)
        {
            return _repository.Create(chayBangLuong);
        }

        public int Update(ChayBangLuong chayBangLuong)
        {
            return _repository.Update(chayBangLuong);
        }

        public int Delete(string maChayLuong)
        {
            return _repository.Delete(maChayLuong);
        }

        public int Complete(string maChayLuong)
        {
            return _repository.Complete(maChayLuong);
        }

        public IEnumerable<ChayBangLuong> Search(string keyword)
        {
            return _repository.Search(keyword);
        }
    }
}
