using BLL.Interfaces;
using System;
using System.Collections.Generic;
using DAL.Interfaces;
using Model;

namespace BLL
{
    public class NguoiDungBusiness : INguoiDungBusiness
    {
        private readonly INguoiDungRepository _repository;

        public NguoiDungBusiness(INguoiDungRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<NguoiDung> GetAll()
        {
            return _repository.GetAll();
        }

        public NguoiDung GetById(string maNguoiDung)
        {
            return _repository.GetById(maNguoiDung);
        }

        public NguoiDung GetByUsername(string tenDangNhap)
        {
            return _repository.GetByUsername(tenDangNhap);
        }

        public int Create(NguoiDung nguoiDung)
        {
            if (nguoiDung == null)
                return 0;

            // Tự động mã hóa mật khẩu trước khi lưu
            nguoiDung.matkhau =
                BCrypt.Net.BCrypt.HashPassword(nguoiDung.matkhau);

            return _repository.Create(nguoiDung);
        }

        public int Update(NguoiDung nguoiDung)
        {
            if (nguoiDung == null)
                return 0;

            // Nếu mật khẩu chưa phải BCrypt hash thì mới mã hóa.
            // Nếu đã là hash thì giữ nguyên, tránh hash 2 lần.
            if (string.IsNullOrEmpty(nguoiDung.matkhau) ||
                !nguoiDung.matkhau.StartsWith("$2"))
            {
                nguoiDung.matkhau =
                    BCrypt.Net.BCrypt.HashPassword(nguoiDung.matkhau);
            }

            return _repository.Update(nguoiDung);
        }

        public int Delete(string maNguoiDung)
        {
            return _repository.Delete(maNguoiDung);
        }

        public IEnumerable<NguoiDung> Search(string keyword)
        {
            return _repository.Search(keyword);
        }
    }
}