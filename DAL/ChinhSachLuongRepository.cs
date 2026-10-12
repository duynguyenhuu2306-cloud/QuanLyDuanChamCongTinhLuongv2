using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ChinhSachLuongRepository : IChinhSachLuongRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public ChinhSachLuongRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IEnumerable<ChinhSachLuong> GetAll()
        {
            string sql = @"
                SELECT *
                FROM ChinhSachLuong
                ORDER BY Hieulucngay DESC";

            return _databaseHelper.Query<ChinhSachLuong>(sql);
        }

        public ChinhSachLuong GetById(string maChinhSach)
        {
            string sql = @"
                SELECT *
                FROM ChinhSachLuong
                WHERE Machinhsach = @Machinhsach";

            return _databaseHelper.QueryFirstOrDefault<ChinhSachLuong>(
                sql,
                new { Machinhsach = maChinhSach });
        }

        public IEnumerable<ChinhSachLuong> Search(string keyword)
        {
            string sql = @"
                SELECT *
                FROM ChinhSachLuong
                WHERE Machinhsach LIKE @Keyword
                   OR Quytaclamtron LIKE @Keyword
                ORDER BY Hieulucngay DESC";

            return _databaseHelper.Query<ChinhSachLuong>(
                sql,
                new { Keyword = "%" + keyword + "%" });
        }

        public int Create(ChinhSachLuong chinhSach)
        {
            string sql = @"
                INSERT INTO ChinhSachLuong
                (
                    Machinhsach,
                    Luonggiocoban,
                    Hesonhanlamthem,
                    Quytaclamtron,
                    Hieulucngay
                )
                VALUES
                (
                    @Machinhsach,
                    @Luonggiocoban,
                    @Hesonhanlamthem,
                    @Quytaclamtron,
                    @Hieulucngay
                )";

            return _databaseHelper.Execute(sql, chinhSach);
        }

        public int Update(ChinhSachLuong chinhSach)
        {
            string sql = @"
                UPDATE ChinhSachLuong
                SET
                    Luonggiocoban = @Luonggiocoban,
                    Hesonhanlamthem = @Hesonhanlamthem,
                    Quytaclamtron = @Quytaclamtron,
                    Hieulucngay = @Hieulucngay
                WHERE Machinhsach = @Machinhsach";

            return _databaseHelper.Execute(sql, chinhSach);
        }

        public int Delete(string maChinhSach)
        {
            string sql = @"
                DELETE FROM ChinhSachLuong
                WHERE Machinhsach = @Machinhsach";

            return _databaseHelper.Execute(
                sql,
                new { Machinhsach = maChinhSach });
        }

        public ChinhSachLuong GetCurrent()
        {
            string sql = @"
                SELECT TOP 1 *
                FROM ChinhSachLuong
                WHERE Hieulucngay <= CAST(GETDATE() AS DATE)
                ORDER BY Hieulucngay DESC";

            return _databaseHelper.QueryFirstOrDefault<ChinhSachLuong>(sql);
        }
    }
}
