namespace PnpUtilGui
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.driverGridView = new System.Windows.Forms.DataGridView();
            this.refreshButton = new ReaLTaiizor.Controls.MaterialButton();
            this.deleteButton = new ReaLTaiizor.Controls.MaterialButton();
            this.forceCheckBox = new ReaLTaiizor.Controls.MaterialCheckBox();
            this.logTextBox = new ReaLTaiizor.Controls.MaterialRichTextBox();
            this.exportButton = new PnpUtilGui.Controls.FixedColorMaterialButton();
            this.selectAllCheckBox = new ReaLTaiizor.Controls.MaterialCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.driverGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // driverGridView
            // 
            resources.ApplyResources(this.driverGridView, "driverGridView");
            this.driverGridView.AllowUserToAddRows = false;
            this.driverGridView.AllowUserToDeleteRows = false;
            this.driverGridView.AllowUserToOrderColumns = true;
            this.driverGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.driverGridView.MultiSelect = false;
            this.driverGridView.Name = "driverGridView";
            this.driverGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.driverGridView_ColumnHeaderMouseClick);
            //
            // refreshButton
            //
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.AutoSize = false;
            this.refreshButton.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            this.refreshButton.HighEmphasis = true;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_ClickAsync);
            //
            // deleteButton
            //
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.AutoSize = false;
            this.deleteButton.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            this.deleteButton.HighEmphasis = true;
            this.deleteButton.UseAccentColor = true;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.DeleteButton_ClickAsync);
            //
            // forceCheckBox
            //
            resources.ApplyResources(this.forceCheckBox, "forceCheckBox");
            this.forceCheckBox.AutoSize = false;
            this.forceCheckBox.Size = new System.Drawing.Size(150, 30);
            this.forceCheckBox.Name = "forceCheckBox";
            // 
            // logTextBox
            // 
            resources.ApplyResources(this.logTextBox, "logTextBox");
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            //
            // exportButton
            //
            resources.ApplyResources(this.exportButton, "exportButton");
            this.exportButton.AutoSize = false;
            this.exportButton.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            this.exportButton.HighEmphasis = true;
            this.exportButton.FixedColor = System.Drawing.Color.FromArgb(0, 103, 192);
            this.exportButton.Name = "exportButton";
            this.exportButton.Click += new System.EventHandler(this.ExportButton_ClickAsync);
            //
            // selectAllCheckBox
            //
            this.selectAllCheckBox.AutoSize = false;
            this.selectAllCheckBox.Location = new System.Drawing.Point(280, 92);
            this.selectAllCheckBox.Name = "selectAllCheckBox";
            this.selectAllCheckBox.Size = new System.Drawing.Size(150, 30);
            this.selectAllCheckBox.TabIndex = 6;
            this.selectAllCheckBox.Text = Properties.Resources.Form1_SelectAllCheckBox_Text;
            this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.SelectAllCheckBox_CheckedChanged);
            //
            // Form1
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormStyle = ReaLTaiizor.Enum.Material.FormStyles.ActionBar_64;
            this.Controls.Add(this.selectAllCheckBox);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.forceCheckBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.driverGridView);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.driverGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView driverGridView;
        private ReaLTaiizor.Controls.MaterialButton refreshButton;
        private ReaLTaiizor.Controls.MaterialButton deleteButton;
        private ReaLTaiizor.Controls.MaterialCheckBox forceCheckBox;
        private ReaLTaiizor.Controls.MaterialRichTextBox logTextBox;
        private PnpUtilGui.Controls.FixedColorMaterialButton exportButton;
        private ReaLTaiizor.Controls.MaterialCheckBox selectAllCheckBox;
    }
}

