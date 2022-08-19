using Domain.Domain.Model;
using Domain.Model.Domain.Model;
using System.Collections.Generic;

namespace IPCServices.Services.Interface
{
    public interface IDataRepository
    {
        /// <summary>
        /// Lấy số lượng heo, trả về kết quả double check (true / flase)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<NumberOnlyDto> GetData(GetNumberDto model);

        /// <summary>
        /// Thống kê các lượt cân
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<StatisticalDto> GetThongKe(GetStatisticalDto model);

        public List<DataDto> GetDataAll(GetDataDto model);

        /// <summary>
        /// Cập nhật lượt cân
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DataDto> Update(UpdateDataDto model);

        /// <summary>
        /// Hoàn thành lượt cân
        /// </summary>
        /// <param name="CageId"></param>
        /// <returns></returns>
        public List<StatusDto> FinishWeight(string CageId);


        /// <summary>
        /// Xóa các lượt test có số lượng = 0
        /// </summary>
        /// <returns></returns>
        public List<StatusDto> DeleteNumberTest(DeleteNumberTestDto model);

    }
}
