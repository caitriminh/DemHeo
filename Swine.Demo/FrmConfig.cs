using DevExpress.XtraEditors;
using Swine.Demo.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Swine.Demo
{
    public partial class FrmConfig : DevExpress.XtraEditors.XtraForm
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            //Load thông tin đã ghi nhớ
            txtCageId.Text = ConfigAppSetting.GetSetting("CageId");
            txtIPServer.Text = ConfigAppSetting.GetSetting("IP_SERVER");
            txtAPI.Text = ConfigAppSetting.GetSetting("URL_API");
            txtAPI_Backup.Text = ConfigAppSetting.GetSetting("URL_API_Backup");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var dgr = XtraMessageBox.Show("Bạn có muốn lưu lại thông tin cấu hình không?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dgr != DialogResult.Yes) { return; }
            ConfigAppSetting.SetSetting("CageId", txtCageId.Text);
            ConfigAppSetting.SetSetting("IP_SERVER", txtIPServer.Text);
            ConfigAppSetting.SetSetting("URL_API", txtAPI.Text);
            ConfigAppSetting.SetSetting("URL_API_Backup", txtAPI_Backup.Text);
        }
    }
}