using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace IPCServices.Services.Interface
{

    public interface IDapperORM : IDisposable
    {
        DbConnection GetDbconnection();

        T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        IEnumerable<T> ExecProcedureData<T>(string ProcedureName, object parametter = null);

        int Execute(string ProcedureName, object parametter = null);
    }
}
