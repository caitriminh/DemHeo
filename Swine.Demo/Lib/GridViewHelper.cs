using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ColorPick.Picker;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Swine.Demo.Extention
{
    internal class GridViewHelper
    {
        public static string TEMP_PATH = Path.GetTempPath() + Assembly.GetExecutingAssembly().GetName().Name + "\\";
        public static void SaveAndRestoreLayout(GridControl gridControl, string FormName)
        {
            gridControl.ForceInitialize();
            var path_layout_default = TEMP_PATH + "\\" + FormName + "\\" + gridControl.Name + "\\" + "default_layout.xml";
            var folder = TEMP_PATH + "\\" + FormName + "\\" + gridControl.Name;
            if (!Directory.Exists(folder))
            {
                DirectoryInfo di = Directory.CreateDirectory(folder);
            }
            if (!File.Exists(path_layout_default))
            {
                gridControl.MainView.SaveLayoutToXml(path_layout_default, OptionsLayoutBase.FullLayout);
            }
            var path = TEMP_PATH + "\\" + FormName + "\\" + gridControl.Name + "\\custom_layout.xml";
            if (File.Exists(path))
            {
                gridControl.ForceInitialize();
                gridControl.MainView.RestoreLayoutFromXml(path, OptionsLayoutBase.FullLayout);
            }
        }

        public static void NotContainItem_Click(object sender, EventArgs e, GridView gridView)
        {
            string filter = gridView.ActiveFilterString;
            string value = (sender as DXMenuItem).Caption.Split('\'')[1];
            string newFilterString = "Not Contains([" + gridView.FocusedColumn.FieldName + "], '" + value + "')";

            if (filter == String.Empty)
                gridView.ActiveFilterString = newFilterString;
            else
                gridView.ActiveFilterString += "And " + newFilterString;
        }

        public static void ContainItem_Click(object sender, EventArgs e, GridView gridView)
        {
            string filter = gridView.ActiveFilterString;
            string value = (sender as DXMenuItem).Caption.Split('\'')[1];
            string newFilterString = "Contains([" + gridView.FocusedColumn.FieldName + "], '" + value + "')";

            if (filter == String.Empty)
                gridView.ActiveFilterString = newFilterString;
            else
                gridView.ActiveFilterString += "And " + newFilterString;
        }

        public static void GridView_ShownEditor(object sender, EventArgs e, GridView gridView)
        {
            DXMenuItem containItem;
            DXMenuItem notContainItem;
            containItem = new DXMenuItem();
            containItem.Click += (ss, ee) => { ContainItem_Click(ss, ee, gridView); };
            notContainItem = new DXMenuItem();
            notContainItem.Click += (ss, ee) => { NotContainItem_Click(ss, ee, gridView); };

            if (gridView.ActiveEditor is TextEdit)
            {
                var editor = gridView.ActiveEditor as TextEdit;
                editor.Properties.BeforeShowMenu += (ss, ee) => { Properties_BeforeShowMenu(ss, ee, containItem, notContainItem); };
            }
        }

        public static void ClearMenu(DXPopupMenu menu)
        {
            foreach (DXMenuItem item in menu.Items)
                if (item.Caption.Contains("Filter by"))
                    item.Visible = false;
        }

        public static bool initializeMenu = true;
        public static void Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e, DXMenuItem containItem, DXMenuItem notContainItem)
        {
            ClearMenu(e.Menu);
            string text = (sender as TextEdit).SelectedText;

            if (text != String.Empty)
            {
                containItem.Caption = "Filter by contains '" + text + "'";
                notContainItem.Caption = "Filter by not contains '" + text + "'";
                containItem.Visible = true;
                notContainItem.Visible = true;

                initializeMenu = false;
                e.Menu.Items.Add(containItem);
                e.Menu.Items.Add(notContainItem);

            }
        }

        public static void GridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e, GridControl gridControl, GridView gridView)
        {
            if (!gridView.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }
                    SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, e.Appearance.Font);
                    Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                    gridControl.BeginInvoke(new MethodInvoker(delegate { cal(_Width, gridView); }));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = string.Format("[{0}]", (e.RowHandle * -1));
                SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, e.Appearance.Font);
                Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                gridControl.BeginInvoke(new MethodInvoker(delegate { cal(_Width, gridView); }));
            }
        }

        public static bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        public static Image createImage(Color color)
        {
            Bitmap flag = new Bitmap(16, 16);
            Graphics flagGraphics = Graphics.FromImage(flag);
            Pen blackPen = new Pen(Color.Black, 2);
            Rectangle rect = new Rectangle(0, 0, 16, 16);
            flagGraphics.DrawRectangle(blackPen, rect);
            flagGraphics.FillRectangle(new SolidBrush(color), 1, 1, 14, 14);
            return flag;
        }

        [Obsolete]
        public static void AddFontAndColortoPopupMenuShowing(object sender, PopupMenuShowingEventArgs e, GridControl gridcontrol, GridView gridView, string FormName)
        {
            //nếu sử dụng thì tích hợp save layout.          
            if (e.MenuType == GridMenuType.Column)
            {
                GridViewColumnMenu menu = e.Menu as GridViewColumnMenu;
                //menu.Items.Clear();
                if (menu.Column != null)
                {



                    var fixLeft = new DXMenuCheckItem("Cố Định Cột Bên Trái");
                    //   fixLeft.ImageOptions.SvgImage = Properties.Resources.left_align;
                    menu.Items.Add(fixLeft);
                    fixLeft.Click += (ss, ee) =>
                    {
                        menu.Column.Fixed = FixedStyle.Left;
                    };

                    var fixRight = new DXMenuCheckItem("Cố Định Cột Bên Phải");
                    //  fixRight.ImageOptions.SvgImage = Properties.Resources.right_align;
                    menu.Items.Add(fixRight);
                    fixRight.Click += (ss, ee) =>
                    {
                        menu.Column.Fixed = FixedStyle.Right;
                    };

                    var unFix = new DXMenuCheckItem("Xóa Cố Định Cột");
                    //   unFix.ImageOptions.SvgImage = Properties.Resources.delete;
                    unFix.Enabled = menu.Column.Fixed != FixedStyle.None;
                    menu.Items.Add(unFix);
                    unFix.Click += (ss, ee) =>
                    {
                        menu.Column.Fixed = FixedStyle.None;
                    };

                    var menu_exportExcel = new DXMenuCheckItem("Xuất Excel");
                    //  menu_exportExcel.ImageOptions.SvgImage = Properties.Resources.excel1;

                    menu.Items.Add(menu_exportExcel);
                    menu_exportExcel.Click += (ss, ee) =>
                    {
                        var dlg = new SaveFileDialog { Filter = "Export Excel|*.xlsx" };
                        dlg.FileName = "data_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            gridcontrol.MainView.ExportToXlsx(dlg.FileName);
                            Process.Start(dlg.FileName);
                            //var frm = new FrmViewExcel(dlg.FileName);
                            //frm.Show();
                        }

                    };

                    // font chữ

                    DXMenuCheckItem font = new DXMenuCheckItem("Fonts", true, FrmMain.Instance.ribbonImageCollection.Images[1], new EventHandler(OnFixedClick));
                    font.Tag = new MenuInfo(menu.Column, FixedStyle.None);
                    menu.Items.Add(font);


                    // Màu nền
                    DXSubMenuItem sItem = new DXSubMenuItem("Màu Nền");

                    //   sItem.ImageOptions.SvgImage = Properties.Resources.background1;
                    Color mauhong = ColorTranslator.FromHtml("#FFC2BE");
                    Color mauxanh = ColorTranslator.FromHtml("#A8D5FF");
                    Color xanhduong = ColorTranslator.FromHtml("#C1F49C");
                    Color mauvang = ColorTranslator.FromHtml("#FEF7A5");
                    Color mautim = ColorTranslator.FromHtml("#E0CFE9");
                    Color xanhlam = ColorTranslator.FromHtml("#8DE9DF");
                    Color mautrang = ColorTranslator.FromHtml("#FFFFFF");
                    Color mauden = ColorTranslator.FromHtml("#000000");

                    sItem.Items.Add(CreateCheckItem("Color White", menu.Column, FixedStyle.None, mautrang));
                    sItem.Items.Add(CreateCheckItem("Color Black", menu.Column, FixedStyle.None, mauden));
                    sItem.Items.Add(CreateCheckItem("Color Pink", menu.Column, FixedStyle.None, mauhong));
                    sItem.Items.Add(CreateCheckItem("Color Blue", menu.Column, FixedStyle.None, mauxanh));
                    sItem.Items.Add(CreateCheckItem("Color Green", menu.Column, FixedStyle.None, xanhduong));
                    sItem.Items.Add(CreateCheckItem("Color Yellow", menu.Column, FixedStyle.None, mauvang));
                    sItem.Items.Add(CreateCheckItem("Color Purple", menu.Column, FixedStyle.None, mautim));
                    sItem.Items.Add(CreateCheckItem("Color Cyan", menu.Column, FixedStyle.None, xanhlam));

                    sItem.Items.Add(CreateCheckItem("Color Customize...", menu.Column, FixedStyle.None, Color.Transparent));
                    menu.Items.Add(sItem);

                    // màu chữ
                    var mauchu = new DXSubMenuItem("Màu Chữ");
                    //     mauchu.ImageOptions.SvgImage = Properties.Resources.font_color;
                    Color Red = Color.Red;
                    Color pink = ColorTranslator.FromHtml("#E91E63");
                    Color purple = ColorTranslator.FromHtml("#9C27B0");
                    Color DeepPurple = ColorTranslator.FromHtml("#673AB7");
                    Color Indigo = ColorTranslator.FromHtml("#E0CFE9");
                    Color blue = ColorTranslator.FromHtml("#3F51B5");
                    Color cyan = ColorTranslator.FromHtml("#00BCD4");
                    Color Teal = ColorTranslator.FromHtml("#009688");
                    Color green = ColorTranslator.FromHtml("#4CAF50");
                    Color Lime = ColorTranslator.FromHtml("#CDDC39");
                    Color Amber = ColorTranslator.FromHtml("#FFC107");
                    Color Orange = ColorTranslator.FromHtml("#FF9800");
                    Color depOrange = ColorTranslator.FromHtml("#FF5722");
                    Color brown = ColorTranslator.FromHtml("#795548");
                    Color grey = ColorTranslator.FromHtml("#9E9E9E");
                    Color BlueGrey = ColorTranslator.FromHtml("#607D8B");
                    Color Black = ColorTranslator.FromHtml("#000000");
                    Color White = ColorTranslator.FromHtml("#FFFFFF");

                    mauchu.Items.Add(CreateCheckItem("Black", menu.Column, FixedStyle.None, Black));
                    mauchu.Items.Add(CreateCheckItem("White", menu.Column, FixedStyle.None, White));
                    mauchu.Items.Add(CreateCheckItem("Pink", menu.Column, FixedStyle.None, pink));
                    mauchu.Items.Add(CreateCheckItem("Purple", menu.Column, FixedStyle.None, purple));
                    mauchu.Items.Add(CreateCheckItem("Deep Purple", menu.Column, FixedStyle.None, DeepPurple));
                    mauchu.Items.Add(CreateCheckItem("Indigo", menu.Column, FixedStyle.None, Indigo));
                    mauchu.Items.Add(CreateCheckItem("Blue", menu.Column, FixedStyle.None, blue));
                    mauchu.Items.Add(CreateCheckItem("Cyan", menu.Column, FixedStyle.None, cyan));
                    mauchu.Items.Add(CreateCheckItem("Teal", menu.Column, FixedStyle.None, Teal));
                    mauchu.Items.Add(CreateCheckItem("Green", menu.Column, FixedStyle.None, green));
                    mauchu.Items.Add(CreateCheckItem("Lime", menu.Column, FixedStyle.None, Lime));
                    mauchu.Items.Add(CreateCheckItem("Amber", menu.Column, FixedStyle.None, Amber));
                    mauchu.Items.Add(CreateCheckItem("Orange", menu.Column, FixedStyle.None, Orange));
                    mauchu.Items.Add(CreateCheckItem("Deep Orange", menu.Column, FixedStyle.None, depOrange));
                    mauchu.Items.Add(CreateCheckItem("Brown", menu.Column, FixedStyle.None, brown));
                    mauchu.Items.Add(CreateCheckItem("Grey", menu.Column, FixedStyle.None, grey));
                    mauchu.Items.Add(CreateCheckItem("Blue Grey", menu.Column, FixedStyle.None, BlueGrey));

                    mauchu.Items.Add(CreateCheckItem("ForeColor Customize...", menu.Column, FixedStyle.None, Color.Transparent));
                    menu.Items.Add(mauchu);

                    DXMenuCheckItem save_layout = new DXMenuCheckItem("Lưu Layout", true);
                    //    save_layout.ImageOptions.SvgImage = Properties.Resources.floppy_disk;
                    save_layout.CheckedChanged += (ss, ee) =>
                    {
                        var path = TEMP_PATH + "\\" + FormName + "\\" + gridcontrol.Name + "\\custom_layout.xml";
                        gridcontrol.MainView.SaveLayoutToXml(path, OptionsLayoutBase.FullLayout);
                        XtraMessageBox.Show("Đã lưu cấu hình layout", "Thông Báo");
                    };
                    DXMenuCheckItem reset_layout = new DXMenuCheckItem("Khôi Phục Layout", true);
                    reset_layout.CheckedChanged += (ss, ee) =>
                    {
                        var path = TEMP_PATH + "\\" + FormName + "\\" + gridcontrol.Name + "\\default_layout.xml";
                        var path_custom = TEMP_PATH + "\\" + FormName + "\\" + gridcontrol.Name + "\\custom_layout.xml";
                        if (File.Exists(path))
                        {
                            gridcontrol.MainView.RestoreLayoutFromXml(path, OptionsLayoutBase.FullLayout);
                            XtraMessageBox.Show("Khôi phục layout thành công.", "Thông Báo");
                        }

                        if (File.Exists(path_custom))
                        {
                            File.Delete(path_custom);
                        }

                    };
                    //   reset_layout.ImageOptions.SvgImage = Properties.Resources.undo;
                    menu.Items.Add(save_layout);
                    menu.Items.Add(reset_layout);
                }
            }
        }

        private static void GridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {

            e.Info.Caption = string.Empty;
            e.Painter.DrawObject(e.Info);

            e.Handled = true;
            // DrawEditorHelper.DrawColumnInplaceEditor(e, _Item, EditValue, GetRightIndent());
            //DrawEditorHelper.DrawColumnInplaceEditor(e, _Item, _Column.Caption, GetRightIndent());

        }

        //Create a menu item 
        public static DXMenuCheckItem CreateCheckItem(string caption, GridColumn column, FixedStyle style, Color color)
        {
            Image image = createImage(color);
            DXMenuCheckItem item = new DXMenuCheckItem(caption, column.Fixed == style, image, new EventHandler(OnFixedClick));
            item.Tag = new MenuInfo(column, style);
            return item;
        }

        //Menu item click handler 
        public static void OnFixedClick(object sender, EventArgs e)
        {
            DXMenuItem item = sender as DXMenuItem;
            MenuInfo info = item.Tag as MenuInfo;
            if (info == null) return;

            if (item.Caption.Substring(0, 3) == "Col")
            {
                if (item.Caption == "Color Customize...")
                {
                    ColorPickEdit colorPickerEdit = new ColorPickEdit();
                    FrmColorPicker frm = new FrmColorPicker(colorPickerEdit.Properties);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.TopMost = true;
                    if (frm.ShowDialog(colorPickerEdit.FindForm()) == DialogResult.OK)
                    {
                        info.Column.AppearanceCell.BackColor = frm.SelectedColor;
                    }

                }
                else
                {
                    info.Column.AppearanceCell.BackColor = ((Bitmap)item.Image).GetPixel(5, 5);
                }
            }
            else if (item.Caption.Substring(0, 4) == "Font")
            {
                FontDialog fontDialog = new FontDialog();
                fontDialog.ShowDialog();
                info.Column.AppearanceCell.Font = fontDialog.Font;
            }
            else
            {
                if (item.Caption == "ForeColor Customize...")
                {
                    ColorPickEdit colorPickerEdit = new ColorPickEdit();
                    FrmColorPicker frm = new FrmColorPicker(colorPickerEdit.Properties);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.TopMost = true;
                    if (frm.ShowDialog(colorPickerEdit.FindForm()) == System.Windows.Forms.DialogResult.OK)
                    {
                        info.Column.AppearanceCell.ForeColor = frm.SelectedColor;
                    }

                }
                else
                {
                    info.Column.AppearanceCell.ForeColor = ((Bitmap)item.Image).GetPixel(5, 5);
                }
            }


        }
        class MenuInfo
        {
            public MenuInfo(GridColumn column, FixedStyle style)
            {
                this.Column = column;
                this.Style = style;
            }
            public FixedStyle Style;
            public GridColumn Column;
        }

        //private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    gridView2.SaveLayoutToXml("data.xml", OptionsLayoutBase.FullLayout);
        //}
    }
}
