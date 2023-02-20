using orm.console.Entities;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace orm.console.Data
{
    public class MyORM<T> : IORM<T> where T : IEntity<int>
    {
        private readonly Type _type = typeof(T);
        private readonly IEnumerable<PropertyInfo> _propertyInfos;
        private readonly string _connectionString;
        private IList<Dictionary<string, Dictionary<string, object>>> _sqlQueries;

        public MyORM(string connectionString)
        {
            _connectionString = connectionString;
            _propertyInfos = _type.GetProperties();
            _sqlQueries = new List<Dictionary<string, Dictionary<string, object>>>();
        }

        #region Get All Records From Tables Recursively
        public void GetAll()
        {
            int totalRecord = default;
            var type = typeof(T);
            StringBuilder query = new StringBuilder($"select * from {type.Name} ");
            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (
                    !propertyInfo.PropertyType.IsPrimitive &&
                    propertyInfo.PropertyType != typeof(string) &&
                    propertyInfo.PropertyType != typeof(DateTime)
                    )
                {
                    if (propertyInfo.PropertyType.IsGenericType)
                        query.Append(GetItemById(propertyInfo.PropertyType.GetGenericArguments()[0], type.Name));
                    else
                        query.Append(GetItemById(propertyInfo.PropertyType, type.Name));
                }
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                    {
                        while (reader.Read())
                        {
                            totalRecord++;
                            foreach (var col in reader.GetColumnSchema())
                            {
                                Console.WriteLine($"{col.ColumnName}: {reader[col.ColumnName]}");
                            }

                            Console.WriteLine("------------------------------------------------");
                        }
                    }
                }
            }

            Console.WriteLine($"Total Records: {totalRecord}");
        }

        public void GetById(int id)
        {
            int totalRecord = default;
            var type = typeof(T);
            StringBuilder query = new StringBuilder($"select * from {type.Name} ");
            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (
                    !propertyInfo.PropertyType.IsPrimitive &&
                    propertyInfo.PropertyType != typeof(string) &&
                    propertyInfo.PropertyType != typeof(DateTime)
                    )
                {
                    if (propertyInfo.PropertyType.IsGenericType)
                        query.Append(GetItemById(propertyInfo.PropertyType.GetGenericArguments()[0], type.Name));
                    else
                        query.Append(GetItemById(propertyInfo.PropertyType, type.Name));
                }
            }
            query.Append($"where {type.Name}.id = {id}");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                    {
                        Console.WriteLine($"Total Column: {reader.FieldCount}");

                        while (reader.Read())
                        {
                            totalRecord++;
                            foreach (var col in reader.GetColumnSchema())
                            {
                                Console.WriteLine($"{col.ColumnName}: {reader[col.ColumnName]}");
                            }

                            Console.WriteLine("------------------------------------------------");
                        }
                    }
                }
            }

            Console.WriteLine($"Total Records: {totalRecord}");
        }

        private string GetItemById(Type type, string parentTable)
        {
            StringBuilder joinQuery = new StringBuilder();
            joinQuery.Append($"left join {type.Name} on {type.Name}.{parentTable}_id = {parentTable}.id ");
            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.PropertyType.IsPrimitive &&
                    propertyInfo.PropertyType != typeof(string) &&
                    propertyInfo.PropertyType != typeof(DateTime)
                    )
                {
                    if (propertyInfo.PropertyType.IsGenericType)
                        joinQuery.Append(GetItemById(propertyInfo.PropertyType.GetGenericArguments()[0], type.Name));
                    else
                        joinQuery.Append(GetItemById(propertyInfo.PropertyType, type.Name));
                }
            }

            return joinQuery.ToString();
        }
        #endregion

        #region Insert into multiple tables Recursively
        public void Insert(T item)
        {
            Store(item);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    foreach (var sqlQuery in _sqlQueries)
                    {
                        foreach (var query in sqlQuery)
                        {
                            using (SqlCommand cmd = new SqlCommand(query.Key, conn, tran))
                            {
                                foreach (var value in query.Value)
                                {
                                    cmd.Parameters.Add(new SqlParameter(value.Key, value.Value));
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();
                    Console.WriteLine("Data successfully inserted!");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Store<T1>(T1 item)
        {
            Type type = item.GetType();
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            var isArray = typeof(IEnumerable).IsAssignableFrom(item.GetType());
            StringBuilder query = new StringBuilder();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (isArray)
            {
                foreach (var i in item as IEnumerable)
                {
                    Store(i);
                }
            }
            else
            {
                query.Append($"insert into {type.Name} (");

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
                    {
                        query.Append($"{property.Name}, ");
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        query.Append($"{property.Name}, ");
                    }
                    else
                    {
                        if (property.GetValue(item) != null)
                            Store(property.GetValue(item));
                    }
                }
                query.Remove(query.ToString().Length - 2, 2);
                query.Append(") values (");

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
                    {
                        query.Append($"@{property.Name}, ");
                        queryParams.Add($"@{property.Name}", property.GetValue(item));
                    }
                }
                query.Remove(query.ToString().Length - 2, 2);
                query.Append(")");

                /*
                 * Here we are making a Key, Value pairs
                 * of sql query and Query Paramters
                 * and assign it to our List of sqlQuery
                 */
                Dictionary<string, Dictionary<string, object>> keyValuePairs =
                    new Dictionary<string, Dictionary<string, object>>();

                keyValuePairs.Add(query.ToString(), queryParams);
                _sqlQueries.Add(keyValuePairs);
            }
        }
        #endregion

        #region Update multiple tables Recursively
        public void Update(T item)
        {
            UpdateItem(item);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    foreach (var sqlQuery in _sqlQueries)
                    {
                        foreach (var query in sqlQuery)
                        {
                            using (SqlCommand cmd = new SqlCommand(query.Key, conn, tran))
                            {
                                foreach (var value in query.Value)
                                {
                                    cmd.Parameters.Add(new SqlParameter(value.Key, value.Value));
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();
                    Console.WriteLine("Data successfully Updated!");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void UpdateItem<T2>(T2 item)
        {
            Type type = item.GetType();
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            var isArray = typeof(IEnumerable).IsAssignableFrom(item.GetType());
            StringBuilder query = new StringBuilder();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (isArray)
            {
                foreach (var i in item as IEnumerable)
                {
                    UpdateItem(i);
                }
            }
            else
            {
                query.Append($"update {type.Name} set ");

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.ToLower() != "id")
                    {
                        if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType || property.PropertyType == typeof(DateTime))
                        {
                            query.Append($@"{property.Name} = @{property.Name}, ");
                        }
                        else
                        {
                            if (property.GetValue(item) != null)
                                UpdateItem(property.GetValue(item));
                        }
                    }
                }
                query.Remove(query.ToString().Length - 2, 2);
                query.Append(" where id = @id");

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
                    {
                        queryParams.Add($"@{property.Name}", property.GetValue(item));
                    }
                }

                /*
                 * Here we are making a Key, Value pairs
                 * of sql query and Query Paramters
                 * and assign it to our List of sqlQuery
                 */
                Dictionary<string, Dictionary<string, object>> keyValuePairs =
                    new Dictionary<string, Dictionary<string, object>>();

                keyValuePairs.Add(query.ToString(), queryParams);
                _sqlQueries.Add(keyValuePairs);
            }
        }
        #endregion

        #region Delete by item from multiple table Recursively
        public void Delete(T item)
        {
            Destroy(item);
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    foreach (var sqlQuery in _sqlQueries)
                    {
                        foreach (var query in sqlQuery)
                        {
                            using (SqlCommand cmd = new SqlCommand(query.Key, conn, tran))
                            {
                                foreach (var value in query.Value)
                                {
                                    cmd.Parameters.Add(new SqlParameter(value.Key, value.Value));
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();
                    Console.WriteLine("Data successfully Deleted!");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Destroy<T2>(T2 item)
        {
            Type type = item.GetType();
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            var isArray = typeof(IEnumerable).IsAssignableFrom(item.GetType());
            StringBuilder query = new StringBuilder();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (isArray)
            {
                foreach (var i in item as IEnumerable)
                {
                    Destroy(i);
                }
            }
            else
            {
                query.Append($"delete from {type.Name} ");

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) ||
                        property.PropertyType.IsValueType ||
                        property.PropertyType == typeof(DateTime)
                        )
                    {
                        if (property.Name.ToLower() == "id")
                        {
                            query.Append(" where id = @id");
                            queryParams.Add($"@{property.Name}", property.GetValue(item));
                        }
                    }
                    else
                    {
                        if (property.GetValue(item) != null)
                            Destroy(property.GetValue(item));
                    }
                }

                Console.WriteLine(query.ToString());

                /*
                 * Here we are making a Key, Value pairs
                 * of sql query and Query Paramters
                 * and assign it to our List of sqlQuery
                 */
                Dictionary<string, Dictionary<string, object>> keyValuePairs =
                    new Dictionary<string, Dictionary<string, object>>();

                keyValuePairs.Add(query.ToString(), queryParams);
                _sqlQueries.Add(keyValuePairs);
            }
        }
        #endregion

        #region Delete by id
        public void Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var query = QueryBuilder<T>.Delete(id);

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    if (cmd.ExecuteNonQuery() >= 1)
                        Console.WriteLine("Data successfully deleted");
                    else
                        Console.WriteLine("Oops! We counld not found the data.");
                }
            }
        }
        #endregion
    }
}
