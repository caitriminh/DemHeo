using DevExpress.XtraEditors;
using FastMember;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Swine.Demo.API;
using Swine.Demo.Extention;
using Swine.Demo.Lib;
using Swine.Demo.Properties;
using Swine.Demo.Services;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Swine.Demo
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        private ILoggingService _loggingService;
        DataServices objData;
        private short baseOutputAddress = 0xA02;
        private short baseInputAddress = 0xA03;
        private int _decimalDo = 0;
        private short _dataPort = 0x4F;
        private static FrmMain _defaultInstance;
        public static string _CageId = ConfigAppSetting.GetSetting("CageId");
        public static string strPath = ConfigAppSetting.GetSetting("Path");
        public static string strPathThumb = Application.StartupPath + "\\images\\";
        public string strFileStatus = "D:\\status.txt";
        [Obsolete]
        public FrmMain()
        {
            InitializeComponent();
            objData = new DataServices();
            _loggingService = new LoggingService();

            picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
            picLock2.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
            picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\stop.png");
            picImageNumberPig.Image = ReadImageToStream(Application.StartupPath + "\\images\\logo.jpg");
            dteFromDate.EditValue = DateTime.Now.Date;
            dteToDate.EditValue = DateTime.Now.Date;

            grvLuotCan.CustomDrawRowIndicator += (ss, ee) => { GridViewHelper.GridView_CustomDrawRowIndicator(ss, ee, grcLuotCan, grvLuotCan); };
            grvLuotCan.PopupMenuShowing += (s, e) => { GridViewHelper.AddFontAndColortoPopupMenuShowing(s, e, grcLuotCan, grvLuotCan, Name); };
            GridViewHelper.SaveAndRestoreLayout(grcLuotCan, Name);

            grvThongKeLuotCan.CustomDrawRowIndicator += (ss, ee) => { GridViewHelper.GridView_CustomDrawRowIndicator(ss, ee, grcThongKeLuotCan, grvThongKeLuotCan); };
            grvThongKeLuotCan.PopupMenuShowing += (s, e) => { GridViewHelper.AddFontAndColortoPopupMenuShowing(s, e, grcThongKeLuotCan, grvThongKeLuotCan, Name); };
            GridViewHelper.SaveAndRestoreLayout(grcThongKeLuotCan, Name);

            grvLuotCan.FocusedRowChanged += GrvLuotCan_FocusedRowChanged;
        }

        [Obsolete]
        public static FrmMain Instance
        {
            get
            {
                if (_defaultInstance == null || _defaultInstance.IsDisposed)
                {
                    _defaultInstance = new FrmMain();
                }
                return _defaultInstance;
            }
            set => _defaultInstance = value;
        }

        #region "Function"
        public Image ReadImageToStream(string url)
        {
            var arrbyte = File.ReadAllBytes(url);
            using (Stream stream = new MemoryStream(arrbyte))
            {
                Image image = Image.FromStream(stream);
                stream.Dispose();
                return image;
            }
        }

        private void SendDo(int value, bool isOn = false)
        {
            // 0000 0001 0010 0100 1000 - binary
            // 0 1, 2, 4, 8 -- hex
            // 0 1, 2, 4, 8 -- decimal 
            var index = 0;
            switch (value)
            {
                case 1:
                    index = 3;
                    break;
                case 2:
                    index = 2;
                    break;
                case 4:
                    index = 1;
                    break;
                case 8:
                    index = 0;
                    break;
                case 16:
                    index = 3;
                    break;
            }
            //EnterSIO();
            var doValue = CincozeService.Input(baseOutputAddress); // decimal value
            var doBinaryValue = Convert.ToString(doValue, 2).PadLeft(8, '0');

            doBinaryValue = doBinaryValue.Substring(4, 4);
            _decimalDo = Convert.ToInt32(doBinaryValue, 2);
            if (isOn == false)
            {
                if (doBinaryValue[index] == '1')
                    _decimalDo -= value;
            }
            else
            {
                if (doBinaryValue[index] == '0')
                    _decimalDo += value;
            }
            CincozeService.Output(baseOutputAddress, _decimalDo);
            lblStatus.Text = "doValue: " + doValue + " - doBinaryValue: " + doBinaryValue + " - _decimalDo: " + _decimalDo;
        }

        string flag = "";
        int j = 0;
        //DI 1: Value: 4
        //DI 2: Value: 8
        //DI 3: Value: 16
        //DI 4: Value: 32
        //DI 5: Value: 2
        //DI 6: Value: 1
        int intLuotCan = 0;
        private void GetDI()
        {
            var value = CincozeService.Input(baseInputAddress); // decimal value
            var binaryValue = Convert.ToString(value, 2).PadLeft(8, '0');
            var diBinaryValue = binaryValue.Substring(0, 6);
            _decimalDo = Convert.ToInt32(diBinaryValue, 2); // convert binary to decimal value

            textEdit2.Text = "J: " + j + "- DI: " + _decimalDo.ToString() + " - binaryValue: " + value.ToString() + " - diBinaryValue: " + diBinaryValue;
            if ((_decimalDo == 52 || _decimalDo == 20) && flag == "") //Bât DI 1
            {
                _loggingService.Debug($"B1: CHUẨN BỊ LƯỢT CÂN MỚI - DI: " + _decimalDo);
                //SendDo(16, true); //Mở khóa cho heo vào
                chkDO1_CheckedChanged(null, null);
                SendDo(8, true); //Bật đèn đỏ

                //Đổi ảnh
                try
                {
                    picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                    picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\red.png");
                }
                catch (Exception)
                {

                }

                lblStatus.Text = "CHUẨN BỊ LƯỢT CÂN MỚI. CHO HEO VÀO CÂN";
                flag = "DI1";
                textEdit1.Text = "DI: " + _decimalDo + " - Flag: " + flag;
                return;
            }
            if (_decimalDo == 56) //Nhận DI4=32 + DI2=8 = 40, Xác nhận 02 trạng thái công tác hành trình và nút nhấn DI2
            {
                _loggingService.Debug($"B2: Đang chuẩn bị cân - DI: " + _decimalDo);
                textEdit1.Text = "DI: " + _decimalDo + " - Flag: " + flag;
                //if (flag != "DI1") { return; }
                SendDo(16, false); //Khóa cửa heo vào
                SendDo(8, false); //tắt đèn đỏ
                SendDo(4, true); //bật đèn xanh
                try
                {
                    picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
                    picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\green.png");
                }
                catch (Exception)
                {

                }

                lblStatus.Text = "ĐANG CHUẨN BỊ CÂN";
                //Gủi trạng thái cho VSTech tiến hành lấy ảnh
                var wc = new WebClient();
                wc.DownloadString(ApiHelper.url + "api/SignalR/GetNumber?CageId=" + _CageId);
                return;
            }
            if (_decimalDo == 60) //nút kết hợp
            {
                SendDo(16, true); //Mở khóa cho heo vào
                SendDo(8, true); //Bật đèn đỏ
                //Đổi ảnh
                try
                {
                    picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                    picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\red.png");
                }
                catch (Exception)
                {

                }
                return;
            }
            if (_decimalDo == 48) //Nhận DI4=32 + DI2=8 = 40, Xác nhận 02 trạng thái công tác hành trình và nút nhấn DI2
            {
                if (flag == "CANXONG")
                {
                    SendDo(2, true); //khóa cửa heo ra
                    try
                    {
                        picLock2.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                    }
                    catch (Exception)
                    {

                    }
                    return;
                }

                if (flag == "CANXONG2")
                {
                    SendDo(2, false); //khóa cửa heo ra
                    SendDo(8, true); //bật đèn bỏ    
                    SendDo(16, true); //mở khóa cho heo vào
                    try
                    {
                        picLock2.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
                        picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\red.png");
                        picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                    }
                    catch (Exception)
                    {

                    }
                    lblStatus.Text = "CHUẨN BỊ LƯỢT CÂN MỚI";
                    flag = "";
                    return;
                }

                textEdit1.Text = "DI: " + _decimalDo + " - Flag: " + flag;
                return;
            }

            if (_decimalDo == 32) //Bật DI4, Dập khóa DI ngậm điện ghi nhận trạng thái khóa đã đóng
            {
                if (flag == "CANXONG")
                {
                    flag = "CANXONG2";
                }
                textEdit1.Text = "DI: " + _decimalDo + " - Flag: " + flag;
                return;
            }
            else if ((_decimalDo == 50 || _decimalDo == 34 || _decimalDo == 18 || _decimalDo == 2 || _decimalDo == 32) && (flag == "" || flag == "KHANCAP")) //Bật DI 5, nút nhấn khẩn cấp
            {
                _loggingService.Debug($"Nút nhấn khẩn cấp - DI: " + _decimalDo);
                SendDo(16, true);
                SendDo(2, true);
                try
                {
                    picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                    picLock2.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                }
                catch (Exception)
                {

                }

                flag = "KHANCAP";
                textEdit1.Text = "DI: " + _decimalDo + " - Flag: " + flag;
                //ResetDO();
                return;
            }
            else if (_decimalDo == 16 && flag == "KHANCAP")
            {
                SendDo(2, true); //khóa cửa heo ra
                SendDo(8, true); //bật đèn bỏ     
                SendDo(16, true); //mở khóa cho heo vào
                try
                {
                    picLock2.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
                    picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\red.png");
                    picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                }
                catch (Exception)
                {

                }

                lblStatus.Text = "KHAN CAP.";
                flag = "";
                ResetDO();
                return;
            }
        }

        [Obsolete]
        public async Task GetData()
        {
            var x = grvLuotCan.FocusedRowHandle;
            var y = grvLuotCan.TopRowIndex;
            var data = chkHeoNai.Checked == true ? await objData.GetData_BoQuaHeoNai(Convert.ToDateTime(dteFromDate.EditValue), Convert.ToDateTime(dteToDate.EditValue)) : await objData.GetData(Convert.ToDateTime(dteFromDate.EditValue), Convert.ToDateTime(dteToDate.EditValue));
            var dt = new DataTable();
            using (var reader = ObjectReader.Create(data)) { dt.Load(reader); }
            grcLuotCan.DataSource = dt;
            grvLuotCan.FocusedRowHandle = x;
            grvLuotCan.TopRowIndex = y;
        }

        [Obsolete]
        public async Task GetThongKe()
        {
            var x = grvThongKeLuotCan.FocusedRowHandle;
            var y = grvThongKeLuotCan.TopRowIndex;
            var data = await objData.GetThongKe(chkHeoNai.Checked ? "GET_DATA_BOHEONAI" : "GET_DATA", Convert.ToDateTime(dteFromDate.EditValue), Convert.ToDateTime(dteToDate.EditValue));
            grcThongKeLuotCan.DataSource = data;
            grvThongKeLuotCan.FocusedRowHandle = x;
            grvThongKeLuotCan.TopRowIndex = y;
        }

        #endregion

        #region "SignalR"
        private string _titleForm = "";
        HubConnection _connection;
        private string GetLanIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        string GetOSName()
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        }

        [Obsolete]
        private void InstanceConnectSignal()
        {
            _titleForm = this.Text;
            if (!string.IsNullOrEmpty(ApiHelper.url))
            {
                var deviceId = Guid.NewGuid().ToString();
                _connection = new HubConnectionBuilder()
                    .WithUrl($"{ApiHelper.url}applicationHub", options =>
                    {
                        options.Headers.Add("ip", GetLanIp());
                        options.Headers.Add("deviceId", deviceId);
                        options.Headers.Add("os", GetOSName());
                    })
                    .Build();

                _connection.Closed += async (error) =>
                {
                    while (_connection.State != HubConnectionState.Connected)
                    {
                        await Task.Delay(3000);
                        ThreadPool.QueueUserWorkItem(async state =>
                        {
                            try
                            {
                                await _connection.StartAsync();
                            }
                            catch (Exception)
                            {
                                //_loggingService.Log($"InstanceConnectSignal: {e}");
                            }
                        });
                    }
                };
                _connection.Reconnecting += async (error) =>
                {
                    await Task.Delay(3000);
                    if (_connection.State != HubConnectionState.Connected)
                        await _connection.StartAsync();
                    else
                    {
                        barConnect.Caption = $@"{_titleForm} - Connected";
                        barConnect.ImageOptions.Image = Properties.Resources.calendar;
                        //Get IP Internet
                        //IP_Internet = new WebClient().DownloadString("https://ipv4.icanhazip.com/").TrimEnd();
                    }
                };
                _connection.Reconnected += async (error) =>
                {
                    await Task.Delay(3000);
                    if (_connection.State != HubConnectionState.Connected)
                        await _connection.StartAsync();
                    else
                    {
                        barConnect.Caption = $@"{_titleForm} - Connected";
                        barConnect.ImageOptions.Image = Properties.Resources.calendar;
                        //Get IP Internet
                        // IP_Internet = new WebClient().DownloadString("https://ipv4.icanhazip.com/").TrimEnd();
                    }
                };
            }

            InitSignalR();
        }

        [Obsolete]
        private async void InitSignalR()
        {
            _connection.On("CheckAutoUpdate", () =>
            {
                // CheckVersionApp();
            });

            _connection.On("UpdateNow", () =>
            {
                //FlagUpdateAppNow = true;
                //CheckVersionApp();
            });


            _connection.On("FinishWeight", (string CageId) =>
            {
                if (CageId == _CageId)
                {
                    if (ApiHelper.app_test == "1")
                    {
                        //Thread.Sleep(ApiHelper.delaytime); //Đợi 10s sau đó mới mở lồng. Chỉ áp dụng khi test
                        Thread.Sleep(10);
                    }
                    _loggingService.Debug($"Đã hoàn thành cân: Mở khóa cho heo ra khỏi chuồng.");
                    SendDo(4, false);//tat den xanh                   
                    SendDo(2, true);//mo khoa cho heo di ra
                    try
                    {
                        picLightRedGreen.EditValue = Resources.stop_svgrepo_com;
                        picLock2.EditValue = Resources.lock_svgrepo_com__2_;
                    }
                    catch (Exception)
                    {

                    }

                    flag = "CANXONG";
                    textEdit1.Text = flag;
                    lblStatus.Text = "ĐẴ CÂN XONG. CHO HEO ĐI RA CHUỒNG CÂN";
                    //Reset các đối tưỡng
                    txtResult.Text = "0";
                    intLuotCan += 1;
                    txtLuotCan.Text = intLuotCan.ToString();
                    //picImageNumberPig.Image = ReadImageToStream(Application.StartupPath + "\\images\\logo.jpg");
                }
            });

            _connection.On("GetNumber", (string CageId) =>
            {
                if (CageId == _CageId)
                {
                    _loggingService.Debug($"Đã nhận yêu cầu lấy số lượng từ lồng cân...");
                    picImageNumberPig.Image = ReadImageToStream(Application.StartupPath + "\\images\\logo.jpg");
                    txtResult.Text = "Đang xử lý...";
                    try
                    {
                        picImageNumberPig.Image = ReadImageToStream(Application.StartupPath + "\\images\\logo.jpg");
                    }
                    catch (Exception)
                    {

                    }
                    _loggingService.Debug($"Tạo trigger cho hệ thống VSTech tiến hành xử lý.");
                    WriterFile.Status();
                    //Đợi VSTech Xử lý
                    timer1.Enabled = true;
                }
            });

            _connection.On("CancelWeight", (string CageId) =>
            {
                if (CageId == _CageId)
                {
                    _loggingService.Debug($"Hủy lượt cân: Mở cửa lồng heo -> lùa heo ra cân lại.");
                    SendDo(16, true); //Mở khóa cho heo vào
                    SendDo(8, true); //Bật đèn đỏ
                    //Đổi ảnh
                    try
                    {
                        picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\unlock.png");
                        picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\red.png");
                    }
                    catch (Exception)
                    {

                    }
                }
            });

            try
            {
                await _connection.StartAsync();
                if (_connection.State == HubConnectionState.Connected)
                {
                    barConnect.Caption = $@"{_titleForm} - Connected";
                    barConnect.ImageOptions.Image = Resources.calendar;
                }

            }
            catch (Exception)
            {
                _loggingService.Log($"InitSignalR: ERROR");
            }

        }

        #endregion

        #region "Event"

        [Obsolete]
        private void Form1_Load(object sender, EventArgs e)
        {
            InstanceConnectSignal();
            GetDI();
            var timer = new System.Timers.Timer(500);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        /// <summary>
        /// Show thông tin chi tiết lượt cân
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrvLuotCan_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var i = grvLuotCan.FocusedRowHandle;
            if (i < 0) { return; }
            txtCageId.Text = grvLuotCan.GetRowCellValue(i, "CageId").ToString();
            txtNumber.EditValue = grvLuotCan.GetRowCellValue(i, "Number").ToString();
            txtDate.Text = grvLuotCan.GetRowCellValue(i, "CreateDate").ToString();
            txtNumberLine.EditValue = grvLuotCan.GetRowCellValue(i, "NumberLine").ToString();
            try
            {
                //Read image server
                string file = ApiHelper.url + $"uploads\\images\\" + Convert.ToDateTime(grvLuotCan.GetRowCellValue(i, "CreateDate")).ToString("yyyyMMdd") + "\\" + grvLuotCan.GetRowCellValue(i, "FileImage");
                pictureEdit2.LoadAsync(file);
            }
            catch (Exception)
            {
                //load hình lỗi
                pictureEdit2.Image = ReadImageToStream(Application.StartupPath + "\\images\\logo.jpg");
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetDI();
        }

        private void btnResetDO_Click(object sender, EventArgs e)
        {
            ResetDO();
        }

        /// <summary>
        /// Reset các D.O
        /// </summary>
        public void ResetDO()
        {
            _decimalDo = int.Parse("0", System.Globalization.NumberStyles.HexNumber);
            CincozeService.Output(baseOutputAddress, _decimalDo);
            Thread.Sleep(1);
            PrintDi();
            flag = "";
            try
            {
                picLightRedGreen.Image = ReadImageToStream(Application.StartupPath + "\\images\\stop.png");
                picLock1.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
                picLock2.Image = ReadImageToStream(Application.StartupPath + "\\images\\lock.png");
            }
            catch (Exception)
            {

            }
        }


        [Obsolete]
        public async void GetLoadData()
        {
            if (xtraTabControl1.SelectedTabPage == tabChiTietLuotCan)
            {
                _loggingService.Debug($"GetData: Xem nhật ký lượt cân từ ngày " + Convert.ToDateTime(dteFromDate.EditValue).ToString("dd-MM-yyyy") + " - "
                    + Convert.ToDateTime(dteToDate.EditValue).ToString("dd-MM-yyyy"));
                await GetData();
            }
            else if (xtraTabControl1.SelectedTabPage == tabThongKeLuotCan)
            {
                _loggingService.Debug($"GetData: Xem thống kê lượt cân từ ngày " + Convert.ToDateTime(dteFromDate.EditValue).ToString("dd-MM-yyyy") + " - "
                   + Convert.ToDateTime(dteToDate.EditValue).ToString("dd-MM-yyyy"));
                await GetThongKe();
            }
        }

        /// <summary>
        /// Tìm kiếm lịch sử theo ngày
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void btnTim_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetLoadData();
        }

        [Obsolete]
        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            GetLoadData();
        }

        [Obsolete]
        private void barGetNumber_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var wc = new WebClient();
            wc.DownloadString(ApiHelper.url + "api/SignalR/GetNumber?CageId=" + _CageId);
            //Lấy số lượng heo tại lồng heo
            _loggingService.Debug(ApiHelper.url + "api/SignalR/GetNumber?CageId=" + _CageId);
        }

        private void barWeightFinish_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Hoàn thành cân
            var wc = new WebClient();
            wc.DownloadString(ApiHelper.url + "Data/FinishWeight?status=finish&CageId=" + _CageId);
        }

        [Obsolete]
        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetLoadData();
        }

        #endregion

        #region "Test Điều Khiển DO"
        private void PrintDi()
        {
            var doValue = CincozeService.Input(_dataPort);
            var doBinaryValue = Convert.ToString(doValue, 2).PadLeft(8, '0'); // binary value
            doBinaryValue = doBinaryValue.Substring(4, 4);
        }

        private void chkDO1_CheckedChanged(object sender, EventArgs e)
        {
            SendDo(16, chkDO1.Checked);
        }

        private void chkDO2_CheckedChanged(object sender, EventArgs e)
        {
            SendDo(2, chkDO2.Checked);
        }

        private void chkDO3_CheckedChanged(object sender, EventArgs e)
        {
            SendDo(4, chkDO3.Checked);
        }

        private void chkDO4_CheckedChanged(object sender, EventArgs e)
        {
            SendDo(8, chkDO4.Checked);
        }


        #endregion

        #region "Xử lý ảnh"
        public static Image CreateThumbnailImageByOriginImage(string url, int width = 600, int height = 338)
        {
            Image image = Image.FromFile(url);
            Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
            image.Dispose();
            return thumb;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        #endregion

        int time = 0;
        [Obsolete]
        private async void timer1_Tick(object sender, EventArgs e)
        {
            //Kiểm tra VSTech xử lý xong chưa?          
            if (File.Exists(strFileStatus) == false)
            {
                _loggingService.Debug($"Đọc kết quả từ VSTech.");
                Thread.Sleep(1000);//delay 1s sau khi VSTech trả kết quả
                //Đọc kết quả
                var dt = WriterFile.ReadExcel();
                if (dt.Rows.Count == 0)
                {
                    return;
                }
                int i = dt.Rows.Count;
                int intResult = 0;
                try
                {
                    intResult = Convert.ToInt32(dt.Rows[i - 1]["Number"]); //lấy kết quả cuối cùng
                }
                catch (Exception)
                {
                    intResult = 0;
                }
                int intResultNumberLine = 0;
                try
                {
                    intResultNumberLine = Convert.ToInt32(dt.Rows[i - 1]["NumberLine"]); //lấy kết quả cuối cùng number line
                }
                catch (Exception)
                {
                    intResultNumberLine = 0;
                }

                string filename = dt.Rows[i - 1]["FileImage"].ToString();
                string[] words = filename.Split('/');
                var folder_date = words[0];
                var filename_temp = words[1];
                txtResult.Text = intResult.ToString();
                //Đọc hình ảnh từ VSTech trả về
                picImageNumberPig.Image = ReadImageToStream(strPath + "\\" + folder_date + "\\" + filename_temp);


                //Xử lý hình ành
                _loggingService.Debug($"Xử lý hình ảnh của VSTech -> Save lưu trữ");
                string path_image_of_vstech = "D:\\ImageDemo\\" + folder_date + "\\" + filename_temp;//File ảnh của VSTech trả về

                //Nén ảnh VSTech lưu trữ và upload lên server
                var img = CreateThumbnailImageByOriginImage(path_image_of_vstech);
                Encoder myEncoder = Encoder.Quality;
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 80L);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = myEncoderParameter;

                //Tạo folder
                var pathDir = $"{Directory.GetCurrentDirectory()}/images/{folder_date}"; // slipt image for daily
                if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);

                string ten_file_save_data = "SL_" + intResult + "_" + filename_temp;//Đặt lại tên file 
                img.Save(pathDir + "\\" + ten_file_save_data, jpgEncoder, myEncoderParameters); //Lưu ảnh tại folder máy local

                _loggingService.Debug($"Lưu ảnh hoàn tất");

                //upload hình ảnh lên server
                try
                {
                    _loggingService.Debug($"Bắt đầu upload ảnh lên server");
                    //Lưu ảnh tại máy chủ
                    var client = new WebClient();
                    client.Headers.Add("Content-Type", "binary/octet-stream");
                    client.UploadFileAsync(new Uri(ApiHelper.api_upload()), pathDir + "\\" + ten_file_save_data);

                    _loggingService.Debug($"Upload ảnh lên server thành công");
                }
                catch (Exception)
                {
                    _loggingService.Debug("Lỗi upload ảnh: " + e.ToString());
                }


                //Lưu kết quả xuống Database
                _loggingService.Debug($"Lưu kết quả xuống Database.");
                await objData.UpdateData(0, intResult, 0, _CageId, ten_file_save_data, intResultNumberLine);


                //Hoàn thành cân, C.P thực hiện, bước này chỉ áp dụng demo
                if (ApiHelper.app_test == "1")
                {
                    _loggingService.Debug($"Gủi yêu cầu hoàn thành lượt cân ở lồng cân " + _CageId);
                    var wc = new WebClient();
                    wc.DownloadString(ApiHelper.url + "Data/FinishWeight?status=finish&CageId=" + _CageId);
                }
                timer1.Enabled = false;
                return;
            }
            else //Phần mềm của VSTech bị treo / không hoạt động
            {
                time += 1;
                if (time == 10) //sau 8s tiến hành xử lý mở cửa lồng heo
                {
                    _loggingService.Debug($"Phần mềm nhận diện VS bị lỗi. Hệ thống tiến hành tự động xử lý -> Không trả kết quả");
                    try
                    {
                        File.Delete("D:\\status.txt");
                    }
                    catch (Exception)
                    {

                    }
                    time = 0;
                }
            }
        }

        [Obsolete]
        private async void chkHeoNai_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            await GetData();
        }

        /// <summary>
        /// Lưu lại số lượng cân đúng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private async void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dgr = XtraMessageBox.Show("Bạn có muốn lưu không?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dgr != DialogResult.Yes) { return; }
            await Update();
        }

        /// <summary>
        /// Update lại nhưng trường hợp đếm sai
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public new async Task Update()
        {
            txtTemp.Focus();
            int dem = 0;
            for (var i = 0; i <= grvLuotCan.RowCount; i++)
            {
                var dr = grvLuotCan.GetDataRow(Convert.ToInt32(i));
                if (dr is null)
                {
                    break;
                }
                switch (dr.RowState)
                {
                    case DataRowState.Modified:
                        if (dr["WrongNumber"] == DBNull.Value)
                        {
                            XtraMessageBox.Show("Bạn vui lòng nhập số lượng sai.", "Cảnh Báo");
                            return;
                        }
                        await objData.UpdateData(Convert.ToInt32(dr["Id"]), 0, Convert.ToInt32(dr["CorrectNumber"]), "", "", 0);
                        dem += 1;
                        break;
                }
            }
            if (dem == 0) //Không có thay đỗi trên lưới
            {
                XtraMessageBox.Show("Không có thông tin cần thay đổi.", "Cảnh Báo");
            }
            await GetData();
        }


        private void chkDieuKhien_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (chkDieuKhien.Checked)
            {
                tabDieuKhien.PageVisible = true;
                barGetNumber.Enabled = true;
                barWeightFinish.Enabled = true;
            }
            else
            {
                tabDieuKhien.PageVisible = false;
                barGetNumber.Enabled = false;
                barWeightFinish.Enabled = false;
            }
        }


        private void barConfig_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (FrmConfig frm = new FrmConfig())
            {
                if (FrmMaskedDialog.ShowDialog(this, frm) == DialogResult.OK)
                {

                }
            }
        }

        /// <summary>
        /// Hủy lượt cân
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var wc = new WebClient();
            wc.DownloadString(ApiHelper.url + "Data/CancelWeight?CageId=" + _CageId);
        }

        [Obsolete]
        private async void btnXoaCacLuotTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dgr = XtraMessageBox.Show("Bạn có muốn xóa các lượt cân test không?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dgr != DialogResult.Yes) { return; }
            var data = await objData.DeleteNumberTest(_CageId);
            if (data.Count > 0)
            {
                await GetData();
                XtraMessageBox.Show(data[0].message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
    }
}
