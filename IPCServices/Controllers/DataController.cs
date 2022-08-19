using Domain.Domain.Model;
using Domain.Model.Domain.Model;
using IPC.SignalR;
using IPCServices.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPCServices.Controllers
{
    [Produces("application/json")]
    //[Route("[controller]/[Action]")]
    [Route("[controller]")]
    public class DataController : Controller
    {
        private IDataRepository _dataRepository;
        private readonly IHubContext<ApplicationHub> _hub;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DataController(IDataRepository dataRepository, IWebHostEnvironment webHostEnvironment, IHubContext<ApplicationHub> hub)
        {
            _dataRepository = dataRepository;
            _webHostEnvironment = webHostEnvironment;
            this._hub = hub;
        }
        #region "Chức năng dành cho C.P"
        /// <summary>
        /// Đếm số lượng heo
        /// </summary>
        /// <param name="CageId"></param>
        /// <returns></returns>
        [HttpGet("GetNumber")]
        public IActionResult GetNumber(string CageId)
        {
            var model = new GetNumberDto();
            model.CageId = CageId;
            var result = new CustomJsonResult();
            string item;
            var kq = _dataRepository.GetData(model);
            if (kq.Count > 0)
            {
                result.Message = "Thành Công";
                result.StatusCode = 200;
                result.Result = kq;
                item = kq[0].Number.ToString();
            }
            else
            {
                result.Message = "Lỗi Cập Nhật!!";
                result.StatusCode = 200;
                result.Result = kq;
                item = "0-false";
            }
            return Content(item.ToString());
        }


        /// <summary>
        /// Hoàn thành lượt cân -> mở cửa lồng heo
        /// </summary>
        /// <param name="status"></param>
        /// <param name="CageId"></param>
        /// <returns></returns>
        [HttpGet("FinishWeight")]
        public IActionResult FinishWeight(string status, string CageId)
        {
            string result;
            if (status.ToLower() == "finish" && CageId.Length > 0)
            {
                //Gủi SignalR xử lý mở lồng cân
                _hub.Clients.All.SendAsync("FinishWeight", CageId);
                //Cập nhật lại trạng thái dữ liệu IsProcess=1, Đã xử lý xong
                var kq = _dataRepository.FinishWeight(CageId);
                result = "True";
            }
            else
            {
                result = "True";
            }
            return Content(result.ToLower());
        }

        /// <summary>
        /// Hủy lượt cân
        /// </summary>
        /// <param name="CageId"></param>
        /// <returns></returns>
        [HttpGet("CancelWeight")]
        public IActionResult CancelWeight(string CageId)
        {
            //string result;
            if (CageId.Length > 0)
            {
                //Gủi SignalR xử lý mở lồng cân
                _hub.Clients.All.SendAsync("CancelWeight", CageId);
                //Cập nhật lại trạng thái dữ liệu IsProcess=1, Đã xử lý xong
                var kq = _dataRepository.FinishWeight(CageId);
                //result = "True";
            }
            else
            {
                //  result = "False";
            }
            return Ok();
        }

        #endregion

        #region "Xứ lý và thống kê dữ liệu"
        [HttpGet("GetData")]
        public ActionResult GetData([FromQuery] string CageId)
        {
            var model = new GetNumberDto();
            model.CageId = CageId;
            var result = new CustomJsonResult();

            var kq = _dataRepository.GetData(model);
            string item;
            if (kq != null && kq.Count > 0)
            {
                result.Message = "Thành Công";
                result.StatusCode = 200;
                result.Result = kq;
                item = kq[0].Number.ToString();
            }
            else
            {
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 200;
                result.Result = kq;
                item = "-1";
            }
            // return Ok(result);
            return Content(item.ToString());
        }

        [HttpPost("GetThongKe")]
        [Produces(typeof(CustomJsonResult))]
        public ActionResult GetThongKe([FromBody] GetStatisticalDto model)
        {
            var result = new CustomJsonResult();
            var kq = _dataRepository.GetThongKe(model);
            if (kq.Count > 0)
            {
                result.Message = "Thành Công";
                result.StatusCode = 200;
                result.Result = kq;

            }
            else
            {
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 200;
                result.Result = kq;
            }
            return Ok(result);
        }


        [HttpPost("GetDataAll")]
        [Produces(typeof(CustomJsonResult))]
        public ActionResult GetDataAll([FromBody] GetDataDto model)
        {
            var result = new CustomJsonResult();
            var kq = _dataRepository.GetDataAll(model);
            if (kq != null)
            {
                result.Message = "Thành Công";
                result.StatusCode = 200;
                result.Result = kq;
            }
            else
            {
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 200;
                result.Result = kq;
            }
            return Ok(result);
        }


        [HttpPost("UpdateData")]
        [Produces(typeof(CustomJsonResult))]
        public ActionResult UpdateData([FromBody] UpdateDataDto model)
        {
            var result = new CustomJsonResult();
            var kq = _dataRepository.Update(model);
            if (kq != null)
            {
                result.Message = "Thành Công";
                result.StatusCode = 200;
                result.Result = kq;

            }
            else
            {
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 200;
                result.Result = kq;
            }
            return Ok(result);
        }


        [HttpPost("DeleteNumberTest")]
        [Produces(typeof(CustomJsonResult))]
        public ActionResult DeleteNumberTest([FromBody] DeleteNumberTestDto model)
        {
            var result = new CustomJsonResult();
            var kq = _dataRepository.DeleteNumberTest(model);
            if (kq != null)
            {
                result.Message = "Thành Công";
                result.StatusCode = 200;
                result.Result = kq;

            }
            else
            {
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 200;
                result.Result = kq;
            }
            return Ok(result);
        }

        #endregion

        #region "Upload Image"
        public class ResponseData
        {
            public string status { get; set; }
            public string message { get; set; }
            public string data { get; set; }
        }
        public class FileUploadInfo
        {
            public string filename { get; set; }
            public long filesize { get; set; }
            public string prettyFileSize
            {
                get
                {
                    return BytesToReadableValue(this.filesize);
                }
            }

            public string BytesToReadableValue(long number)
            {
                var suffixes = new List<string> { " B", " KB", " MB", " GB", " TB", " PB" };

                for (int i = 0; i < suffixes.Count; i++)
                {
                    long temp = number / (int)Math.Pow(1024, i + 1);

                    if (temp == 0)
                    {
                        return (number / (int)Math.Pow(1024, i)) + suffixes[i];
                    }
                }

                return number.ToString();
            }
        }

        [HttpPost("UploadFileAsync")]
        public async Task<IActionResult> UploadFileAsync(List<IFormFile> file)
        {
            const bool AllowLimitSize = true;

            var limitFileSize = 2097152; // allow upload file less 2MB = 2097152
            var listFileError = new List<FileUploadInfo>();
            var responseData = new ResponseData();
            string result = "";

            if (file.Count <= 0)
            {
                responseData.status = "ERROR";
                responseData.message = $"Please, select file to upload.";
                result = JsonConvert.SerializeObject(responseData);
                return Ok(result);
            }

            var listFileTypeAllow = "jpg|png|gif|jpeg";
            {
                foreach (var formFile in file)
                {
                    var file_ext = Path.GetExtension(formFile.FileName).Replace(".", "");
                    var isAllow = listFileTypeAllow.Split('|').Any(x => x.ToLower() == file_ext.ToLower());
                    if (!isAllow)
                    {
                        listFileError.Add(new FileUploadInfo()
                        {
                            filename = formFile.FileName,
                            filesize = formFile.Length
                        });
                    }

                }
            }


            if (listFileError.Count > 0)
            {
                responseData.status = "ERROR";
                responseData.data = JsonConvert.SerializeObject(listFileError);
                responseData.message = $"File type upload only Allow Type: ({listFileTypeAllow}) \r\n {responseData.data}";
                result = JsonConvert.SerializeObject(responseData);
                return Ok(result);
            }


            // check list file less limit size
            if (AllowLimitSize)
            {
                foreach (var formFile in file)
                {
                    if (formFile.Length > limitFileSize)
                    {
                        listFileError.Add(new FileUploadInfo()
                        {
                            filename = formFile.FileName,
                            filesize = formFile.Length
                        });
                    }
                }
            }


            var listLinkUploaded = new List<string>();

            if (listFileError.Count > 0)
            {
                responseData.status = "ERROR";
                responseData.data = JsonConvert.SerializeObject(listFileError);
                responseData.message = $"File upload must less 2MB ({responseData.data})";
                result = JsonConvert.SerializeObject(responseData);
                return Ok(result);
            }

            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    var templateUrl = formFile.FileName;
                    string strPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/uploads/images/{DateTime.Today.ToString("yyyyMMdd")}/");
                    if (!Directory.Exists(strPath)) //Kiểm tra đường dẫn 
                        Directory.CreateDirectory(strPath);
                    string filePath = strPath + templateUrl;

                    string fileName = Path.GetFileName(filePath);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    listLinkUploaded.Add($"{baseUrl}/uploads/images/{formFile.FileName}");
                }
            }

            responseData.status = "SUCCESS";
            responseData.data = JsonConvert.SerializeObject(listLinkUploaded);
            responseData.message = $"uploaded {file.Count} files successful.";
            result = JsonConvert.SerializeObject(responseData);

            return Ok(result);
        }

        #endregion

    }
}

