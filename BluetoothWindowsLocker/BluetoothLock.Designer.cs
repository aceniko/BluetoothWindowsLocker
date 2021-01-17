
namespace BluetoothWindowsLocker
{
    partial class BluetoothLock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BluetoothLock));
            this.m_labelUsedDevices = new System.Windows.Forms.Label();
            this.m_deviceTextbox = new System.Windows.Forms.TextBox();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_buttonStart = new System.Windows.Forms.Button();
            this.m_buttonStop = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.m_labelTimeout = new System.Windows.Forms.Label();
            this.timeoutBox = new System.Windows.Forms.NumericUpDown();
            this.m_buttonHide = new System.Windows.Forms.Button();
            this.m_buttonExit = new System.Windows.Forms.Button();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.m_settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_labelMinutes = new System.Windows.Forms.Label();
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_abortPendingLockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutBox)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelUsedDevices
            // 
            this.m_labelUsedDevices.AutoSize = true;
            this.m_labelUsedDevices.Location = new System.Drawing.Point(38, 49);
            this.m_labelUsedDevices.Name = "m_labelUsedDevices";
            this.m_labelUsedDevices.Size = new System.Drawing.Size(89, 13);
            this.m_labelUsedDevices.TabIndex = 2;
            this.m_labelUsedDevices.Text = "Bluetooth Device";
            // 
            // m_deviceTextbox
            // 
            this.m_deviceTextbox.Enabled = false;
            this.m_deviceTextbox.Location = new System.Drawing.Point(41, 75);
            this.m_deviceTextbox.Name = "m_deviceTextbox";
            this.m_deviceTextbox.ReadOnly = true;
            this.m_deviceTextbox.Size = new System.Drawing.Size(257, 20);
            this.m_deviceTextbox.TabIndex = 3;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(301, 73);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(30, 23);
            this.buttonSelect.TabIndex = 4;
            this.buttonSelect.Text = "...";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.SelectDeviceClickHandler);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 184);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(399, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 23;
            this.statusStrip.Text = "Stopped";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(51, 17);
            this.toolStripStatusLabel.Text = "Stopped";
            // 
            // m_buttonStart
            // 
            this.m_buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_buttonStart.Location = new System.Drawing.Point(41, 149);
            this.m_buttonStart.Name = "m_buttonStart";
            this.m_buttonStart.Size = new System.Drawing.Size(70, 23);
            this.m_buttonStart.TabIndex = 24;
            this.m_buttonStart.Text = "&Start";
            this.m_buttonStart.UseVisualStyleBackColor = true;
            this.m_buttonStart.Click += new System.EventHandler(this.StartClickHandler);
            // 
            // m_buttonStop
            // 
            this.m_buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_buttonStop.Location = new System.Drawing.Point(117, 149);
            this.m_buttonStop.Name = "m_buttonStop";
            this.m_buttonStop.Size = new System.Drawing.Size(70, 23);
            this.m_buttonStop.TabIndex = 25;
            this.m_buttonStop.Text = "S&top";
            this.m_buttonStop.UseVisualStyleBackColor = true;
            this.m_buttonStop.Click += new System.EventHandler(this.StopClickHandler);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "Bluetooth Proximity Lock";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Stopped";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClickHandler);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconDoubleClick);
            // 
            // m_labelTimeout
            // 
            this.m_labelTimeout.AutoSize = true;
            this.m_labelTimeout.Location = new System.Drawing.Point(38, 103);
            this.m_labelTimeout.Name = "m_labelTimeout";
            this.m_labelTimeout.Size = new System.Drawing.Size(48, 13);
            this.m_labelTimeout.TabIndex = 26;
            this.m_labelTimeout.Text = "Timeout:";
            // 
            // timeoutBox
            // 
            this.timeoutBox.Location = new System.Drawing.Point(133, 101);
            this.timeoutBox.Name = "timeoutBox";
            this.timeoutBox.Size = new System.Drawing.Size(70, 20);
            this.timeoutBox.TabIndex = 27;
            // 
            // m_buttonHide
            // 
            this.m_buttonHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_buttonHide.Location = new System.Drawing.Point(193, 149);
            this.m_buttonHide.Name = "m_buttonHide";
            this.m_buttonHide.Size = new System.Drawing.Size(70, 23);
            this.m_buttonHide.TabIndex = 38;
            this.m_buttonHide.Text = "&Hide";
            this.m_buttonHide.UseVisualStyleBackColor = true;
            this.m_buttonHide.Click += new System.EventHandler(this.HideClickHandler);
            // 
            // m_buttonExit
            // 
            this.m_buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_buttonExit.Location = new System.Drawing.Point(269, 149);
            this.m_buttonExit.Name = "m_buttonExit";
            this.m_buttonExit.Size = new System.Drawing.Size(70, 23);
            this.m_buttonExit.TabIndex = 39;
            this.m_buttonExit.Text = "E&xit";
            this.m_buttonExit.UseVisualStyleBackColor = true;
            this.m_buttonExit.Click += new System.EventHandler(this.ExitClickHandler);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_settingsToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(399, 24);
            this.mainMenu.TabIndex = 41;
            this.mainMenu.Text = "menuStrip1";
            // 
            // m_settingsToolStripMenuItem
            // 
            this.m_settingsToolStripMenuItem.Name = "m_settingsToolStripMenuItem";
            this.m_settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.m_settingsToolStripMenuItem.Text = "&Options";
            this.m_settingsToolStripMenuItem.Click += new System.EventHandler(this.OptionsMenuItem);
            // 
            // m_labelMinutes
            // 
            this.m_labelMinutes.AutoSize = true;
            this.m_labelMinutes.Location = new System.Drawing.Point(209, 103);
            this.m_labelMinutes.Name = "m_labelMinutes";
            this.m_labelMinutes.Size = new System.Drawing.Size(44, 13);
            this.m_labelMinutes.TabIndex = 43;
            this.m_labelMinutes.Text = "Minutes";
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_restoreToolStripMenuItem,
            this.m_abortPendingLockToolStripMenuItem,
            this.m_aboutToolStripMenuItem,
            this.m_exitToolStripMenuItem});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(181, 114);
            // 
            // m_restoreToolStripMenuItem
            // 
            this.m_restoreToolStripMenuItem.Name = "m_restoreToolStripMenuItem";
            this.m_restoreToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.m_restoreToolStripMenuItem.Text = "&Restore";
            this.m_restoreToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuRestore);
            // 
            // m_abortPendingLockToolStripMenuItem
            // 
            this.m_abortPendingLockToolStripMenuItem.Name = "m_abortPendingLockToolStripMenuItem";
            this.m_abortPendingLockToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.m_abortPendingLockToolStripMenuItem.Text = "Abo&rt pending lock";
            this.m_abortPendingLockToolStripMenuItem.Click += new System.EventHandler(this.AbortPendingLockHandler);
            // 
            // m_aboutToolStripMenuItem
            // 
            this.m_aboutToolStripMenuItem.Name = "m_aboutToolStripMenuItem";
            this.m_aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.m_aboutToolStripMenuItem.Text = "&About";
            this.m_aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutMenuItemHandler);
            // 
            // m_exitToolStripMenuItem
            // 
            this.m_exitToolStripMenuItem.Name = "m_exitToolStripMenuItem";
            this.m_exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.m_exitToolStripMenuItem.Text = "E&xit";
            this.m_exitToolStripMenuItem.Click += new System.EventHandler(this.ExitClickHandler);
            // 
            // BluetoothLock
            // 
            this.AcceptButton = this.m_buttonStop;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_buttonExit;
            this.ClientSize = new System.Drawing.Size(399, 206);
            this.Controls.Add(this.m_labelMinutes);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.m_buttonExit);
            this.Controls.Add(this.m_buttonHide);
            this.Controls.Add(this.timeoutBox);
            this.Controls.Add(this.m_labelTimeout);
            this.Controls.Add(this.m_buttonStop);
            this.Controls.Add(this.m_buttonStart);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.buttonSelect);
            this.Controls.Add(this.m_deviceTextbox);
            this.Controls.Add(this.m_labelUsedDevices);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "BluetoothLock";
            this.Text = "BluetoothLock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosingHandler);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormClosedHandler);
            this.Load += new System.EventHandler(this.BtProxLockLoad);
            this.LocationChanged += new System.EventHandler(this.BtProxLockLocationChanged);
            this.SizeChanged += new System.EventHandler(this.BtProxSizeChanged);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutBox)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_labelUsedDevices;
        private System.Windows.Forms.TextBox m_deviceTextbox;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button m_buttonStart;
        private System.Windows.Forms.Button m_buttonStop;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label m_labelTimeout;
        private System.Windows.Forms.NumericUpDown timeoutBox;
        private System.Windows.Forms.Button m_buttonHide;
        private System.Windows.Forms.Button m_buttonExit;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem m_settingsToolStripMenuItem;
        private System.Windows.Forms.Label m_labelMinutes;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem m_restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_abortPendingLockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_exitToolStripMenuItem;
    }
}

