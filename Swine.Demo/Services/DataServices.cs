using DevExpress.XtraEditors;
using Domain.Domain.Model;
using Domain.Model.Domain.Model;
using Newtonsoft.Json;
using Swine.Demo.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Swine.Demo.Services
{
    internal class DataServices
    {
        private readonly ApiHelper api = null;

        public DataServices()
        {
            api = new ApiHelper();
        }

        /// <summary>
        /// Tìm kiếm lượt cân theo ngày
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<List<DataDto>> GetData(DateTime FromDate, DateTime ToDate)
        {
            var body = new GetDataDto()
            {
                action = "GET_BY_DATE",
                FromDate = FromDate,
                ToDate = ToDate
            };
            var result = await api.PostAsync("Data/GetDataAll", body);
            if (result.StatusCode != 200)
            {
                XtraMessageBox.Show("Lỗi không kết nối với máy chủ.", "Cảnh Báo");
                return null;
            }
            var data = JsonConvert.DeserializeObject<List<DataDto>>(result.Result.ToString());
            return data;
        }

        /// <summary>
        /// Lấy dữ liệu lượt cần bỏ qua các lượt cân heo nái (SL >1)
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<List<DataDto>> GetData_BoQuaHeoNai(DateTime FromDate, DateTime ToDate)
        {
            var body = new GetDataDto()
            {
                action = "GET_BY_DATE_BOQUA_HEONAI",
                FromDate = FromDate,
                ToDate = ToDate
            };
            var result = await api.PostAsync("Data/GetDataAll", body);
            if (result.StatusCode != 200)
            {
                XtraMessageBox.Show("Lỗi không kết nối với máy chủ.", "Cảnh Báo");
                return null;
            }
            var data = JsonConvert.DeserializeObject<List<DataDto>>(result.Result.ToString());
            return data;
        }


        /// <summary>
        /// Thống kê các lượt cân
        /// </summary>
        /// <param name="action"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<List<StatisticalDto>> GetThongKe(string action, DateTime FromDate, DateTime ToDate)
        {
            var body = new GetStatisticalDto()
            {
                action = action,
                FromDate = FromDate,
                ToDate = ToDate
            };
            var result = await api.PostAsync("Data/GetThongKe", body);
            if (result.StatusCode != 200)
            {
                XtraMessageBox.Show("Lỗi không kết nối với máy chủ.", "Cảnh Báo");
                return null;
            }
            var data = JsonConvert.DeserializeObject<List<StatisticalDto>>(result.Result.ToString());
            return data;
        }

        /// <summary>
        /// Cập nhật lượt cân
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Number"></param>
        /// <param name="CorrectNumber"></param>
        /// <param name="CageId"></param>
        /// <param name="FileImage"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<List<DataDto>> UpdateData(int Id, int Number, int CorrectNumber, string CageId, string FileImage, int NumberLine)
        {
            var body = new UpdateDataDto()
            {
                Id = Id,
                Number = Number,
                CageId = CageId,
                FileImage = FileImage,
                CreateId = "",
                CorrectNumber = CorrectNumber,
                NumberLine = NumberLine
            };
            var result = await api.PostAsync("Data/UpdateData", body);
            if (result.StatusCode != 200)
            {
                XtraMessageBox.Show("Lỗi không kết nối với máy chủ.", "Cảnh Báo");
                return null;
            }
            var data = JsonConvert.DeserializeObject<List<DataDto>>(result.Result.ToString());
            return data;
        }

        [Obsolete]
        public async Task<List<StatusDto>> DeleteNumberTest(string CageId)
        {
            var body = new DeleteNumberTestDto()
            {
                CageId = CageId
            };
            var result = await api.PostAsync("Data/DeleteNumberTest", body);
            if (result.StatusCode != 200)
            {
                XtraMessageBox.Show("Lỗi không kết nối với máy chủ.", "Cảnh Báo");
                return null;
            }
            var data = JsonConvert.DeserializeObject<List<StatusDto>>(result.Result.ToString());
            return data;
        }
    }
}
