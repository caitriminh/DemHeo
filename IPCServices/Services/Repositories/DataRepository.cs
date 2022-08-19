using Domain.Domain.Model;
using Domain.Model.Domain.Model;
using IPCServices.Services.Interface;
using System.Collections.Generic;
using System.Linq;

namespace IPCServices.Services.Repositories
{
    public class DataRepository : IDataRepository
    {

        private readonly IDapperORM _dapper;

        public DataRepository(IDapperORM dapper)
        {
            _dapper = dapper;
        }


        public List<NumberOnlyDto> GetData(GetNumberDto model)
        {
            return _dapper.ExecProcedureData<NumberOnlyDto>("sp_GetData", new
            {
                model.CageId

            }).ToList();
        }

        public List<StatusDto> FinishWeight(string CageId)
        {
            return _dapper.ExecProcedureData<StatusDto>("spFinish", new
            {
                CageId

            }).ToList();
        }

        public List<StatisticalDto> GetThongKe(GetStatisticalDto model)
        {
            return _dapper.ExecProcedureData<StatisticalDto>("sp_GetThongKeLuotCan", new
            {
                model.action,
                model.FromDate,
                model.ToDate
            }).ToList();
        }

        public List<DataDto> GetDataAll(GetDataDto model)
        {
            return _dapper.ExecProcedureData<DataDto>("sp_GetData_All", new
            {
                model.action,
                model.FromDate,
                model.ToDate

            }).ToList();
        }

        public List<DataDto> Update(UpdateDataDto model)
        {
            return _dapper.ExecProcedureData<DataDto>("sp_UpdateData", new
            {
                model.Id,
                model.Number,
                model.CageId,
                model.FileImage,
                model.CreateId,
                model.CorrectNumber,
                model.NumberLine
            }).ToList();
        }

        public List<StatusDto> DeleteNumberTest(DeleteNumberTestDto model)
        {
            return _dapper.ExecProcedureData<StatusDto>("spDeleteNumberTest", new
            {
                model.CageId
            }).ToList();
        }
    }
}