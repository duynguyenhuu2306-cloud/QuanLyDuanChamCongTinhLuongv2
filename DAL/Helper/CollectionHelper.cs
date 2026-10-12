using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DAL.Helper
{
    public static class MessageConvert
    {
        private static readonly JsonSerializerSettings Settings;

        static MessageConvert()
        {
            Settings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public static string SerializeObject(this object obj)
        {
            if (obj == null)
                return "";

            return JsonConvert.SerializeObject(obj, Settings);
        }

        public static T DeserializeObject<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonConvert.DeserializeObject<T>(
                json,
                Settings);
        }

        public static object DeserializeObject(
            this string json,
            Type type)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonConvert.DeserializeObject(
                    json,
                    type,
                    Settings);
            }
            catch
            {
                return null;
            }
        }
    }

    public static class CollectionHelper
    {
        public static object GetPropertyValue(
            this object obj,
            string propName)
        {
            if (obj == null)
                return null;

            PropertyInfo property =
                obj.GetType().GetProperty(propName);

            if (property == null)
                return null;

            return property.GetValue(obj, null);
        }

        public static string GetPropertyValueToString(
            this object obj,
            string propName)
        {
            if (obj == null)
                return "";

            PropertyInfo property =
                obj.GetType().GetProperty(propName);

            if (property == null)
                return "";

            object value =
                property.GetValue(obj, null);

            return Convert.ToString(value);
        }

        public static List<T> GetSourceWithPaging<T>(
            IEnumerable<T> source,
            int pageSize,
            int pageIndex,
            ref int totalPage)
        {
            if (source == null)
            {
                totalPage = 0;
                return new List<T>();
            }

            if (pageSize <= 0)
                pageSize = 10;

            if (pageIndex <= 0)
                pageIndex = 1;

            var enumerable =
                source as T[] ?? source.ToArray();

            int totalRow = enumerable.Count();

            totalPage =
                totalRow % pageSize == 0
                    ? totalRow / pageSize
                    : totalRow / pageSize + 1;

            int skip =
                (pageIndex - 1) * pageSize;

            return enumerable
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

        public static DataTable ConvertTo<T>(
            this IList<T> list)
        {
            DataTable table = CreateTable<T>();

            if (list == null)
                return table;

            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    object value =
                        prop.GetValue(item);

                    row[prop.Name] =
                        value ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static IList<T> ConvertTo<T>(
            IList<DataRow> rows)
        {
            IList<T> list =
                new List<T>();

            if (rows == null)
                return list;

            foreach (DataRow row in rows)
            {
                T item =
                    CreateItem<T>(row);

                list.Add(item);
            }

            return list;
        }

        public static IList<T> ConvertTo<T>(
            this DataTable table)
        {
            if (table == null)
                return new List<T>();

            var rows =
                new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        public static T CreateItem<T>(
            DataRow row)
        {
            T obj = default;

            if (row == null)
                return obj;

            obj =
                Activator.CreateInstance<T>();

            foreach (DataColumn column
                in row.Table.Columns)
            {
                PropertyInfo prop =
                    obj.GetType()
                       .GetProperty(column.ColumnName);

                if (prop == null)
                    continue;

                object value =
                    row[column.ColumnName];

                if (value == DBNull.Value)
                    continue;

                try
                {
                    Type type =
                        prop.PropertyType;

                    Type underlyingType =
                        Nullable.GetUnderlyingType(type);

                    if (underlyingType != null)
                    {
                        type = underlyingType;
                    }

                    if (column.ColumnName
                        .ToLower()
                        .Contains("json"))
                    {
                        object jsonValue =
                            MessageConvert.DeserializeObject(
                                Convert.ToString(value),
                                type);

                        prop.SetValue(
                            obj,
                            jsonValue);
                    }
                    else
                    {
                        object safeValue =
                            Convert.ChangeType(
                                value,
                                type);

                        prop.SetValue(
                            obj,
                            safeValue);
                    }
                }
                catch
                {
                    throw;
                }
            }

            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType =
                typeof(T);

            DataTable table =
                new DataTable(entityType.Name);

            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(
                    entityType);

            foreach (PropertyDescriptor prop
                in properties)
            {
                Type columnType =
                    prop.PropertyType;

                Type underlyingType =
                    Nullable.GetUnderlyingType(
                        columnType);

                if (underlyingType != null)
                {
                    columnType = underlyingType;
                }

                table.Columns.Add(
                    prop.Name,
                    columnType);
            }

            return table;
        }
    }
}
