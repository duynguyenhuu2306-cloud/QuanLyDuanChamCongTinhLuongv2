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
            if (chayBangLuong == null || string.IsNullOrWhiteSpace(chayBangLuong.Machayluong) ||
                chayBangLuong.Ngayketthuckyluong.Date < chayBangLuong.Ngaybatdaukyluong.Date)
                return 0;
            chayBangLuong.Trangthai = "Draft";
            return _repository.Create(chayBangLuong);
        }

        public int Update(ChayBangLuong chayBangLuong)
        {
            if (chayBangLuong == null || string.IsNullOrWhiteSpace(chayBangLuong.Machayluong) ||
                chayBangLuong.Ngayketthuckyluong.Date < chayBangLuong.Ngaybatdaukyluong.Date)
                return 0;
            chayBangLuong.Trangthai = "Draft";
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

        public int GeneratePayslips(string maChayLuong) => _repository.GeneratePayslips(maChayLuong);

        public IEnumerable<ChayBangLuong> Search(string keyword)
        {
            return _repository.Search(keyword);
        }
    }
}
