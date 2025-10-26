namespace PowerHouse_desktop
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grdFormData = new System.Windows.Forms.DataGridView();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnMarkAsRead = new System.Windows.Forms.Button();
            this.lblUnreadIndicator = new System.Windows.Forms.Label();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdFormData)).BeginInit();
            this.SuspendLayout();
            // 
            // grdFormData
            // 
            this.grdFormData.AllowUserToAddRows = false;
            this.grdFormData.AllowUserToDeleteRows = false;
            this.grdFormData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdFormData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdFormData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.grdFormData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdFormData.DefaultCellStyle = dataGridViewCellStyle1;
            this.grdFormData.Location = new System.Drawing.Point(12, 41);
            this.grdFormData.Name = "grdFormData";
            this.grdFormData.ReadOnly = false;
            this.grdFormData.RowHeadersVisible = false;
            this.grdFormData.RowHeadersWidth = 51;
            this.grdFormData.RowTemplate.Height = 29;
            this.grdFormData.Size = new System.Drawing.Size(1030, 383);
            this.grdFormData.TabIndex = 0;
            this.grdFormData.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdFormData_CellContentClick);
            this.grdFormData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdFormData_CellValueChanged);
            this.grdFormData.CurrentCellDirtyStateChanged += new System.EventHandler(this.grdFormData_CurrentCellDirtyStateChanged);
            // 
            // btnReload
            // 
            this.btnReload.BackColor = System.Drawing.Color.Red;
            this.btnReload.Location = new System.Drawing.Point(12, 3);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(119, 32);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnMarkAsRead
            // 
            this.btnMarkAsRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarkAsRead.Location = new System.Drawing.Point(923, 3);
            this.btnMarkAsRead.Name = "btnMarkAsRead";
            this.btnMarkAsRead.Size = new System.Drawing.Size(119, 32);
            this.btnMarkAsRead.TabIndex = 2;
            this.btnMarkAsRead.Text = "Mark as Read";
            this.btnMarkAsRead.UseVisualStyleBackColor = true;
            this.btnMarkAsRead.Click += new System.EventHandler(this.btnMarkAsRead_Click);
            // 
            // lblUnreadIndicator
            // 
            this.lblUnreadIndicator.AutoSize = true;
            this.lblUnreadIndicator.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUnreadIndicator.ForeColor = System.Drawing.Color.Red;
            this.lblUnreadIndicator.Location = new System.Drawing.Point(151, 10);
            this.lblUnreadIndicator.Name = "lblUnreadIndicator";
            this.lblUnreadIndicator.Size = new System.Drawing.Size(117, 20);
            this.lblUnreadIndicator.TabIndex = 3;
            this.lblUnreadIndicator.Text = "Unread: (none)";
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 30000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.Text = "Powerhouse updates";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 450);
            this.Controls.Add(this.lblUnreadIndicator);
            this.Controls.Add(this.btnMarkAsRead);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.grdFormData);
            this.Name = "FrmMain";
            this.Text = "Powerhouse";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdFormData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView grdFormData;
        private Button btnReload;
        private Button btnMarkAsRead;
        private Label lblUnreadIndicator;
        private System.Windows.Forms.Timer refreshTimer;
        private NotifyIcon notifyIcon;
    }
}
