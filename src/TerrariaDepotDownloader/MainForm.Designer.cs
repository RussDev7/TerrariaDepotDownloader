
namespace TerrariaDepotDownloader
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Main_ListView = new System.Windows.Forms.ListView();
            this.ManifestID_ColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Version_ColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Installed_ColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Main_TabControl = new System.Windows.Forms.TabControl();
            this.Downloader_TabPage = new System.Windows.Forms.TabPage();
            this.OpenDepots_Button = new System.Windows.Forms.Button();
            this.RemoveAll_Button = new System.Windows.Forms.Button();
            this.ReloadList_Button = new System.Windows.Forms.Button();
            this.Settings_TabPage = new System.Windows.Forms.TabPage();
            this.Options_GroupBox = new System.Windows.Forms.GroupBox();
            this.UseSeperateConfigs_CheckBox = new System.Windows.Forms.CheckBox();
            this.EnableCollectorsEdition_CheckBox = new System.Windows.Forms.CheckBox();
            this.DarkMode_CheckBox = new System.Windows.Forms.CheckBox();
            this.RememberLogin_CheckBox = new System.Windows.Forms.CheckBox();
            this.SkipUpdateCheck_CheckBox = new System.Windows.Forms.CheckBox();
            this.LogActions_CheckBox = new System.Windows.Forms.CheckBox();
            this.ShowTooltips_CheckBox = new System.Windows.Forms.CheckBox();
            this.UseSteamDirectory_CheckBox = new System.Windows.Forms.CheckBox();
            this.BaseDepotDirectory_GroupBox = new System.Windows.Forms.GroupBox();
            this.Browse_Button = new System.Windows.Forms.Button();
            this.BaseDepotDirectory_TextBox = new System.Windows.Forms.TextBox();
            this.SteamLoginInformation_GroupBox = new System.Windows.Forms.GroupBox();
            this.AccountName_Label = new System.Windows.Forms.Label();
            this.AccountName_TextBox = new System.Windows.Forms.TextBox();
            this.Show_Button = new System.Windows.Forms.Button();
            this.Password_Label = new System.Windows.Forms.Label();
            this.Password_TextBox = new System.Windows.Forms.TextBox();
            this.Log_TabPage = new System.Windows.Forms.TabPage();
            this.Log_RichTextBox = new System.Windows.Forms.RichTextBox();
            this.ClearLog_Button = new System.Windows.Forms.Button();
            this.Main_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.File_ToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.Close_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Info_ToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.Close_Button = new System.Windows.Forms.Button();
            this.Launch_Button = new System.Windows.Forms.Button();
            this.Logo_PictureBox = new System.Windows.Forms.PictureBox();
            this.RemoveApp_Button = new System.Windows.Forms.Button();
            this.Main_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DownloadApp_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveApp_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Main_TabControl.SuspendLayout();
            this.Downloader_TabPage.SuspendLayout();
            this.Settings_TabPage.SuspendLayout();
            this.Options_GroupBox.SuspendLayout();
            this.BaseDepotDirectory_GroupBox.SuspendLayout();
            this.SteamLoginInformation_GroupBox.SuspendLayout();
            this.Log_TabPage.SuspendLayout();
            this.Main_ToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).BeginInit();
            this.Main_ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // Main_ListView
            // 
            this.Main_ListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Main_ListView.BackColor = System.Drawing.SystemColors.Window;
            this.Main_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ManifestID_ColumnHeader,
            this.Version_ColumnHeader,
            this.Installed_ColumnHeader});
            this.Main_ListView.FullRowSelect = true;
            this.Main_ListView.GridLines = true;
            this.Main_ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Main_ListView.HideSelection = false;
            this.Main_ListView.Location = new System.Drawing.Point(0, 0);
            this.Main_ListView.Name = "Main_ListView";
            this.Main_ListView.Size = new System.Drawing.Size(540, 251);
            this.Main_ListView.TabIndex = 2;
            this.Main_ListView.UseCompatibleStateImageBehavior = false;
            this.Main_ListView.View = System.Windows.Forms.View.Details;
            this.Main_ListView.Click += new System.EventHandler(this.Main_ListView_Click);
            this.Main_ListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Main_ListView_MouseClick);
            // 
            // ManifestID_ColumnHeader
            // 
            this.ManifestID_ColumnHeader.Text = "Version";
            this.ManifestID_ColumnHeader.Width = 91;
            // 
            // Version_ColumnHeader
            // 
            this.Version_ColumnHeader.Text = "Manifest ID";
            this.Version_ColumnHeader.Width = 200;
            // 
            // Installed_ColumnHeader
            // 
            this.Installed_ColumnHeader.Text = "Installed";
            this.Installed_ColumnHeader.Width = 204;
            // 
            // Main_TabControl
            // 
            this.Main_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Main_TabControl.Controls.Add(this.Downloader_TabPage);
            this.Main_TabControl.Controls.Add(this.Settings_TabPage);
            this.Main_TabControl.Controls.Add(this.Log_TabPage);
            this.Main_TabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Main_TabControl.ItemSize = new System.Drawing.Size(28, 28);
            this.Main_TabControl.Location = new System.Drawing.Point(12, 144);
            this.Main_TabControl.Name = "Main_TabControl";
            this.Main_TabControl.Padding = new System.Drawing.Point(40, 3);
            this.Main_TabControl.SelectedIndex = 0;
            this.Main_TabControl.Size = new System.Drawing.Size(550, 328);
            this.Main_TabControl.TabIndex = 1;
            // 
            // Downloader_TabPage
            // 
            this.Downloader_TabPage.Controls.Add(this.OpenDepots_Button);
            this.Downloader_TabPage.Controls.Add(this.RemoveAll_Button);
            this.Downloader_TabPage.Controls.Add(this.ReloadList_Button);
            this.Downloader_TabPage.Controls.Add(this.Main_ListView);
            this.Downloader_TabPage.Location = new System.Drawing.Point(4, 32);
            this.Downloader_TabPage.Name = "Downloader_TabPage";
            this.Downloader_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.Downloader_TabPage.Size = new System.Drawing.Size(542, 292);
            this.Downloader_TabPage.TabIndex = 0;
            this.Downloader_TabPage.Text = "Downloader";
            this.Downloader_TabPage.UseVisualStyleBackColor = true;
            // 
            // OpenDepots_Button
            // 
            this.OpenDepots_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenDepots_Button.Location = new System.Drawing.Point(440, 257);
            this.OpenDepots_Button.Name = "OpenDepots_Button";
            this.OpenDepots_Button.Size = new System.Drawing.Size(102, 29);
            this.OpenDepots_Button.TabIndex = 4;
            this.OpenDepots_Button.Text = "Open Depots";
            this.OpenDepots_Button.UseVisualStyleBackColor = true;
            this.OpenDepots_Button.Click += new System.EventHandler(this.OpenDepots_Button_Click);
            // 
            // RemoveAll_Button
            // 
            this.RemoveAll_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveAll_Button.Location = new System.Drawing.Point(94, 257);
            this.RemoveAll_Button.Name = "RemoveAll_Button";
            this.RemoveAll_Button.Size = new System.Drawing.Size(102, 29);
            this.RemoveAll_Button.TabIndex = 3;
            this.RemoveAll_Button.Text = "Remove All";
            this.RemoveAll_Button.UseVisualStyleBackColor = true;
            this.RemoveAll_Button.Click += new System.EventHandler(this.RemoveAll_Button_Click);
            // 
            // ReloadList_Button
            // 
            this.ReloadList_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ReloadList_Button.Location = new System.Drawing.Point(0, 257);
            this.ReloadList_Button.Name = "ReloadList_Button";
            this.ReloadList_Button.Size = new System.Drawing.Size(88, 29);
            this.ReloadList_Button.TabIndex = 2;
            this.ReloadList_Button.Text = "Reload List";
            this.ReloadList_Button.UseVisualStyleBackColor = true;
            this.ReloadList_Button.Click += new System.EventHandler(this.ReloadList_Button_Click);
            // 
            // Settings_TabPage
            // 
            this.Settings_TabPage.Controls.Add(this.Options_GroupBox);
            this.Settings_TabPage.Controls.Add(this.BaseDepotDirectory_GroupBox);
            this.Settings_TabPage.Controls.Add(this.SteamLoginInformation_GroupBox);
            this.Settings_TabPage.Location = new System.Drawing.Point(4, 32);
            this.Settings_TabPage.Name = "Settings_TabPage";
            this.Settings_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.Settings_TabPage.Size = new System.Drawing.Size(542, 292);
            this.Settings_TabPage.TabIndex = 1;
            this.Settings_TabPage.Text = "Settings";
            this.Settings_TabPage.UseVisualStyleBackColor = true;
            // 
            // Options_GroupBox
            // 
            this.Options_GroupBox.Controls.Add(this.UseSeperateConfigs_CheckBox);
            this.Options_GroupBox.Controls.Add(this.EnableCollectorsEdition_CheckBox);
            this.Options_GroupBox.Controls.Add(this.DarkMode_CheckBox);
            this.Options_GroupBox.Controls.Add(this.RememberLogin_CheckBox);
            this.Options_GroupBox.Controls.Add(this.SkipUpdateCheck_CheckBox);
            this.Options_GroupBox.Controls.Add(this.LogActions_CheckBox);
            this.Options_GroupBox.Controls.Add(this.ShowTooltips_CheckBox);
            this.Options_GroupBox.Controls.Add(this.UseSteamDirectory_CheckBox);
            this.Options_GroupBox.Location = new System.Drawing.Point(12, 171);
            this.Options_GroupBox.Name = "Options_GroupBox";
            this.Options_GroupBox.Size = new System.Drawing.Size(519, 110);
            this.Options_GroupBox.TabIndex = 0;
            this.Options_GroupBox.TabStop = false;
            this.Options_GroupBox.Text = "Options";
            // 
            // UseSeperateConfigs_CheckBox
            // 
            this.UseSeperateConfigs_CheckBox.AutoSize = true;
            this.UseSeperateConfigs_CheckBox.Location = new System.Drawing.Point(167, 80);
            this.UseSeperateConfigs_CheckBox.Name = "UseSeperateConfigs_CheckBox";
            this.UseSeperateConfigs_CheckBox.Size = new System.Drawing.Size(158, 20);
            this.UseSeperateConfigs_CheckBox.TabIndex = 17;
            this.UseSeperateConfigs_CheckBox.Text = "Use Separate Configs";
            this.UseSeperateConfigs_CheckBox.UseVisualStyleBackColor = true;
            this.UseSeperateConfigs_CheckBox.CheckedChanged += new System.EventHandler(this.UseSeperateConfigs_CheckBox_CheckedChanged);
            // 
            // EnableCollectorsEdition_CheckBox
            // 
            this.EnableCollectorsEdition_CheckBox.AutoSize = true;
            this.EnableCollectorsEdition_CheckBox.Location = new System.Drawing.Point(167, 60);
            this.EnableCollectorsEdition_CheckBox.Name = "EnableCollectorsEdition_CheckBox";
            this.EnableCollectorsEdition_CheckBox.Size = new System.Drawing.Size(176, 20);
            this.EnableCollectorsEdition_CheckBox.TabIndex = 16;
            this.EnableCollectorsEdition_CheckBox.Text = "Enable Collectors Edition";
            this.EnableCollectorsEdition_CheckBox.UseVisualStyleBackColor = true;
            this.EnableCollectorsEdition_CheckBox.CheckedChanged += new System.EventHandler(this.EnableCollectorsEdition_CheckBox_CheckedChanged);
            // 
            // DarkMode_CheckBox
            // 
            this.DarkMode_CheckBox.AutoSize = true;
            this.DarkMode_CheckBox.Location = new System.Drawing.Point(167, 40);
            this.DarkMode_CheckBox.Name = "DarkMode_CheckBox";
            this.DarkMode_CheckBox.Size = new System.Drawing.Size(93, 20);
            this.DarkMode_CheckBox.TabIndex = 15;
            this.DarkMode_CheckBox.Text = "Dark Mode";
            this.DarkMode_CheckBox.UseVisualStyleBackColor = true;
            this.DarkMode_CheckBox.CheckedChanged += new System.EventHandler(this.DarkMode_CheckBox_CheckedChanged);
            // 
            // RememberLogin_CheckBox
            // 
            this.RememberLogin_CheckBox.AutoSize = true;
            this.RememberLogin_CheckBox.Checked = true;
            this.RememberLogin_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RememberLogin_CheckBox.Location = new System.Drawing.Point(6, 60);
            this.RememberLogin_CheckBox.Name = "RememberLogin_CheckBox";
            this.RememberLogin_CheckBox.Size = new System.Drawing.Size(130, 20);
            this.RememberLogin_CheckBox.TabIndex = 12;
            this.RememberLogin_CheckBox.Text = "Remember Login";
            this.RememberLogin_CheckBox.UseVisualStyleBackColor = true;
            this.RememberLogin_CheckBox.CheckedChanged += new System.EventHandler(this.RememberLogin_CheckBox_CheckedChanged);
            // 
            // SkipUpdateCheck_CheckBox
            // 
            this.SkipUpdateCheck_CheckBox.AutoSize = true;
            this.SkipUpdateCheck_CheckBox.Location = new System.Drawing.Point(167, 20);
            this.SkipUpdateCheck_CheckBox.Name = "SkipUpdateCheck_CheckBox";
            this.SkipUpdateCheck_CheckBox.Size = new System.Drawing.Size(142, 20);
            this.SkipUpdateCheck_CheckBox.TabIndex = 14;
            this.SkipUpdateCheck_CheckBox.Text = "Skip Update Check";
            this.SkipUpdateCheck_CheckBox.UseVisualStyleBackColor = true;
            this.SkipUpdateCheck_CheckBox.CheckedChanged += new System.EventHandler(this.SkipUpdateCheck_CheckBox_CheckedChanged);
            // 
            // LogActions_CheckBox
            // 
            this.LogActions_CheckBox.AutoSize = true;
            this.LogActions_CheckBox.Checked = true;
            this.LogActions_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LogActions_CheckBox.Location = new System.Drawing.Point(6, 20);
            this.LogActions_CheckBox.Name = "LogActions_CheckBox";
            this.LogActions_CheckBox.Size = new System.Drawing.Size(96, 20);
            this.LogActions_CheckBox.TabIndex = 10;
            this.LogActions_CheckBox.Text = "Log Actions";
            this.LogActions_CheckBox.UseVisualStyleBackColor = true;
            this.LogActions_CheckBox.CheckedChanged += new System.EventHandler(this.LogActions_CheckBox_CheckedChanged);
            // 
            // ShowTooltips_CheckBox
            // 
            this.ShowTooltips_CheckBox.AutoSize = true;
            this.ShowTooltips_CheckBox.Checked = true;
            this.ShowTooltips_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowTooltips_CheckBox.Location = new System.Drawing.Point(6, 80);
            this.ShowTooltips_CheckBox.Name = "ShowTooltips_CheckBox";
            this.ShowTooltips_CheckBox.Size = new System.Drawing.Size(111, 20);
            this.ShowTooltips_CheckBox.TabIndex = 13;
            this.ShowTooltips_CheckBox.Text = "Show Tooltips";
            this.ShowTooltips_CheckBox.UseVisualStyleBackColor = true;
            this.ShowTooltips_CheckBox.CheckedChanged += new System.EventHandler(this.ShowTooltips_CheckBox_CheckedChanged);
            // 
            // UseSteamDirectory_CheckBox
            // 
            this.UseSteamDirectory_CheckBox.AutoSize = true;
            this.UseSteamDirectory_CheckBox.Checked = true;
            this.UseSteamDirectory_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseSteamDirectory_CheckBox.Location = new System.Drawing.Point(6, 40);
            this.UseSteamDirectory_CheckBox.Name = "UseSteamDirectory_CheckBox";
            this.UseSteamDirectory_CheckBox.Size = new System.Drawing.Size(150, 20);
            this.UseSteamDirectory_CheckBox.TabIndex = 11;
            this.UseSteamDirectory_CheckBox.Text = "Use Steam Directory";
            this.UseSteamDirectory_CheckBox.UseVisualStyleBackColor = true;
            this.UseSteamDirectory_CheckBox.CheckedChanged += new System.EventHandler(this.UseSteamDirectory_CheckBox_CheckedChanged);
            // 
            // BaseDepotDirectory_GroupBox
            // 
            this.BaseDepotDirectory_GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BaseDepotDirectory_GroupBox.Controls.Add(this.Browse_Button);
            this.BaseDepotDirectory_GroupBox.Controls.Add(this.BaseDepotDirectory_TextBox);
            this.BaseDepotDirectory_GroupBox.Location = new System.Drawing.Point(12, 15);
            this.BaseDepotDirectory_GroupBox.Name = "BaseDepotDirectory_GroupBox";
            this.BaseDepotDirectory_GroupBox.Size = new System.Drawing.Size(519, 55);
            this.BaseDepotDirectory_GroupBox.TabIndex = 0;
            this.BaseDepotDirectory_GroupBox.TabStop = false;
            this.BaseDepotDirectory_GroupBox.Text = "Base Depot Directory";
            // 
            // Browse_Button
            // 
            this.Browse_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Browse_Button.Location = new System.Drawing.Point(439, 21);
            this.Browse_Button.Name = "Browse_Button";
            this.Browse_Button.Size = new System.Drawing.Size(75, 23);
            this.Browse_Button.TabIndex = 6;
            this.Browse_Button.Text = "Browse";
            this.Browse_Button.UseVisualStyleBackColor = true;
            this.Browse_Button.Click += new System.EventHandler(this.Browse_Button_Click);
            // 
            // BaseDepotDirectory_TextBox
            // 
            this.BaseDepotDirectory_TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BaseDepotDirectory_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BaseDepotDirectory_TextBox.Location = new System.Drawing.Point(9, 21);
            this.BaseDepotDirectory_TextBox.Name = "BaseDepotDirectory_TextBox";
            this.BaseDepotDirectory_TextBox.ReadOnly = true;
            this.BaseDepotDirectory_TextBox.Size = new System.Drawing.Size(424, 22);
            this.BaseDepotDirectory_TextBox.TabIndex = 5;
            // 
            // SteamLoginInformation_GroupBox
            // 
            this.SteamLoginInformation_GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SteamLoginInformation_GroupBox.Controls.Add(this.AccountName_Label);
            this.SteamLoginInformation_GroupBox.Controls.Add(this.AccountName_TextBox);
            this.SteamLoginInformation_GroupBox.Controls.Add(this.Show_Button);
            this.SteamLoginInformation_GroupBox.Controls.Add(this.Password_Label);
            this.SteamLoginInformation_GroupBox.Controls.Add(this.Password_TextBox);
            this.SteamLoginInformation_GroupBox.Location = new System.Drawing.Point(12, 76);
            this.SteamLoginInformation_GroupBox.Name = "SteamLoginInformation_GroupBox";
            this.SteamLoginInformation_GroupBox.Size = new System.Drawing.Size(519, 89);
            this.SteamLoginInformation_GroupBox.TabIndex = 0;
            this.SteamLoginInformation_GroupBox.TabStop = false;
            this.SteamLoginInformation_GroupBox.Text = "Steam Login Information";
            // 
            // AccountName_Label
            // 
            this.AccountName_Label.AutoSize = true;
            this.AccountName_Label.Location = new System.Drawing.Point(6, 31);
            this.AccountName_Label.Name = "AccountName_Label";
            this.AccountName_Label.Size = new System.Drawing.Size(98, 16);
            this.AccountName_Label.TabIndex = 0;
            this.AccountName_Label.Text = "Account Name:";
            // 
            // AccountName_TextBox
            // 
            this.AccountName_TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountName_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AccountName_TextBox.Location = new System.Drawing.Point(111, 31);
            this.AccountName_TextBox.Name = "AccountName_TextBox";
            this.AccountName_TextBox.Size = new System.Drawing.Size(322, 22);
            this.AccountName_TextBox.TabIndex = 7;
            // 
            // Show_Button
            // 
            this.Show_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Show_Button.Location = new System.Drawing.Point(439, 59);
            this.Show_Button.Name = "Show_Button";
            this.Show_Button.Size = new System.Drawing.Size(75, 23);
            this.Show_Button.TabIndex = 9;
            this.Show_Button.Text = "Show";
            this.Show_Button.UseVisualStyleBackColor = true;
            this.Show_Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Show_Button_MouseDown);
            this.Show_Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Show_Button_MouseUp);
            // 
            // Password_Label
            // 
            this.Password_Label.AutoSize = true;
            this.Password_Label.Location = new System.Drawing.Point(6, 62);
            this.Password_Label.Name = "Password_Label";
            this.Password_Label.Size = new System.Drawing.Size(70, 16);
            this.Password_Label.TabIndex = 0;
            this.Password_Label.Text = "Password:";
            // 
            // Password_TextBox
            // 
            this.Password_TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Password_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Password_TextBox.Location = new System.Drawing.Point(111, 59);
            this.Password_TextBox.Name = "Password_TextBox";
            this.Password_TextBox.PasswordChar = '*';
            this.Password_TextBox.Size = new System.Drawing.Size(322, 22);
            this.Password_TextBox.TabIndex = 8;
            // 
            // Log_TabPage
            // 
            this.Log_TabPage.Controls.Add(this.Log_RichTextBox);
            this.Log_TabPage.Controls.Add(this.ClearLog_Button);
            this.Log_TabPage.Location = new System.Drawing.Point(4, 32);
            this.Log_TabPage.Name = "Log_TabPage";
            this.Log_TabPage.Size = new System.Drawing.Size(542, 292);
            this.Log_TabPage.TabIndex = 2;
            this.Log_TabPage.Text = "Log";
            this.Log_TabPage.UseVisualStyleBackColor = true;
            // 
            // Log_RichTextBox
            // 
            this.Log_RichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Log_RichTextBox.Location = new System.Drawing.Point(0, 0);
            this.Log_RichTextBox.Name = "Log_RichTextBox";
            this.Log_RichTextBox.ReadOnly = true;
            this.Log_RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Log_RichTextBox.Size = new System.Drawing.Size(540, 251);
            this.Log_RichTextBox.TabIndex = 18;
            this.Log_RichTextBox.Text = "";
            this.Log_RichTextBox.TextChanged += new System.EventHandler(this.Log_RichTextBox_TextChanged);
            // 
            // ClearLog_Button
            // 
            this.ClearLog_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ClearLog_Button.Location = new System.Drawing.Point(0, 257);
            this.ClearLog_Button.Name = "ClearLog_Button";
            this.ClearLog_Button.Size = new System.Drawing.Size(88, 29);
            this.ClearLog_Button.TabIndex = 19;
            this.ClearLog_Button.Text = "Clear Log";
            this.ClearLog_Button.UseVisualStyleBackColor = true;
            this.ClearLog_Button.Click += new System.EventHandler(this.ClearLog_Button_Click);
            // 
            // Main_ToolStrip
            // 
            this.Main_ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Main_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.File_ToolStripDropDownButton,
            this.Info_ToolStripDropDownButton});
            this.Main_ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.Main_ToolStrip.Name = "Main_ToolStrip";
            this.Main_ToolStrip.Size = new System.Drawing.Size(574, 27);
            this.Main_ToolStrip.TabIndex = 0;
            this.Main_ToolStrip.Text = "toolStrip1";
            // 
            // File_ToolStripDropDownButton
            // 
            this.File_ToolStripDropDownButton.AutoToolTip = false;
            this.File_ToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.File_ToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Close_ToolStripMenuItem});
            this.File_ToolStripDropDownButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.File_ToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("File_ToolStripDropDownButton.Image")));
            this.File_ToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.File_ToolStripDropDownButton.Name = "File_ToolStripDropDownButton";
            this.File_ToolStripDropDownButton.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.File_ToolStripDropDownButton.ShowDropDownArrow = false;
            this.File_ToolStripDropDownButton.Size = new System.Drawing.Size(46, 24);
            this.File_ToolStripDropDownButton.Text = "File";
            // 
            // Close_ToolStripMenuItem
            // 
            this.Close_ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Close_ToolStripMenuItem.Image = global::TerrariaDepotDownloader.Properties.Resources.CoolDown;
            this.Close_ToolStripMenuItem.Name = "Close_ToolStripMenuItem";
            this.Close_ToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.Close_ToolStripMenuItem.Text = "Close";
            this.Close_ToolStripMenuItem.Click += new System.EventHandler(this.Close_ToolStripMenuItem_Click);
            // 
            // Info_ToolStripDropDownButton
            // 
            this.Info_ToolStripDropDownButton.AutoToolTip = false;
            this.Info_ToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Info_ToolStripDropDownButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Info_ToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("Info_ToolStripDropDownButton.Image")));
            this.Info_ToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Info_ToolStripDropDownButton.Name = "Info_ToolStripDropDownButton";
            this.Info_ToolStripDropDownButton.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Info_ToolStripDropDownButton.ShowDropDownArrow = false;
            this.Info_ToolStripDropDownButton.Size = new System.Drawing.Size(49, 24);
            this.Info_ToolStripDropDownButton.Text = "Info";
            this.Info_ToolStripDropDownButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Info_ToolStripDropDownButton_MouseUp);
            // 
            // Close_Button
            // 
            this.Close_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Close_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Close_Button.Location = new System.Drawing.Point(330, 480);
            this.Close_Button.Name = "Close_Button";
            this.Close_Button.Size = new System.Drawing.Size(113, 53);
            this.Close_Button.TabIndex = 21;
            this.Close_Button.Text = "Close";
            this.Close_Button.UseVisualStyleBackColor = true;
            this.Close_Button.Click += new System.EventHandler(this.Close_Button_Click);
            // 
            // Launch_Button
            // 
            this.Launch_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Launch_Button.Enabled = false;
            this.Launch_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Launch_Button.Location = new System.Drawing.Point(449, 480);
            this.Launch_Button.Name = "Launch_Button";
            this.Launch_Button.Size = new System.Drawing.Size(113, 53);
            this.Launch_Button.TabIndex = 22;
            this.Launch_Button.Text = "Launch";
            this.Launch_Button.UseVisualStyleBackColor = true;
            this.Launch_Button.Click += new System.EventHandler(this.Launch_Button_Click);
            // 
            // Logo_PictureBox
            // 
            this.Logo_PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Logo_PictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Logo_PictureBox.BackgroundImage")));
            this.Logo_PictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Logo_PictureBox.Location = new System.Drawing.Point(12, 30);
            this.Logo_PictureBox.Name = "Logo_PictureBox";
            this.Logo_PictureBox.Size = new System.Drawing.Size(550, 113);
            this.Logo_PictureBox.TabIndex = 3;
            this.Logo_PictureBox.TabStop = false;
            // 
            // RemoveApp_Button
            // 
            this.RemoveApp_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveApp_Button.Enabled = false;
            this.RemoveApp_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveApp_Button.Location = new System.Drawing.Point(16, 480);
            this.RemoveApp_Button.Name = "RemoveApp_Button";
            this.RemoveApp_Button.Size = new System.Drawing.Size(113, 53);
            this.RemoveApp_Button.TabIndex = 20;
            this.RemoveApp_Button.Text = "Remove App";
            this.RemoveApp_Button.UseVisualStyleBackColor = true;
            this.RemoveApp_Button.Click += new System.EventHandler(this.RemoveApp_Button_Click);
            // 
            // Main_ContextMenuStrip
            // 
            this.Main_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DownloadApp_ToolStripMenuItem,
            this.RemoveApp_ToolStripMenuItem});
            this.Main_ContextMenuStrip.Name = "contextMenuStrip1";
            this.Main_ContextMenuStrip.Size = new System.Drawing.Size(163, 48);
            // 
            // DownloadApp_ToolStripMenuItem
            // 
            this.DownloadApp_ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadApp_ToolStripMenuItem.Image = global::TerrariaDepotDownloader.Properties.Resources.Item_149;
            this.DownloadApp_ToolStripMenuItem.Name = "DownloadApp_ToolStripMenuItem";
            this.DownloadApp_ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.DownloadApp_ToolStripMenuItem.Text = "Download App";
            this.DownloadApp_ToolStripMenuItem.Click += new System.EventHandler(this.DownloadApp_ToolStripMenuItem_Click);
            // 
            // RemoveApp_ToolStripMenuItem
            // 
            this.RemoveApp_ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveApp_ToolStripMenuItem.Image = global::TerrariaDepotDownloader.Properties.Resources.Trash;
            this.RemoveApp_ToolStripMenuItem.Name = "RemoveApp_ToolStripMenuItem";
            this.RemoveApp_ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.RemoveApp_ToolStripMenuItem.Text = "Remove App";
            this.RemoveApp_ToolStripMenuItem.Click += new System.EventHandler(this.RemoveApp_ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(574, 545);
            this.Controls.Add(this.RemoveApp_Button);
            this.Controls.Add(this.Launch_Button);
            this.Controls.Add(this.Close_Button);
            this.Controls.Add(this.Main_TabControl);
            this.Controls.Add(this.Main_ToolStrip);
            this.Controls.Add(this.Logo_PictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(590, 490);
            this.Name = "MainForm";
            this.Text = "Terraria Depot Downloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.Main_TabControl.ResumeLayout(false);
            this.Downloader_TabPage.ResumeLayout(false);
            this.Settings_TabPage.ResumeLayout(false);
            this.Options_GroupBox.ResumeLayout(false);
            this.Options_GroupBox.PerformLayout();
            this.BaseDepotDirectory_GroupBox.ResumeLayout(false);
            this.BaseDepotDirectory_GroupBox.PerformLayout();
            this.SteamLoginInformation_GroupBox.ResumeLayout(false);
            this.SteamLoginInformation_GroupBox.PerformLayout();
            this.Log_TabPage.ResumeLayout(false);
            this.Main_ToolStrip.ResumeLayout(false);
            this.Main_ToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).EndInit();
            this.Main_ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView Main_ListView;
        private System.Windows.Forms.ColumnHeader ManifestID_ColumnHeader;
        private System.Windows.Forms.ColumnHeader Version_ColumnHeader;
        private System.Windows.Forms.TabControl Main_TabControl;
        private System.Windows.Forms.TabPage Downloader_TabPage;
        private System.Windows.Forms.TabPage Settings_TabPage;
        private System.Windows.Forms.TabPage Log_TabPage;
        private System.Windows.Forms.PictureBox Logo_PictureBox;
        private System.Windows.Forms.ToolStrip Main_ToolStrip;
        private System.Windows.Forms.ToolStripDropDownButton Info_ToolStripDropDownButton;
        private System.Windows.Forms.ToolStripDropDownButton File_ToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem Close_ToolStripMenuItem;
        private System.Windows.Forms.Button Close_Button;
        private System.Windows.Forms.Button Launch_Button;
        private System.Windows.Forms.Button ReloadList_Button;
        private System.Windows.Forms.Button ClearLog_Button;
        private System.Windows.Forms.RichTextBox Log_RichTextBox;
        private System.Windows.Forms.ColumnHeader Installed_ColumnHeader;
        private System.Windows.Forms.TextBox BaseDepotDirectory_TextBox;
        private System.Windows.Forms.Button Browse_Button;
        private System.Windows.Forms.Label AccountName_Label;
        private System.Windows.Forms.TextBox AccountName_TextBox;
        private System.Windows.Forms.TextBox Password_TextBox;
        private System.Windows.Forms.Label Password_Label;
        private System.Windows.Forms.Button Show_Button;
        private System.Windows.Forms.Button RemoveAll_Button;
        private System.Windows.Forms.Button RemoveApp_Button;
        private System.Windows.Forms.Button OpenDepots_Button;
        private System.Windows.Forms.CheckBox LogActions_CheckBox;
        private System.Windows.Forms.GroupBox SteamLoginInformation_GroupBox;
        private System.Windows.Forms.GroupBox BaseDepotDirectory_GroupBox;
        private System.Windows.Forms.CheckBox UseSteamDirectory_CheckBox;
        private System.Windows.Forms.ContextMenuStrip Main_ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem DownloadApp_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveApp_ToolStripMenuItem;
        private System.Windows.Forms.CheckBox ShowTooltips_CheckBox;
        private System.Windows.Forms.GroupBox Options_GroupBox;
        private System.Windows.Forms.CheckBox SkipUpdateCheck_CheckBox;
        private System.Windows.Forms.CheckBox RememberLogin_CheckBox;
        private System.Windows.Forms.CheckBox DarkMode_CheckBox;
        private System.Windows.Forms.CheckBox EnableCollectorsEdition_CheckBox;
        private System.Windows.Forms.CheckBox UseSeperateConfigs_CheckBox;
    }
}

