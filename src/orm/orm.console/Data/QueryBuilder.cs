using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace orm.console.Data
{
    public static class QueryBuilder<T>
    {
        public static Dictionary<string, object> GetParams(T item, IEnumerable<PropertyInfo> propertyInfos)
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            foreach (var propInfo in propertyInfos)
            {
                if (propInfo.PropertyType == typeof(string) || propInfo.PropertyType.IsValueType)
                    queryParams.Add($"@{propInfo.Name}", propInfo.GetValue(item));
            }

            return queryParams;
        }

        public static string Select() => $"select * from {typeof(T).Name}s";

        public static string Find(int id) => $"select * from {typeof(T).Name} where id = {id}";

        public static string Insert(T item, IEnumerable<PropertyInfo> propertyInfos)
        {
            StringBuilder query = new StringBuilder($"insert into {typeof(T).Name}s (");

            foreach (var propInfo in propertyInfos)
            {
                if (propInfo.Name.ToLower() != "id")
                {
                    query.Append($"{propInfo.Name}, ");
                }
            }

            query.Remove(query.ToString().Length - 2, 2);
            query.Append(") output inserted.id values (");

            foreach (var propInfo in propertyInfos)
            {
                if (propInfo.Name.ToLower() != "id")
                {
                    query.Append($"@{propInfo.Name}, ");
                }
            }
            query.Remove(query.ToString().Length - 2, 2);
            query.Append(")");

            return query.ToString();
        }

        public static string Update(T item, IEnumerable<PropertyInfo> propertyInfos)
        {
            StringBuilder query = new StringBuilder($"update {typeof(T).Name}s set ");

            foreach (var propInfo in propertyInfos)
            {
                if (propInfo.Name.ToLower() != "id")
                    query.Append($"{propInfo.Name} = @{propInfo.Name}, ");
            }

            query.Remove(query.ToString().Length - 2, 2);
            query.Append(" where id = @id");

            return query.ToString();
        }

        public static string Delete(int id) => $"delete from {typeof(T).Name} where id = {id}";

        public static string Delete(T item, IEnumerable<PropertyInfo> propertyInfos)
        {
            StringBuilder query = new StringBuilder($"delete from {typeof(T).Name}s where ");

            foreach (var propInfo in propertyInfos)
            {
                if (propInfo.Name.ToLower() == "id")
                {
                    query.Append($"{propInfo.Name} = {propInfo.GetValue(item)}");
                    break;
                }
            }
            return query.ToString();
        }
    }
}
