using DAL.Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace DAL.Helper
{
    public sealed class DatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Chưa cấu hình chuỗi kết nối DefaultConnection.");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public int Execute(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();

            return connection.Execute(
                commandText,
                parameters,
                commandType: commandType);
        }

        public T ExecuteScalar<T>(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();

            return connection.ExecuteScalar<T>(
                commandText,
                parameters,
                commandType: commandType);
        }

        public IEnumerable<T> Query<T>(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();

            return connection.Query<T>(
                commandText,
                parameters,
                commandType: commandType);
        }

        public T QueryFirstOrDefault<T>(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();

            return connection.QueryFirstOrDefault<T>(
                commandText,
                parameters,
                commandType: commandType);
        }
    }
}
