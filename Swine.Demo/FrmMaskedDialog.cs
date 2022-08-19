using DevExpress.XtraEditors;
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
    public partial class FrmMaskedDialog : DevExpress.XtraEditors.XtraForm
    {
        public FrmMaskedDialog()
        {
            //InitializeComponent();
            SuspendLayout();
            ClientSize = new Size(783, 490);
            Name = "MaskedDialog";
            FormClosing += MaskedDialog_FormClosing;
            Load += MaskedDialog_Load;
            ResumeLayout(false);
        }

        private static FrmMaskedDialog _mask;
        private static Form _frmContainer;

        private Form _dialog;
        private UserControl _ucDialog;

        private FrmMaskedDialog(Form parent, Form dialog)
        {
            this._dialog = dialog;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            Opacity = 0.50;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Size = parent.ClientSize;
            Location = parent.PointToScreen(Point.Empty);
            parent.Move += AdjustPosition;
            parent.SizeChanged += AdjustPosition;
        }

        private FrmMaskedDialog(Form parent, UserControl ucDialog)
        {
            this._ucDialog = ucDialog;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            Opacity = 0.50;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Size = parent.ClientSize;
            Location = parent.PointToScreen(Point.Empty);
            parent.Move += AdjustPosition;
            parent.SizeChanged += AdjustPosition;
        }

        private void AdjustPosition(object sender, EventArgs e)
        {
            Form parent = sender as Form;
            Location = parent.PointToScreen(Point.Empty);
            ClientSize = parent.ClientSize;
        }


        public static DialogResult ShowDialog(Form parent, Form dialog)
        {
            _mask = new FrmMaskedDialog(parent, dialog);
            dialog.StartPosition = FormStartPosition.CenterParent;
            _mask.MdiParent = parent.MdiParent;
            _mask.BringToFront();
            _mask.Show(parent);
            DialogResult result = dialog.ShowDialog(_mask);
            //if (result == DialogResult.OK)
            //{
            _mask.Close();
            //}

            return result;
        }

        public static DialogResult ShowDialog(Form parent, UserControl dialog)
        {
            _mask = new FrmMaskedDialog(parent, dialog);
            _frmContainer = new Form();
            _frmContainer.ShowInTaskbar = false;
            _frmContainer.FormBorderStyle = FormBorderStyle.None;
            _frmContainer.StartPosition = FormStartPosition.CenterScreen;
            _frmContainer.Height = dialog.Height;
            _frmContainer.Width = dialog.Width;

            _frmContainer.Controls.Add(dialog);
            _mask.MdiParent = parent.MdiParent;
            _mask.Show();
            DialogResult result = _frmContainer.ShowDialog(_mask);
            _frmContainer.Close();
            _mask.Close();
            return result;
        }

        public static void CloseDialog()
        {
            if (_frmContainer != null)
            {
                _frmContainer.Close();
            }
        }

        private void MaskedDialog_Load(object sender, EventArgs e)
        {
        }

        private void MaskedDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}