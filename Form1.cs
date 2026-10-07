using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using PnpUtilGui.Models;
using PnpUtilGui.Properties;
using PnpUtilGui.Utils;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;

namespace PnpUtilGui
{
    public partial class Form1 : MaterialForm
    {
        private readonly BindingList<Driver> _bindingSource = new BindingList<Driver>();
        private Font _titleFont;

        // Color schemes: the light theme (classic indigo) and the Windows dark
        // mode variant (blue-grey), applied automatically by OsThemeWatcher.
        private readonly MaterialColorScheme _lightScheme = new MaterialColorScheme(
            MaterialPrimary.Indigo500,
            MaterialPrimary.Indigo700,
            MaterialPrimary.Indigo100,
            MaterialAccent.Pink200,
            MaterialTextShade.WHITE);

        private readonly MaterialColorScheme _darkScheme = new MaterialColorScheme(
            MaterialPrimary.BlueGrey800,
            MaterialPrimary.BlueGrey900,
            MaterialPrimary.BlueGrey700,
            MaterialAccent.Pink200,
            MaterialTextShade.WHITE);

        // Original grid colors so the light look can be fully restored when
        // Windows switches back to light mode.
        private Color _gridOriginalBackground;
        private Color _gridOriginalGridColor;
        private Color _gridOriginalCellBack;
        private Color _gridOriginalCellFore;
        private Color _gridOriginalCellSelBack;
        private Color _gridOriginalCellSelFore;
        private Color _gridOriginalHeaderBack;
        private Color _gridOriginalHeaderFore;
        private Color _gridOriginalRowHeaderBack;
        private Color _gridOriginalRowHeaderFore;

        public Form1()
        {
            InitializeComponent();

            CaptureGridDefaults();

            // ReaLTaiizor Material theme, following the Windows app mode
            // (light/dark) and re-applying itself when the OS setting changes.
            OsThemeWatcher.Initialize();
            OsThemeWatcher.DarkModeChanged += OsThemeWatcher_DarkModeChanged;
            FormClosing += Form1_FormClosing;

            var skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            ApplyTheme(OsThemeWatcher.IsDarkMode);

            Load += Form1_LoadAsync;
        }

        private void CaptureGridDefaults()
        {
            _gridOriginalBackground = driverGridView.BackgroundColor;
            _gridOriginalGridColor = driverGridView.GridColor;
            _gridOriginalCellBack = driverGridView.DefaultCellStyle.BackColor;
            _gridOriginalCellFore = driverGridView.DefaultCellStyle.ForeColor;
            _gridOriginalCellSelBack = driverGridView.DefaultCellStyle.SelectionBackColor;
            _gridOriginalCellSelFore = driverGridView.DefaultCellStyle.SelectionForeColor;
            _gridOriginalHeaderBack = driverGridView.ColumnHeadersDefaultCellStyle.BackColor;
            _gridOriginalHeaderFore = driverGridView.ColumnHeadersDefaultCellStyle.ForeColor;
            _gridOriginalRowHeaderBack = driverGridView.RowHeadersDefaultCellStyle.BackColor;
            _gridOriginalRowHeaderFore = driverGridView.RowHeadersDefaultCellStyle.ForeColor;
        }

        private void ApplyTheme(bool darkMode)
        {
            var skinManager = MaterialSkinManager.Instance;
            skinManager.Theme = darkMode
                ? MaterialSkinManager.Themes.DARK
                : MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = darkMode ? _darkScheme : _lightScheme;

            ApplyGridTheme(darkMode);

            // The skin manager resets the form style while applying the theme,
            // so re-apply the action bar afterwards.
            FormStyle = ReaLTaiizor.Enum.Material.FormStyles.ActionBar_64;

            // Repaint the custom title (OnPaint reads the current color scheme).
            Invalidate(true);
        }

        private void ApplyGridTheme(bool darkMode)
        {
            if (darkMode)
            {
                driverGridView.EnableHeadersVisualStyles = false;
                driverGridView.BackgroundColor = Color.FromArgb(33, 33, 33);
                driverGridView.GridColor = Color.FromArgb(66, 66, 66);
                driverGridView.DefaultCellStyle.BackColor = Color.FromArgb(48, 48, 48);
                driverGridView.DefaultCellStyle.ForeColor = Color.FromArgb(224, 224, 224);
                driverGridView.DefaultCellStyle.SelectionBackColor =
                    MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
                driverGridView.DefaultCellStyle.SelectionForeColor = Color.White;
                driverGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 50, 56);
                driverGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                driverGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 50, 56);
                driverGridView.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            }
            else
            {
                driverGridView.EnableHeadersVisualStyles = true;
                driverGridView.BackgroundColor = _gridOriginalBackground;
                driverGridView.GridColor = _gridOriginalGridColor;
                driverGridView.DefaultCellStyle.BackColor = _gridOriginalCellBack;
                driverGridView.DefaultCellStyle.ForeColor = _gridOriginalCellFore;
                driverGridView.DefaultCellStyle.SelectionBackColor = _gridOriginalCellSelBack;
                driverGridView.DefaultCellStyle.SelectionForeColor = _gridOriginalCellSelFore;
                driverGridView.ColumnHeadersDefaultCellStyle.BackColor = _gridOriginalHeaderBack;
                driverGridView.ColumnHeadersDefaultCellStyle.ForeColor = _gridOriginalHeaderFore;
                driverGridView.RowHeadersDefaultCellStyle.BackColor = _gridOriginalRowHeaderBack;
                driverGridView.RowHeadersDefaultCellStyle.ForeColor = _gridOriginalRowHeaderFore;
            }
        }

        private void OsThemeWatcher_DarkModeChanged(bool darkMode)
        {
            // SystemEvents can raise on a non-UI thread; marshal back if needed.
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(ApplyTheme), darkMode);
                return;
            }

            ApplyTheme(darkMode);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            OsThemeWatcher.DarkModeChanged -= OsThemeWatcher_DarkModeChanged;
            OsThemeWatcher.Shutdown();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // ReaLTaiizor draws the window title in the 64px action bar and the
            // minimize/maximize/close buttons in the 24px status bar above it.
            // Move the title text into the same bar as the buttons, using a
            // small font so it fits the 24px strip.
            var skin = MaterialSkinManager.Instance;

            // 1) Erase the big title rendered by the base class in the action
            //    bar band (24px status bar + 64px action bar, ActionBar_64).
            using (var barBrush = new SolidBrush(skin.ColorScheme.PrimaryColor))
            {
                e.Graphics.FillRectangle(barBrush, 0, 24, ClientSize.Width, 64);
            }

            // 2) Draw the title inside the 24px status bar, left of the window
            //    buttons (they occupy the right 72px: 3 x 24px).
            if (_titleFont == null)
            {
                _titleFont = skin.GetFontByType(MaterialSkinManager.FontType.Body2);
            }

            using (var textBrush = new SolidBrush(skin.ColorScheme.TextColor))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            {
                var titleRect = new RectangleF(
                    16f,
                    0f,
                    Math.Max(0f, ClientSize.Width - 96f),
                    24f);
                e.Graphics.DrawString(Text, _titleFont, textBrush, titleRect, format);
            }
        }

        private async void Form1_LoadAsync(object sender, EventArgs e)
        {
            driverGridView.CurrentCellDirtyStateChanged += DriverGridView_CurrentCellDirtyStateChanged;

            await UpdateDataGridViewAsync();
            driverGridView.DataSource = new BindingListView<Driver>(_bindingSource);

            // The grid columns are auto-generated from the Driver model, so
            // their headers default to the raw property names. Replace them
            // with localized header texts from the string resources.
            ApplyColumnHeaders();

            for (var i = 0; i < driverGridView.ColumnCount; i++)
            {
                if (i == 0)
                {
                    driverGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
                    continue;
                }

                driverGridView.Columns[i].ReadOnly = true;
                driverGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;

                if (i == driverGridView.ColumnCount - 1)
                {
                    driverGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }

            driverGridView.Update();
            driverGridView.Refresh();
        }

        // Gives every auto-generated grid column a localized header text
        // looked up in the string resources of the active UI language
        // (neutral English is the fallback). Columns are matched by the
        // Driver property name they display.
        private void ApplyColumnHeaders()
        {
            SetColumnHeader("Checked", Resources.Form1_Column_Checked);
            SetColumnHeader("FileName", Resources.Form1_Column_FileName);
            SetColumnHeader("SourceName", Resources.Form1_Column_SourceName);
            SetColumnHeader("Publisher", Resources.Form1_Column_Publisher);
            SetColumnHeader("DriverClass", Resources.Form1_Column_DriverClass);
            SetColumnHeader("ClassGuid", Resources.Form1_Column_ClassGuid);
            SetColumnHeader("DateAndVersion", Resources.Form1_Column_DateAndVersion);
            SetColumnHeader("CertificateSignerName", Resources.Form1_Column_CertificateSignerName);
        }

        private void SetColumnHeader(string propertyName, string headerText)
        {
            var column = driverGridView.Columns[propertyName];

            if (column != null)
            {
                column.HeaderText = headerText;
            }
        }

        private async Task UpdateDataGridViewAsync()
        {
            _bindingSource.Clear();

            foreach (var driver in await PnpUtilHelper.EnumDrivers())
            {
                _bindingSource.Add(driver);
            }

            // The refreshed list starts with every row unchecked.
            selectAllCheckBox.Checked = false;
        }

        private async void DeleteButton_ClickAsync(object sender, EventArgs e)
        {
            driverGridView.EndEdit();

            var selected = _bindingSource.Where(x => x.Checked).ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show(
                    Resources.Form1_NoDriversSelected_Message,
                    Resources.Form1_DeleteButton_Click_MessageBox_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result =
                MessageBox.Show(
                    string.Format(Resources.Form1_DeleteButton_Click_MessageBoxText, selected.Count),
                    Resources.Form1_DeleteButton_Click_MessageBox_Caption,
                    MessageBoxButtons.YesNo
                );

            if (result != DialogResult.Yes)
            {
                return;
            }

            foreach (var item in selected)
            {
                logTextBox.AppendText(
                    string.Format(Resources.Form1_LogDriver, item.FileName) + Environment.NewLine);

                try
                {
                    var output = await PnpUtilHelper.DeleteDriver(item.FileName, forceCheckBox.Checked);
                    logTextBox.AppendText(output + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    logTextBox.AppendText(
                        string.Format(
                            Resources.Form1_LogErrorDeleting, item.FileName, ex.Message) +
                        Environment.NewLine);
                }

                logTextBox.AppendText(Environment.NewLine);
            }

            logTextBox.AppendText(
                "==================================================" + Environment.NewLine + Environment.NewLine);

            await UpdateDataGridViewAsync();
        }

        private async void RefreshButton_ClickAsync(object sender, EventArgs e)
        {
            await UpdateDataGridViewAsync();
        }

        private void DriverGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Commit checkbox edits immediately so the Checked value is up to
            // date when the user clicks Export/Delete right after checking a
            // row (otherwise the last click would still be a pending edit).
            if (driverGridView.IsCurrentCellDirty)
            {
                driverGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void SelectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var driver in _bindingSource)
            {
                driver.Checked = selectAllCheckBox.Checked;
            }

            // BindingListView does not forward item property-change
            // notifications to the grid, so force the bound controls to
            // re-read the values from the data source (otherwise the row
            // checkboxes would not visibly update on screen).
            driverGridView.Refresh();

            if (driverGridView.DataSource != null && driverGridView.BindingContext != null)
            {
                var currencyManager =
                    driverGridView.BindingContext[driverGridView.DataSource] as CurrencyManager;
                currencyManager?.Refresh();
            }
        }

        private async void ExportButton_ClickAsync(object sender, EventArgs e)
        {
            driverGridView.EndEdit();

            var selected = _bindingSource.Where(x => x.Checked).ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show(
                    Resources.Form1_NoDriversSelected_Message,
                    Resources.Form1_DeleteButton_Click_MessageBox_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return;
                }

                logTextBox.AppendText(
                    string.Format(
                        Resources.Form1_LogExporting, selected.Count, dialog.SelectedPath) +
                    Environment.NewLine);

                foreach (var driver in selected)
                {
                    logTextBox.AppendText(
                        string.Format(Resources.Form1_LogDriver, driver.FileName) + Environment.NewLine);

                    try
                    {
                        var output = await PnpUtilHelper.ExportDriver(driver.FileName, dialog.SelectedPath);
                        logTextBox.AppendText(output + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        logTextBox.AppendText(
                            string.Format(
                                Resources.Form1_LogErrorExporting, driver.FileName, ex.Message) +
                            Environment.NewLine);
                    }

                    logTextBox.AppendText(Environment.NewLine);
                }

                logTextBox.AppendText(
                    "==================================================" + Environment.NewLine + Environment.NewLine);
            }
        }

        private ListSortDirection _sortColumnDirection;

        private int _currentSortColumnIndex;

        private void driverGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (driverGridView.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
            {
                if (e.ColumnIndex == _currentSortColumnIndex)
                {
                    _sortColumnDirection = _sortColumnDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }

                _currentSortColumnIndex = e.ColumnIndex;

                switch (_sortColumnDirection)
                {
                    case ListSortDirection.Ascending:
                        driverGridView.Sort(
                            driverGridView.Columns[_currentSortColumnIndex],
                            ListSortDirection.Ascending);
                        break;
                    case ListSortDirection.Descending:
                        driverGridView.Sort(
                            driverGridView.Columns[_currentSortColumnIndex],
                            ListSortDirection.Descending);
                        break;
                }
            }
        }
    }
}
