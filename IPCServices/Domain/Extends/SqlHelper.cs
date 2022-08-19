using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json.Linq;

namespace IPCServices.Domain.Extends
{
    public static class SqlHelper
    {
        public static string ConnectionString = "";
        public static void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public static IEnumerable<T> ExecProcedureData<T>(string procedureName, object parameter = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            IEnumerable<T> result = connection.Query<T>(procedureName, param: parameter,
                commandType: CommandType.StoredProcedure);
            connection.Close();
            return result;
        }

        public static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> ExecProcedureMultipleResult<T1,T2>(string procedureName, object parameter = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            IEnumerable<T1> result1 = null;
            IEnumerable<T2> result2 = null;
            using (var multi = await connection.QueryMultipleAsync(procedureName,parameter, commandType: CommandType.StoredProcedure))
            {
                result1 = multi.Read<T1>().ToList();
                result2 = multi.Read<T2>().ToList();
            }

            connection.Close();
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(result1, result2);
        }

        public static async Task<List<IEnumerable<dynamic>>> ExecProcedureMultipleResult2(string procedureName, JObject jObject = null)
        {
            DynamicParameters parameter = new DynamicParameters();
            foreach (var param in jObject)
            {
                parameter.Add($"@{param.Key}", param.Value.ToString());
            }

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            List< IEnumerable <dynamic>> result = new List<IEnumerable<dynamic>>();
            using (var reader = await connection.QueryMultipleAsync(procedureName, parameter, commandType: CommandType.StoredProcedure))
            {
                while (reader.IsConsumed == false)
                {
                    result.Add(await reader?.ReadAsync<dynamic>());
                }
            }
            connection.Close();
            return result;
        }

        public static async Task<IEnumerable<T>> ExecProcedureDataAsync<T>(string procedureName, object parameter = null)
        {
            await using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            IEnumerable<T> result = await connection.QueryAsync<T>(procedureName, param: parameter,
                commandType: CommandType.StoredProcedure);
            connection.Close();
            return result;
        }
        private static DataTable ExecuteNonQuery(SqlCommand cmd)
        {
            var dtTemp = new DataTable();
            try
            {
                using (var dt = new SqlDataAdapter(cmd))
                {
                    dt.SelectCommand = cmd;
                    cmd.CommandTimeout = 100000;

                    dt.Fill(dtTemp);
                    return dtTemp;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable ExecQueryDataAsDataTable(string procedureName, Dictionary<string, object> parameters = null)
        {
            //await using var connection = new SqlConnection(ConnectionString);
            //connection.Open();
            //DataTable result = (DataTable)connection.Query(procedureName, param: parameter,
            //    commandType: CommandType.StoredProcedure);
            //connection.Close();
            DataTable result = new DataTable();
            using (var conn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter { 
                            ParameterName = param.Key,
                            Value = param.Value.ToString(),
                            DbType = DbType.String
                        });
                    }
                    conn.Open();
                    result = ExecuteNonQuery(cmd);
                }
            }
            return result;
        }

        public static async Task<T> ExecProcedureDataFirstOrDefaultAsync<T>(string procedureName, object parameter = null)
        {
            await using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            T result = await connection.QueryFirstOrDefaultAsync<T>(procedureName, param: parameter,
                commandType: CommandType.StoredProcedure);
            connection.Close();
            return result;
        }

        public static async Task<T> ExecProcedureDataFirstOrDefaultAsync<T>(string procedureName, JObject jObject = null)
        {
            DynamicParameters parameter = new DynamicParameters();
            foreach (var param in jObject)
            {
                parameter.Add($"@{param.Key}", param.Value.ToString());
            }

            await using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            T result = await connection.QueryFirstOrDefaultAsync<T>(procedureName, param: parameter,
                commandType: CommandType.StoredProcedure);
            connection.Close();
            return result;
        }
    }
}
