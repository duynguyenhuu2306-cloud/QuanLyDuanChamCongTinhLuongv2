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
    public class ChinhSachLuongBusiness : IChinhSachLuongBusiness
    {
        private readonly IChinhSachLuongRepository _repository;

        public ChinhSachLuongBusiness(
            IChinhSachLuongRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ChinhSachLuong> GetAll()
        {
            return _repository.GetAll();
        }

        public ChinhSachLuong GetById(string maChinhSach)
        {
            return _repository.GetById(maChinhSach);
        }

        public int Create(ChinhSachLuong chinhSach)
        {
            if (!IsValid(chinhSach)) return 0;
            return _repository.Create(chinhSach);
        }

        public int Update(ChinhSachLuong chinhSach)
        {
            if (!IsValid(chinhSach)) return 0;
            return _repository.Update(chinhSach);
        }

        public int Delete(string maChinhSach)
        {
            return _repository.Delete(maChinhSach);
        }

        public ChinhSachLuong GetCurrent()
        {
            return _repository.GetCurrent();
        }

        public IEnumerable<ChinhSachLuong> Search(string keyword)
        {
            return _repository.Search(keyword);
        }

        private static bool IsValid(ChinhSachLuong policy) =>
            policy != null && !string.IsNullOrWhiteSpace(policy.Machinhsach) &&
            policy.Luonggiocoban >= 0 && policy.Hesonhanlamthem >= 1 &&
            !string.IsNullOrWhiteSpace(policy.Quytaclamtron);


    }
}
