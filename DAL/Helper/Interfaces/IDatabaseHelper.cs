using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Helper.Interfaces
{
    public interface IDatabaseHelper
    {
        int Execute(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);

        T ExecuteScalar<T>(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);

        IEnumerable<T> Query<T>(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);

        T QueryFirstOrDefault<T>(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);
    }
}
