using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Ionic.Zip;
using System;

namespace TerrariaDepotDownloader
{
    public partial class MainForm : Form
    {
        // Say Hello To Decompilers
        private static readonly string HelloThere = "Hello There Fellow Decompiler, This Program Was Made By Discord:dannyruss.";

        #region Form Load

        // Define encryption key. // Uses the user.config file path as a key string.
        readonly string EncryptionKey = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).ToString();

        public MainForm()
        {
            InitializeComponent();
            Console.SetOut(new MultiTextWriter(new ControlWriter(Log_RichTextBox), Console.Out));
        }

        // Toggle the forms darkmode.
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Toggle darkmode.
            if (Properties.Settings.Default.DarkMode)
                DarkMode(true);

            // Update checkboxes.
            DarkMode_CheckBox.Checked = Properties.Settings.Default.DarkMode;
        }

        // Do Loading Events
        public ToolTip Tooltips = new ToolTip();
        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Initiate the log.
            try
            {
                // Open the file in append mode using StreamWriter.
                using (StreamWriter writer = new StreamWriter(Application.StartupPath + @"\Log.txt", true))
                {
                    // Write the character to the file.
                    await writer.WriteLineAsync("[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + "TerrariaDepotDownloader Initiated.");
                    await writer.WriteLineAsync("==================================================");
                }
            }
            catch (Exception)
            { }

            #region Load .NET

            // Verify .NET 8.0 Or Later Exists - Update 1.8.3
            // Verify .NET 9.0 Or Later Exists - Update 1.8.5.6
            var dotnet86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\dotnet\host\fxr";
            var dotnet64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\host\fxr";
            var dotnet86SDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\dotnet\sdk";
            var dotnet64SDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\sdk";

            // Check If A Single Paths Exists
            if (!Directory.Exists(dotnet86) && !Directory.Exists(dotnet64) && !Directory.Exists(dotnet86SDK) && !Directory.Exists(dotnet64SDK))
            {
                // Write error.
                Console.WriteLine(".NET 9.0 Is Required! Please Install And Try Again. \n \n https://dotnet.microsoft.com/download/dotnet/9.0");

                // Display error.
                MessageBox.Show(".NET 9.0 Is Required! Please Install And Try Again. \n \n https://dotnet.microsoft.com/download/dotnet/9.0", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Close Application
                Application.Exit();
            }

            // Check Value
            try
            {
                // Create A List For Versions.
                List<String> versionList = new List<string> { };

                // Combine all paths.
                string[] allPaths = { dotnet86, dotnet64, dotnet86SDK, dotnet64SDK };

                // Iterate through each path.
                foreach (string path in allPaths)
                {
                    if (Directory.Exists(path))
                    {
                        // Add Folder Names To List.
                        foreach (string version in Directory.GetDirectories(path))
                        {
                            // Get the last part of the directory path which is the version number.
                            string versionName = new DirectoryInfo(version).Name;

                            // Remove Any "-" From Name.
                            versionList.Add(versionName.Split('-')[0]);
                        }
                    }
                }

                // Debugging; Show each available .net version.
                // Console.WriteLine(".NET Versions: " + String.Join(", ", versionList.ToArray()));

                // Check If Version Is Above or Equal 8.0.0 // Depot DL 2.6.0 API is now .net 8.0.
                // Check If Version Is Above or Equal 9.0.0 // Depot DL 2.7.4 API is now .net 9.0.
                var maxVersion = versionList.Select(v => Version.Parse(v)).Max();
                if (maxVersion < new Version("9.0.0"))
                {
                    // Log .NET Version
                    Console.WriteLine("ERROR: Highest .NET version found: " + versionList.Max());

                    // Version Not Found
                    MessageBox.Show(".NET 9.0+ is required! Please install and try Again. \n \n https://dotnet.microsoft.com/download/dotnet/9.0 \n \n Highest .NET version found: " + versionList.Max(), "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    // Close Application
                    Application.Exit();
                }
                else
                {
                    // Log .NET Version
                    if (Properties.Settings.Default.LogActions) // Checkbox Not Initilized Yet
                    {
                        Console.WriteLine("Highest .NET version found: " + versionList.Max());
                    }
                }
            }
            catch (Exception)
            {
                // Error, No Updated Manifests
                MessageBox.Show("Unable to check .NET version!", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Close Application
                Application.Exit();
            }
            #endregion

            #region Load Controls

            // Load Steam Textbox Data
            AccountName_TextBox.Text = IsBase64String(Properties.Settings.Default.SteamUser) ? DecryptString(Properties.Settings.Default.SteamUser, EncryptionKey) : ""; // Decrypt username. Return blank if invalid key.
            Password_TextBox.Text = IsBase64String(Properties.Settings.Default.SteamPass)    ? DecryptString(Properties.Settings.Default.SteamPass, EncryptionKey) : ""; // Decrypt password. Return blank if invalid key.

            // Create Depot Folder
            if (!Directory.Exists(Application.StartupPath + @"\TerrariaDepots"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\TerrariaDepots");
                Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
            }
            BaseDepotDirectory_TextBox.Text = Application.StartupPath + @"\TerrariaDepots";

            // Populate Depot Setting Path
            if (Directory.Exists(Application.StartupPath + @"\TerrariaDepots") && Properties.Settings.Default.DepotPath == "")
            {
                // Check If Use Steam Directory Is Enabled
                if (UseSteamDirectory_CheckBox.Checked)
                {
                    // Use Steam Directory Enabled
                    string gameLocation = GetGameLocation();
                    if (gameLocation != "" && gameLocation != "null" && gameLocation != "missing")
                    {
                        // Update install location.
                        Properties.Settings.Default.DepotPath = Directory.GetParent(gameLocation).FullName;
                    }
                    else
                    {
                        // Steam game not found, use startup path instead!
                        UseSteamDirectory_CheckBox.Checked = false;
                        Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";

                        // Log Item
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("ERROR: No game install location was found upon the first load checks!");
                        }
                    }
                }
                else
                {
                    // Use Steam Directory Disabled
                    Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
                }
            }

            // Check if steam location is still valid!
            if (!Properties.Settings.Default.PathChangeEnabled)
            {
                if (GetGameLocation() == "")
                {
                    // Update install location.
                    UseSteamDirectory_CheckBox.Checked = false;
                    Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";

                    // Log Item
                    if (LogActions_CheckBox.Checked)
                    {
                        Console.WriteLine("ERROR: The existing game install location appears to no longer exist!");
                    }
                }
            }

            // Update Depot Path & Create Folder
            BaseDepotDirectory_TextBox.Text = Properties.Settings.Default.DepotPath;
            if (!Directory.Exists(Properties.Settings.Default.DepotPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.DepotPath);
            }

            // Update Checkboxes
            LogActions_CheckBox.Checked = Properties.Settings.Default.LogActions;
            UseSteamDirectory_CheckBox.Checked = Properties.Settings.Default.UseSteamDir;
            ShowTooltips_CheckBox.Checked = Properties.Settings.Default.ToolTips;
            SkipUpdateCheck_CheckBox.Checked = Properties.Settings.Default.SkipUpdate;
            RememberLogin_CheckBox.Checked = Properties.Settings.Default.SaveLogin;
            UseSeparateConfigs_CheckBox.Checked = Properties.Settings.Default.UseSeparateConfigs;

            #endregion

            #region Load Collectors Edition

            // Check if collectors edition is already enabled.

            // Define the registry paths.
            const string keyPath = @"Software\Terraria";
            const string subKeyName = "Bunny";

            // Open the parent key
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                // Check if the parent key exists
                if (key != null)
                {
                    // Check if the subkey exists
                    if (key.GetValue(subKeyName) != null)
                    {
                        // Get the value of the subkey
                        string value = key.GetValue(subKeyName).ToString();

                        // Check if the value matches the expected value
                        if (value == "1")
                        {
                            // Value already exists. Enable checkbox.
                            EnableCollectorsEdition_CheckBox.Checked = true;
                        }
                        else
                        {
                            // Value does not exists. Disable checkbox.
                            EnableCollectorsEdition_CheckBox.Checked = false;
                        }
                    }
                    else
                    {
                        // Value does not exists. Disable checkbox.
                        EnableCollectorsEdition_CheckBox.Checked = false;
                    }
                }
                else
                {
                    // Value does not exists. Disable checkbox.
                    EnableCollectorsEdition_CheckBox.Checked = false;
                }
            }
            #endregion

            #region Load Tooltips

            // Add Tooltips - Update 1.8.5
            Tooltips.InitialDelay = 1000;
            Tooltips.SetToolTip(Close_Button, "Close game and application");
            Tooltips.SetToolTip(Launch_Button, "Download / Launch Terraria version");
            Tooltips.SetToolTip(ReloadList_Button, "Reload all installed versions");
            Tooltips.SetToolTip(ClearLog_Button, "Clear log of all entries");
            Tooltips.SetToolTip(RemoveApp_Button, "Remove selected version");
            Tooltips.SetToolTip(Browse_Button, "Browse for a new install directory");
            Tooltips.SetToolTip(Show_Button, "Temporarily show your password");
            Tooltips.SetToolTip(RemoveAll_Button, "Remove all games from the list");
            Tooltips.SetToolTip(OpenDepots_Button, "Open current base directory");

            Tooltips.SetToolTip(LogActions_CheckBox, "Log all actions to the output log");
            Tooltips.SetToolTip(UseSteamDirectory_CheckBox, "All installs use the Steam directory");
            Tooltips.SetToolTip(ShowTooltips_CheckBox, "Show or hide tooltips");
            Tooltips.SetToolTip(SkipUpdateCheck_CheckBox, "Skip API update check");
            Tooltips.SetToolTip(RememberLogin_CheckBox, "Remember the password and steam key for this user");
            Tooltips.SetToolTip(DarkMode_CheckBox, "Enable or disable the dark mode theme");
            Tooltips.SetToolTip(EnableCollectorsEdition_CheckBox, "Enable or disable the collectors edition");
            Tooltips.SetToolTip(UseSeparateConfigs_CheckBox, "Use a separate config folder for each game version");
           
            // Enable or Disable Tooltips
            if (ShowTooltips_CheckBox.Checked)
            {
                // Enable Tooltips
                Tooltips.Active = true;
            }
            else
            {
                // Disable Tooltips
                Tooltips.Active = false;
            }
            #endregion

            #region Update Buttons

            // Update Buttons
            // button6.Enabled = Properties.Settings.Default.PathChangeEnabled;

            // Create Database File
            if (!File.Exists(Application.StartupPath + @"\ManifestVersions.cfg"))
            {
                File.WriteAllText(Application.StartupPath + @"\ManifestVersions.cfg", "");
                using (StreamWriter streamWriter = new StreamWriter(Application.StartupPath + @"\ManifestVersions.cfg"))
                {
                    streamWriter.WriteLine("Version, Manifest-ID");
                }
            }

            // Make Sure Database Is Populated
            if (File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != null && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != "" && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != "Version, Manifest-ID" && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First().Contains(","))
            {
                // Get Database To List
                List<string> manifests = new List<string>() { };
                var manifestPath = Path.Combine(Application.StartupPath, "ManifestVersions.cfg");
                foreach (var raw in File.ReadLines(manifestPath))
                {
                    var line = raw?.Trim();

                    // Ignore blank/whitespace-only lines (common at EOF due to trailing newline).
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    try
                    {
                        // Check If String Contains "null"
                        if (line.Substring(line.LastIndexOf(' ') + 1) == "null")
                        {
                            // String Contains "null", Add Context
                            Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), "(no manifests database exists)", "N/A" }));
                        }
                        else
                        {
                            // Check If Game Version Folder Exists
                            if (Directory.Exists(Properties.Settings.Default.DepotPath + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ','))))
                            {
                                // String Does Not Contain "null", Record Like Normal
                                Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                            }
                            else
                            {
                                // String Does Not Contain "null", Record Like Normal
                                Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Error, No Updated Manifests
                        MessageBox.Show("ERROR: The manifest file contains an error!", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        // Close Application
                        Application.Exit();
                    }
                }
            }
            else
            {
                // Error, No Updated Manifests
                MessageBox.Show("ERROR: Please update the manifest file!", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Close Application
                Application.Exit();
            }

            // Reload List
            ReloadList();

            #endregion

            #region Check For API Updates

            // Check For Updates
            if (!Properties.Settings.Default.SkipUpdate)
            {
                try
                {
                    // Check GitHub For DepotDownloader Update.
                    Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("DepotDownloader"));
                    var releases = await client.Repository.Release.GetAll("SteamRE", "DepotDownloader");
                    var latestGitHubRelease = releases[0].TagName;

                    // Get Current DepotDownload.dll Version.
                    var currentVersionInfo = FileVersionInfo.GetVersionInfo(Application.StartupPath + @"\DepotDownloader.dll");
                    var currentFileVersion = new Version(currentVersionInfo.FileVersion);
                    var currentFileVersionWithoutBuild = new Version(currentFileVersion.Major, currentFileVersion.Minor, currentFileVersion.Build); // Only consider major, minor, and build. Leave off revision.

                    // Debugging; Compare both versions.
                    // Console.WriteLine(latestGitHubRelease.ToString() + " | " + "DepotDownloader_" + currentFileVersionWithoutBuild.ToString());

                    // Do Version Check
                    if (latestGitHubRelease.ToString() != "DepotDownloader_" + currentFileVersionWithoutBuild.ToString())
                    {
                        // New Version Found
                        // Log Item
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("New DepotDownloader API is available.");
                        }

                        // Ask For Install
                        if (MessageBox.Show("You have an outdated version of the DepotDownloader API" + "\n" + "Do you wish to download and install the update?", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            // Install Update
                            try
                            {
                                // Download From GitHub
                                var latestAsset = await client.Repository.Release.GetAllAssets("SteamRE", "DepotDownloader", releases[0].Id);
                                WebClient webClient = new WebClient();
                                webClient.Headers.Add("user-agent", "Anything");
                                await webClient.DownloadFileTaskAsync(new Uri(latestAsset[0].BrowserDownloadUrl), Application.StartupPath + @"\Update.zip");

                                // Extract ZIP Into Dir
                                using (ZipFile archive = new ZipFile(@"" + Application.StartupPath + @"\Update.zip"))
                                {
                                    archive.ExtractAll(@"" + Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);
                                }

                                // Clean Up Files
                                File.Delete(Application.StartupPath + @"\Update.zip");
                                File.Delete(Application.StartupPath + @"\DepotDownloader.exe");
                                File.Delete(Application.StartupPath + @"\DepotDownloader.pdb");
                                File.Delete(Application.StartupPath + @"\README.md");
                                File.Delete(Application.StartupPath + @"\LICENSE");

                                // Task Finished
                                MessageBox.Show("DepotDownloader API has been download and installed successfully.", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                                // Log Item
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("DepotDownloader API has been download and installed successfully.");
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("ERROR: DepotDownloader API download failed!");
                                return;
                            }
                        }
                        else
                        {
                            // Update Was Declined
                            Console.WriteLine("WARN: DepotDownloader API update was declined.");
                            return;
                        }
                    }
                    else
                    {
                        // No Update Needed
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("DepotDownloader API is up to date.");
                        }
                        return;
                    }
                }
                catch (Exception)
                {
                    // Error Checking Version
                    Console.WriteLine("ERROR: Unable to check DepotDownloader API's GitHub version!");
                    return;
                }
            }
            else
            {
                // Log Event
                if (LogActions_CheckBox.Checked)
                {
                    Console.WriteLine("DepotDownloader API new vision check was skipped!");
                }
            }
            #endregion
        }
        #endregion

        #region Form Controls

        #region Form Buttons

        #region Browse

        // Open Browse Dialogue
        private void Browse_Button_Click(object sender, EventArgs e)
        {
            // Check if steam directory is enabled or not. // Fix 1.8.5.7: Allow people to change their default steam location.
            if (!Properties.Settings.Default.PathChangeEnabled)
            {
                // Conformation Box
                if (MessageBox.Show("Warning! Are you sure you want to force change your default steam location? TerrariaDepotDownloader is supposed to automatically find your games correct location!\n\nThis is only reccomended for advanced users!\nYes or No", "WARNING: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Launch path dialog.
                    using (var fbd = new FolderBrowserDialog())
                    {
                        DialogResult result = fbd.ShowDialog();

                        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            BaseDepotDirectory_TextBox.Text = fbd.SelectedPath;
                            Properties.Settings.Default.DepotPath = fbd.SelectedPath;
                        }
                    }
                }
            }
            else
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        BaseDepotDirectory_TextBox.Text = fbd.SelectedPath;
                        Properties.Settings.Default.DepotPath = fbd.SelectedPath;
                    }
                }
            }
        }
        #endregion

        #region Open Folder Directory

        // Open Depot Folder Directory
        private void OpenDepots_Button_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if use steam directory is enabled. // Fix: v1.8.5.7.
                if (UseSteamDirectory_CheckBox.Checked)
                {
                    // Define folder path.
                    string folderPath = Properties.Settings.Default.DepotPath;

                    if (folderPath.EndsWith("\\Terraria"))
                    {
                        // Remove the last directory from the string.
                        folderPath = folderPath.Replace("\\Terraria", "");

                        // Open steam depot path folder.
                        Process.Start(folderPath);
                    }
                    else
                    {
                        // Log error.
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("Could not find the steam directory. Launching default path instead.");
                        }

                        // Open default depot path folder.
                        Process.Start(Properties.Settings.Default.DepotPath);
                    }
                }
                else
                {
                    // Open default depot path folder.
                    Process.Start(Properties.Settings.Default.DepotPath);
                }

                // Log Action
                if (LogActions_CheckBox.Checked)
                {
                    Console.WriteLine("Opened depot directory.");
                }
            }
            catch (Win32Exception win32Exception)
            {
                // The system cannot find the file specified.
                Console.WriteLine(win32Exception.Message);
            }
        }
        #endregion

        #region Close App

        // Close Games & Application
        private void Close_Button_Click(object sender, EventArgs e)
        {
            // Check For Any Open Clients
            if (Process.GetProcessesByName("Terraria").Length > 0)
            {
                // Is running
                foreach (var process in Process.GetProcessesByName("Terraria"))
                {
                    process.Kill();
                }
            }

            // Gather Steam Data
            Properties.Settings.Default.SteamUser = EncryptString(AccountName_TextBox.Text, EncryptionKey); // Encrypt username.
            Properties.Settings.Default.SteamPass = EncryptString(Password_TextBox.Text, EncryptionKey);    // Encrypt password.

            // Save Settings
            Properties.Settings.Default.Save();

            // Close Application
            Application.Exit();
        }
        #endregion

        #region Clear Log

        // Clear Log
        private void ClearLog_Button_Click(object sender, EventArgs e)
        {
            Log_RichTextBox.Clear();
            Log_RichTextBox.Update();
        }
        #endregion

        #region Show / Hide Password

        // Show Password
        private void Show_Button_MouseDown(object sender, MouseEventArgs e)
        {
            Password_TextBox.PasswordChar = '\u0000';
        }

        // Hide Password
        private void Show_Button_MouseUp(object sender, MouseEventArgs e)
        {
            Password_TextBox.PasswordChar = '*';
        }
        #endregion

        #endregion

        #region Form Closing

        // Form Closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Gather Steam Data
            Properties.Settings.Default.SteamUser = EncryptString(AccountName_TextBox.Text, EncryptionKey); // Encrypt username.
            Properties.Settings.Default.SteamPass = EncryptString(Password_TextBox.Text, EncryptionKey);    // Encrypt password.

            // Save Settings
            Properties.Settings.Default.Save();

            // Close Application
            Application.Exit();
        }
        #endregion

        #region ToolStrip / ListView Controls

        // Close Via ToolStrip
        private void Close_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Gather Steam Data
            Properties.Settings.Default.SteamUser = EncryptString(AccountName_TextBox.Text, EncryptionKey); // Encrypt username.
            Properties.Settings.Default.SteamPass = EncryptString(Password_TextBox.Text, EncryptionKey);    // Encrypt password.

            // Save Settings
            Properties.Settings.Default.Save();

            // Close Application
            Application.Exit();
        }

        // Open Info Tab
        private void Info_ToolStripDropDownButton_MouseUp(object sender, MouseEventArgs e)
        {
            // Open New Form2
            About frm2 = new About();
            frm2.ShowDialog();
        }

        // Open Context Menu
        private void Main_ListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = Main_ListView.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    Main_ContextMenuStrip.Show(Cursor.Position);
                }
            }
        }
        #endregion

        #region Logging

        // Auto Scroll To End
        private void Log_RichTextBox_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            Log_RichTextBox.SelectionStart = Log_RichTextBox.Text.Length;

            // scroll it automatically
            Log_RichTextBox.ScrollToCaret();
        }
        #endregion

        #endregion

        #region Main

        #region Reload List

        private bool DownloaderIsBusy = false;

        // Reload List
        private void ReloadList_Button_Click(object sender, EventArgs e)
        {
            ReloadList();
            if (LogActions_CheckBox.Checked)
            {
                Console.WriteLine("App list reloaded");
            }
        }

        public void ReloadList()
        {
            // Clear ListView
            Main_ListView.Items.Clear();
            Main_ListView.Refresh();

            // Check If Directory Contains A ChangeLog If Use Steam Directory Is Enabled
            if (UseSteamDirectory_CheckBox.Checked)
            {
                // Ensure directory exists.
                if (!Directory.Exists(Properties.Settings.Default.DepotPath + @"\Terraria"))
                {
                    Directory.CreateDirectory(Properties.Settings.Default.DepotPath + @"\Terraria");
                    Console.WriteLine("Terraria steam directory not found! Generating a new one.");
                }

                // Check If Directory Contains A ChangeLog
                if (!File.Exists(Properties.Settings.Default.DepotPath + @"\Terraria\changelog.txt"))
                {
                    File.WriteAllText(Properties.Settings.Default.DepotPath + @"\Terraria\changelog.txt", "Empty Changelog!");
                    Console.WriteLine("No changelog file found in steam directory!");
                }
            }

            // Create Database File
            if (!File.Exists(Application.StartupPath + @"\ManifestVersions.cfg"))
            {
                File.WriteAllText(Application.StartupPath + @"\ManifestVersions.cfg", "");
                using (StreamWriter streamWriter = new StreamWriter(Application.StartupPath + @"\ManifestVersions.cfg"))
                {
                    streamWriter.WriteLine("Version, Manifest-ID");
                }
            }

            // Reset Controls
            Launch_Button.Text = "Download";
            Launch_Button.Enabled = false; // Fix 1.8.5.5.
            RemoveApp_Button.Enabled = false;

            // Make Sure Database Is Populated
            if (File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != null && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != "" && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != "Version, Manifest-ID" && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First().Contains(","))
            {
                // Get Database To List
                List<string> manifests = new List<string>() { };
                var manifestPath = Path.Combine(Application.StartupPath, "ManifestVersions.cfg");
                foreach (var raw in File.ReadLines(manifestPath))
                {
                    var line = raw?.Trim();

                    // Ignore blank/whitespace-only lines (common at EOF due to trailing newline).
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        // Check If String Contains "null"
                        if (line.Substring(line.LastIndexOf(' ') + 1) == "null")
                        {
                            // String Contains "null", Add Context
                            Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), "(no manifests database exists)", "N/A" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                        }
                        else
                        {
                            // Check If Use Steam Directory Is Enabled
                            if (UseSteamDirectory_CheckBox.Checked)
                            {
                                var versionDir = Path.Combine(Properties.Settings.Default.DepotPath, "Terraria-v" + String.Concat(line.TakeWhile(c => c != ',')));
                                var genericDir = Path.Combine(Properties.Settings.Default.DepotPath, "Terraria");

                                string dirToInspect = null;
                                if (Directory.Exists(versionDir))
                                    dirToInspect = versionDir;
                                else if (Directory.Exists(genericDir))
                                    dirToInspect = genericDir;

                                // Check if the folder exists in any form.
                                if (dirToInspect != null)
                                {
                                    #region Version Folder

                                    // Check for version folder and if its populated.
                                    if (Directory.Exists(versionDir))
                                    {
                                        // If the downloader is busy, also treat this entree like a valid record (do not delete this directory).
                                        if (Directory.EnumerateFileSystemEntries(versionDir).Any() || DownloaderIsBusy)
                                        {
                                            // Valid record, record like normal.
                                            Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                        }
                                        else
                                        {
                                            // This folder contains no files, delete it.
                                            Directory.Delete(dirToInspect, true);

                                            // Log Item
                                            if (LogActions_CheckBox.Checked)
                                            {
                                                Console.WriteLine("Removed empty folder: " + dirToInspect);
                                            }

                                            // Invalid record, record like normal.
                                            Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                        }
                                        continue;
                                    }
                                    #endregion

                                    #region Generic Folder

                                    // Check for generic folder and if its populated.
                                    if (Directory.Exists(genericDir))
                                    {
                                        // If the downloader is busy, also treat this entree like a valid record (do not delete this directory).
                                        if (Directory.EnumerateFileSystemEntries(genericDir).Any() || DownloaderIsBusy)
                                        {
                                            // Read the version from changelog.
                                            if (File.ReadLines(genericDir + @"\changelog.txt").First().Split(' ')[1].ToString() == String.Concat(line.TakeWhile(c => c != ',')) ||
                                            File.ReadLines(genericDir + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.3" && String.Concat(line.TakeWhile(c => c != ',')) == "1.3.0.1" ||
                                            File.ReadLines(genericDir + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.4" && String.Concat(line.TakeWhile(c => c != ',')) == "1.4.0.1")
                                            {
                                                // Valid record, record like normal.
                                                Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                            }
                                            else
                                            {
                                                // Invalid record, record like normal.
                                                Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                            }
                                        }
                                        else
                                        {
                                            // This folder contains no files, delete it.
                                            Directory.Delete(dirToInspect, true);

                                            // Log Item
                                            if (LogActions_CheckBox.Checked)
                                            {
                                                Console.WriteLine("Removed empty folder: " + dirToInspect);
                                            }

                                            // Invalid record, record like normal.
                                            Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                        }
                                        continue;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    // Version does not exist what so ever, record like normal.
                                    Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                }
                            }
                            else
                            {
                                var versionDir = Path.Combine(Properties.Settings.Default.DepotPath, "Terraria-v" + String.Concat(line.TakeWhile(c => c != ',')));

                                #region Non-Steam Version Folder

                                // Check for version folder.
                                if (Directory.Exists(versionDir))
                                {
                                    // Check if its populated.
                                    // If the downloader is busy, also treat this entree like a valid record (do not delete this directory).
                                    if (Directory.EnumerateFileSystemEntries(versionDir).Any() || DownloaderIsBusy)
                                    {
                                        // String Does Not Contain "null", Record Like Normal
                                        Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                    }
                                    else
                                    {
                                        // This folder contains no files, delete it.
                                        Directory.Delete(versionDir, true);

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Removed empty folder: " + versionDir);
                                        }

                                        // Invalid record, record like normal.
                                        Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                    }
                                }
                                else
                                {
                                    // Version folder does not exist, record like normal.
                                    Main_ListView.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Error, No Updated Manifests
                        MessageBox.Show("ERROR: The manifest file contains an error!\n\n" + ex.ToString(), "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }
            else
            {
                // Error, No Updated Manifests
                MessageBox.Show("ERROR: Please update the manifest file!", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }
        #endregion

        #region Remove Apps

        #region Remove App

        // Remove App
        private void RemoveApp_Button_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.Main_ListView.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Check If Client Is Currently Running - Update 1.8.3
                        // Check if use steam directory.
                        if (!UseSteamDirectory_CheckBox.Checked)
                        {
                            bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                            if (isRunning)
                            {
                                // Is running
                                foreach (var process in Process.GetProcessesByName("Terraria"))
                                {
                                    process.Kill();

                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("The Terraria process was killed to continue operations.");
                                    }
                                }
                            }
                        }

                        // Check if use steam directory.
                        if (UseSteamDirectory_CheckBox.Checked)
                        {
                            // Get the parent directory.
                            string OutDirParent = Properties.Settings.Default.DepotPath;

                            // Check if directory exists, if not, its the current version.
                            if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                            {
                                // Exists, delete it.
                                Directory.Delete(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                                // Log Item
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("Removed: " + OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                }
                            }
                            // Validate the version within the Terraria directory.
                            else
                            {
                                // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                string currentVersion = File.ReadLines(OutDirParent + @"\Terraria" + @"\changelog.txt").First().Split(' ')[1].ToString();
                                currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                // Confirm the version within the Terraria folder is the one selected.
                                if (itemRow.SubItems[0].Text == currentVersion)
                                {
                                    // Show Warning
                                    if (MessageBox.Show("This version is currently active." + "\n\n" + "Terraria-v" + itemRow.SubItems[0].Text + "\n\n" + "Do you want to continue with removal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        // Exists, delete it.
                                        Directory.Delete(OutDirParent + @"\Terraria", true);

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Removed: " + OutDirParent + @"\Terraria : v" + itemRow.SubItems[0].Text);
                                        }
                                    }
                                    else
                                    {
                                        // User canceled.
                                        //
                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Active steam directory version removal canceled. : v" + itemRow.SubItems[0].Text);
                                        }
                                    }
                                }
                                else
                                {
                                    // Does not exist, log it.
                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("ERROR: Version mix-match! The currently active version: " + currentVersion + "is different from the selected Terraria-v" + itemRow.SubItems[0].Text + ".");
                                    }

                                    // Display error.
                                    MessageBox.Show("ERROR: Version mix-match! The currently active version: " + currentVersion + "is different from the selected Terraria-v" + itemRow.SubItems[0].Text + ".", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                        else
                        {
                            // Delete Folder
                            Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                            // Log Item
                            if (LogActions_CheckBox.Checked)
                            {
                                Console.WriteLine("Removed: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text);
                            }
                        }

                        // Update Forum
                        ReloadList();
                    }
                }
            }

            // Edit Button
            RemoveApp_Button.Enabled = false;
        }
        #endregion

        #region Remove Via ToolStrip

        // Remove App Tool Via ToolStrip
        private void RemoveApp_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.Main_ListView.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Check If Client Is Currently Running - Update 1.8.3
                        // Check if use steam directory.
                        if (!UseSteamDirectory_CheckBox.Checked)
                        {
                            bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                            if (isRunning)
                            {
                                // Is running
                                foreach (var process in Process.GetProcessesByName("Terraria"))
                                {
                                    process.Kill();

                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("The Terraria process was killed to continue operations.");
                                    }
                                }
                            }
                        }

                        // Check if use steam directory.
                        if (UseSteamDirectory_CheckBox.Checked)
                        {
                            // Get the parent directory.
                            string OutDirParent = Properties.Settings.Default.DepotPath;

                            // Check if directory exists, if not, its the current version.
                            if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                            {
                                // Exists, delete it.
                                Directory.Delete(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                                // Log Item
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("Removed: " + OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                }
                            }
                            // Validate the version within the Terraria directory.
                            else
                            {
                                // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                string currentVersion = File.ReadLines(OutDirParent + @"\Terraria" + @"\changelog.txt").First().Split(' ')[1].ToString();
                                currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                // Confirm the version within the Terraria folder is the one selected.
                                if (itemRow.SubItems[0].Text == currentVersion)
                                {
                                    // Show Warning
                                    if (MessageBox.Show("This version is currently active." + "\n\n" + "Terraria-v" + itemRow.SubItems[0].Text + "\n\n" + "Do you want to continue with removal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        // Exists, delete it.
                                        Directory.Delete(OutDirParent + @"\Terraria", true);

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Removed: " + OutDirParent + @"\Terraria : v" + itemRow.SubItems[0].Text);
                                        }
                                    }
                                    else
                                    {
                                        // User canceled.
                                        //
                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Active steam directory version removal canceled. : v" + itemRow.SubItems[0].Text);
                                        }
                                    }
                                }
                                else
                                {
                                    // Does not exist, log it.
                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("ERROR: Version mix-match! The currently active version: " + currentVersion + "is different from the selected Terraria-v" + itemRow.SubItems[0].Text + ".");
                                    }

                                    // Display error.
                                    MessageBox.Show("ERROR: Version mix-match! The currently active version: " + currentVersion + "is different from the selected Terraria-v" + itemRow.SubItems[0].Text + ".", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                        else
                        {
                            // Delete Folder
                            Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                            // Log Item
                            if (LogActions_CheckBox.Checked)
                            {
                                Console.WriteLine("Removed: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text);
                            }
                        }

                        // Update Forum
                        ReloadList();
                    }
                }
            }

            // Edit Button
            RemoveApp_Button.Enabled = false;
        }
        #endregion

        #region Remove All

        // Remove All Games
        private void RemoveAll_Button_Click(object sender, EventArgs e)
        {
            // Check For Any Open Clients - Update 1.8.3
            // Check if use steam directory.
            if (!UseSteamDirectory_CheckBox.Checked)
            {
                if (Process.GetProcessesByName("Terraria").Length > 0)
                {
                    // Is running
                    foreach (var process in Process.GetProcessesByName("Terraria"))
                    {
                        process.Kill();

                        // Log Item
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("Running game process was found and terminated.");
                        }
                    }
                }
            }

            // Conformation Box
            if (MessageBox.Show("Remove All Games?\nYes or No", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                // Get Each Row
                foreach (ListViewItem itemRow in this.Main_ListView.Items)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Check if use steam directory.
                        if (UseSteamDirectory_CheckBox.Checked)
                        {
                            // Get the parent directory.
                            string OutDirParent = Properties.Settings.Default.DepotPath;

                            // Check if directory exists, if not, its the current version.
                            if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                            {
                                // Exists, delete it.
                                Directory.Delete(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                                // Log Item
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("Removed: " + OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                }
                            }
                            // Validate the version within the Terraria directory.
                            else if (Directory.Exists(OutDirParent + @"\Terraria"))
                            {
                                // Exists, delete it.
                                Directory.Delete(OutDirParent + @"\Terraria", true);

                                // Log Item
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("Removed: " + OutDirParent + @"\Terraria : v" + itemRow.SubItems[0].Text);
                                }
                            }
                        }
                        else
                        {
                            // Delete Folder
                            Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                            // Log Item
                            if (LogActions_CheckBox.Checked)
                            {
                                Console.WriteLine("Removed: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text);
                            }
                        }
                    }
                }
                // Update Forum
                ReloadList();

                // Log Item
                if (LogActions_CheckBox.Checked)
                {
                    Console.WriteLine("All apps removed");
                }
            }
        }
        #endregion

        #endregion

        #region Dark Mode

        // Set dark mode.
        public void DarkMode(bool enable)
        {
            if (enable)
            {
                // Color pallets.
                var formBackColor = Color.FromArgb(35, 36, 40);        // Mid-Dark.
                var tabControlBackColor = Color.FromArgb(43, 45, 49);  // Lightest.
                var controlBackColor = Color.FromArgb(56, 58, 64);     // Darkest.
                var controlForeColor = Color.White;                    // White.
                var listViewGridColor = Color.LightGray;               // LightGray.

                // Turn on dark mode for main form.
                MainForm.ActiveForm.BackColor = formBackColor;
                MainForm.ActiveForm.ForeColor = controlForeColor;

                // Turn on dark mode for remaining controls.
                foreach (Control component in this.Controls)
                {
                    // Check if component is a picturebox.
                    if (component is PictureBox)
                    {
                        Logo_PictureBox.BackColor = formBackColor;
                    }
                    else
                    {
                        // All other controls.
                        component.BackColor = controlBackColor;
                        component.ForeColor = controlForeColor;
                    }
                }

                // Recolor each tabpage.
                for (int a = 0; a < Main_TabControl.TabPages.Count; a++)
                {
                    Main_TabControl.TabPages[a].BackColor = tabControlBackColor;
                    Main_TabControl.TabPages[a].ForeColor = controlForeColor;
                }

                // Turn on dark mode for remaining controls.
                foreach (TabPage tab in Main_TabControl.TabPages)
                    foreach (Control component in tab.Controls)
                    {
                        // Change groupbox controls.
                        component.BackColor = tabControlBackColor;

                        // Change listview text control containers.
                        if (component is ListView)
                            component.ForeColor = listViewGridColor;
                        else
                            component.ForeColor = controlForeColor;

                        // Change textbox control containers.
                        foreach (TextBox textBox in component.Controls.OfType<TextBox>().ToList())
                        {
                            textBox.BackColor = controlBackColor;
                            textBox.ForeColor = controlForeColor;
                        }

                        // Change button control containers.
                        foreach (Button button in component.Controls.OfType<Button>().ToList())
                        {
                            button.BackColor = controlBackColor;
                            button.ForeColor = controlForeColor;
                        }
                    }
            }
            else
            {
                // Turn off dark mode for main form.
                MainForm.ActiveForm.BackColor = DefaultBackColor;
                MainForm.ActiveForm.ForeColor = DefaultForeColor;

                // Turn off dark mode for remaining controls.
                foreach (Control component in MainForm.ActiveForm.Controls)
                {
                    component.BackColor = DefaultBackColor;
                    component.ForeColor = DefaultForeColor;

                    if (component is Button && !component.Enabled)
                    {
                        component.BackColor = SystemColors.ScrollBar;
                    }

                    component.Invalidate();
                }

                // Recolor each tabpage.
                for (int a = 0; a < Main_TabControl.TabPages.Count; a++)
                {
                    Main_TabControl.TabPages[a].BackColor = DefaultBackColor;
                    Main_TabControl.TabPages[a].ForeColor = DefaultForeColor;
                }

                // Turn on dark mode for remaining controls.
                foreach (TabPage tab in Main_TabControl.TabPages)
                    foreach (Control component in tab.Controls)
                    {
                        // Change groupbox controls.
                        component.BackColor = DefaultBackColor;
                        component.ForeColor = DefaultForeColor;

                        // Change textbox control containers.
                        foreach (TextBox textBox in component.Controls.OfType<TextBox>().ToList())
                        {
                            textBox.BackColor = DefaultBackColor;
                            textBox.ForeColor = DefaultForeColor;
                        }

                        // Change textbox control containers.
                        foreach (Button button in component.Controls.OfType<Button>().ToList())
                        {
                            button.BackColor = DefaultBackColor;
                            button.ForeColor = DefaultForeColor;
                        }

                        component.Invalidate();
                    }
            }
        }
        #endregion

        #region Launch Button Updater

        // Update Button
        private void Main_ListView_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.Main_ListView.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Edit Launch Button
                        Launch_Button.Enabled = true;
                        Launch_Button.Text = "Launch";

                        // Edit Remove Button
                        RemoveApp_Button.Enabled = true;
                    }
                    else if (itemRow.SubItems[2].Text == "No")
                    {
                        // Edit Launch Button
                        Launch_Button.Enabled = true;
                        Launch_Button.Text = "Download";

                        // Edit Remove Button
                        RemoveApp_Button.Enabled = false;
                    }
                    else if (itemRow.SubItems[2].Text == "N/A")
                    {
                        Launch_Button.Text = "N/A";
                        Launch_Button.Enabled = false;

                        // Edit Remove Button // Fix 1.8.5.7.
                        RemoveApp_Button.Enabled = false;
                    }
                }
            }
        }
        #endregion

        #region Helpers

        // Function for renaming active folders.
        public async Task RenameFolderAsync(string sourcePath, string targetPath)
        {
            // Get files in sourcePath and group them by directory.
            var files = Directory.EnumerateFiles(sourcePath, "*", System.IO.SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));

            // Iterate through each directory group.
            foreach (var folder in files)
            {
                // Create target folder based on the corresponding source folder.
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);

                // Iterate through each file in the current directory.
                foreach (var file in folder)
                {
                    // Generate target file path.
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));

                    // If target file exists, delete it.
                    if (File.Exists(targetFile)) File.Delete(targetFile);

                    // Move file asynchronously.
                    await MoveFileAsync(file, targetFile).ConfigureAwait(false);
                }
            }
        }

        // Method to move file asynchronously.
        private async Task MoveFileAsync(string sourceFile, string targetFile)
        {
            // Open source file stream.
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                // Create or overwrite target file.
                using (var targetStream = new FileStream(targetFile, FileMode.CreateNew))
                {
                    // Asynchronously copy source file contents to target file.
                    await sourceStream.CopyToAsync(targetStream).ConfigureAwait(false);
                }
            }

            // Delete source file after copying is done.
            File.Delete(sourceFile);
        }

        // Get game location.
        public string GetGameLocation()
        {
            using (var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                string subKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 105600";
                using (var key = root.OpenSubKey(subKey)) // False is important!
                {
                    // Ensure the key exists. - Added 1.8.5.7.
                    if (key == null)
                        return "null";

                    string s = key.GetValue("InstallLocation") as string;
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        // Define path; should be the games initial install directory.
                        return s; // + @"\Terraria";
                    }
                    else
                        return "missing";
                }
            }
        }
        #endregion

        #endregion

        #region Launch / Download

        #region Launch Button

        // Launch Button
        private async void Launch_Button_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.Main_ListView.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check Options
                    if (Launch_Button.Text == "Launch")
                    {
                        // Launch App
                        //
                        // Check If Use Steam Directory Is Enabled
                        if (UseSteamDirectory_CheckBox.Checked)
                        {
                            try
                            {
                                // Get the correct directory and move it to "Terraria".
                                string OutDir = Properties.Settings.Default.DepotPath;
                                string OutDirParent = OutDir; // Directory.GetParent(OutDir).ToString();

                                // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                string currentVersion = File.ReadLines(OutDir + @"\Terraria\changelog.txt").First().Split(' ')[1].ToString();
                                currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                // Check if version is different from the selected.
                                if (currentVersion != itemRow.SubItems[0].Text)
                                {
                                    try
                                    {
                                        // Check if the existing game version already exists or not.
                                        // Fix 1.8.5.8: Prevent empty game folders from being backed up. 
                                        if (!Directory.Exists(OutDirParent + @"\Terraria-v" + currentVersion) && currentVersion != "Changelog!")
                                        {
                                            // Move to a backup.
                                            DirectoryInfo dir = new DirectoryInfo(OutDir + @"\Terraria");
                                            dir.MoveTo(OutDirParent + @"\Terraria-v" + currentVersion);
                                            Directory.Delete(OutDir + @"\Terraria", true); // Encase any files where left behind.
                                        }
                                        else
                                        {
                                            // This version already exists, delete parent.
                                            Directory.Delete(OutDir + @"\Terraria", true);
                                        }
                                    }
                                    catch (Exception) { }

                                    // Grab and move desired version.
                                    try
                                    {
                                        // Rename target version to "Terraria".
                                        DirectoryInfo dir = new DirectoryInfo(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                        dir.MoveTo(OutDir + @"\Terraria");
                                        Directory.Delete(OutDirParent + @"\Terraria -v" + itemRow.SubItems[0].Text, true); // Encase any files where left behind.
                                    }
                                    catch (Exception) { }
                                }

                                #region Load Separate Configurations

                                // Define config directory path.
                                string configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games";
                                string lastGameVersion = "0.0.0.0";

                                // Read the last loaded configuration version.
                                if (File.Exists(configPath + @"\TerrariaDepotDownloaderData.txt"))
                                {
                                    // Open the file for reading
                                    using (StreamReader reader = new StreamReader(configPath + @"\TerrariaDepotDownloaderData.txt"))
                                    {
                                        // Read the first line.
                                        lastGameVersion = reader.ReadLine();

                                        // Close the reader.
                                        reader.Close();
                                    }
                                }
                                else
                                {
                                    // No data file found. Create new file.
                                    await Task.Run(() => File.Create(configPath + @"\TerrariaDepotDownloaderData.txt"));
                                }

                                // Check if checkbox was checked or not.
                                if (UseSeparateConfigs_CheckBox.Checked)
                                {
                                    // Switch to the defined games config.
                                    try
                                    {
                                        // Check if default folder exists or not already.
                                        if (!Directory.Exists(configPath + @"\Terraria-Original"))
                                        {
                                            // Ensure an original even exists.
                                            if (Directory.Exists(configPath + @"\Terraria"))
                                            {
                                                // Async file renaming. // Backup the original file.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-Original"));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));

                                                // Async create directory.
                                                await Task.Run(() => Directory.CreateDirectory(configPath + @"\Terraria"));
                                            }
                                        }

                                        // Original was already backed up. Lets switch versions.

                                        // Ensure the desired config already exist.
                                        if (Directory.Exists(configPath + @"\Terraria-v" + itemRow.SubItems[0].Text))
                                        {
                                            // Ensure this is not a first time config load.
                                            if (lastGameVersion != "0.0.0.0")
                                            {
                                                // Rename the existing config to its previous version.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-v" + lastGameVersion));
                                            }

                                            // Async delete directory.
                                            await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));

                                            // Now rename the desired version to Terraria.
                                            await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria-v" + itemRow.SubItems[0].Text, configPath + @"\Terraria"));

                                            // Async delete directory.
                                            await Task.Run(() => Directory.Delete(configPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true));
                                        }
                                        else
                                        {
                                            // No configuration for this version found. Create new directory.

                                            // If desired config is different then last config, backup directory.
                                            if (lastGameVersion != itemRow.SubItems[0].Text && lastGameVersion != "0.0.0.0")
                                            {
                                                // Rename the existing config to its previous version.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-v" + lastGameVersion));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));
                                            }

                                            // Async create directory.
                                            await Task.Run(() => Directory.CreateDirectory(configPath + @"\Terraria"));
                                        }

                                        // Log last config loaded.
                                        using (StreamWriter writer = new StreamWriter(configPath + @"\TerrariaDepotDownloaderData.txt", false))
                                        {
                                            // Write the last loaded game version to file.
                                            writer.WriteLine(itemRow.SubItems[0].Text);

                                            // Close the writer.
                                            writer.Close();
                                        }

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            // If last config was 0.0.0.0 state it as the original. // Dont log switching from and to same versions.
                                            if (lastGameVersion != itemRow.SubItems[0].Text)
                                                Console.WriteLine("Switched game config from Terraria" + ((lastGameVersion != "0.0.0.0") ? "-v" + lastGameVersion : "-Original") + " to Terraria-v" + itemRow.SubItems[0].Text + "!");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // Log Item
                                        Console.WriteLine("Failed to switch the game configs!");
                                    }
                                }
                                #endregion

                                // Start Terraria Though Steam
                                Process.Start("steam://rungameid/105600");

                                // Do logging If Enabled
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("Successfully launched Terraria v" + File.ReadLines(Properties.Settings.Default.DepotPath + @"\Terraria\changelog.txt").First().Split(' ')[1].ToString() + " Through Steam!");
                                }
                            }
                            catch (Exception error)
                            {
                                Console.WriteLine("Failed to launch Terraria v" + File.ReadLines(Properties.Settings.Default.DepotPath + @"\Terraria\changelog.txt").First().Split(' ')[1].ToString() + ": " + error.Message.ToString());
                            }
                        }
                        else
                        {
                            try
                            {
                                #region Load Separate Configurations

                                // Define config directory path.
                                string configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games";
                                string lastGameVersion = "0.0.0.0";

                                // Read the last loaded configuration version.
                                if (File.Exists(configPath + @"\TerrariaDepotDownloaderData.txt"))
                                {
                                    // Open the file for reading
                                    using (StreamReader reader = new StreamReader(configPath + @"\TerrariaDepotDownloaderData.txt"))
                                    {
                                        // Read the first line.
                                        lastGameVersion = reader.ReadLine();

                                        // Close the reader.
                                        reader.Close();
                                    }
                                }
                                else
                                {
                                    // No data file found. Create new file.
                                    await Task.Run(() => File.Create(configPath + @"\TerrariaDepotDownloaderData.txt"));
                                }

                                // Check if checkbox was checked or not.
                                if (UseSeparateConfigs_CheckBox.Checked)
                                {
                                    // Switch to the defined games config.
                                    try
                                    {
                                        // Check if default folder exists or not already.
                                        if (!Directory.Exists(configPath + @"\Terraria-Original"))
                                        {
                                            // Ensure an original even exists.
                                            if (Directory.Exists(configPath + @"\Terraria"))
                                            {
                                                // Async file renaming. // Backup the original file.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-Original"));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));

                                                // Async create directory.
                                                await Task.Run(() => Directory.CreateDirectory(configPath + @"\Terraria"));
                                            }
                                        }

                                        // Original was already backed up. Lets switch versions.

                                        // Ensure the desired config already exist.
                                        if (Directory.Exists(configPath + @"\Terraria-v" + itemRow.SubItems[0].Text))
                                        {
                                            // Ensure this is not a first time config load.
                                            if (lastGameVersion != "0.0.0.0")
                                            {
                                                // Rename the existing config to its previous version.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-v" + lastGameVersion));
                                            }

                                            // Async delete directory.
                                            await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));

                                            // Now rename the desired version to Terraria.
                                            await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria-v" + itemRow.SubItems[0].Text, configPath + @"\Terraria"));

                                            // Async delete directory.
                                            await Task.Run(() => Directory.Delete(configPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true));
                                        }
                                        else
                                        {
                                            // No configuration for this version found. Create new directory.

                                            // If desired config is different then last config, backup directory.
                                            if (lastGameVersion != itemRow.SubItems[0].Text && lastGameVersion != "0.0.0.0")
                                            {
                                                // Rename the existing config to its previous version.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-v" + lastGameVersion));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));
                                            }

                                            // Async create directory.
                                            await Task.Run(() => Directory.CreateDirectory(configPath + @"\Terraria"));
                                        }

                                        // Log last config loaded.
                                        using (StreamWriter writer = new StreamWriter(configPath + @"\TerrariaDepotDownloaderData.txt", false))
                                        {
                                            // Write the last loaded game version to file.
                                            writer.WriteLine(itemRow.SubItems[0].Text);

                                            // Close the writer.
                                            writer.Close();
                                        }

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            // If last config was 0.0.0.0 state it as the original. // Dont log switching from and to same versions.
                                            if (lastGameVersion != itemRow.SubItems[0].Text)
                                                Console.WriteLine("Switched game config from Terraria" + ((lastGameVersion != "0.0.0.0") ? "-v" + lastGameVersion : "-Original") + " to Terraria-v" + itemRow.SubItems[0].Text + "!");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // Log Item
                                        Console.WriteLine("Failed to switch the game configs!");
                                    }
                                }
                                #endregion

                                // Start Terraria By File
                                Process startPath = new Process();
                                startPath.StartInfo.WorkingDirectory = Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text;
                                startPath.StartInfo.FileName = Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text + @"\Terraria.exe";
                                startPath.Start();

                                // Do Logging If Enabled
                                if (LogActions_CheckBox.Checked)
                                {
                                    Console.WriteLine("Successfully launched Terraria v" + itemRow.SubItems[0].Text);
                                }
                            }
                            catch (Exception error)
                            {
                                // Log Item
                                Console.WriteLine("Failed to launch Terraria v" + itemRow.SubItems[0].Text + ": " + error.Message.ToString());
                            }
                        }
                        // Disable Button
                        Launch_Button.Enabled = false;
                    }
                    else if (Launch_Button.Text == "Download")
                    {
                        // Check If User & Pass Are Populated
                        if (AccountName_TextBox.Text != "" && Password_TextBox.Text != "")
                        {
                            // Check If Already Downloaded
                            if (itemRow.SubItems[2].Text == "No")
                            {
                                // Disable Button
                                Launch_Button.Enabled = false;

                                // Select Tab Control
                                // tabControl1.SelectedIndex = 2;

                                // Download Version
                                String DLLLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\DepotDownloader.dll";
                                String DotNetLocation = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\dotnet.exe";
                                // Update 1.5.0, Check If Overwrite To Steam Directory Is Enabled
                                String OutDir = Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text;

                                // Use Steam Directory.
                                if (UseSteamDirectory_CheckBox.Checked)
                                {
                                    OutDir = Properties.Settings.Default.DepotPath;
                                    string OutDirParent = OutDir; // Directory.GetParent(OutDir).ToString();

                                    // Check If Client Is Already Running - Update 1.8.3
                                    bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(OutDir, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                                    if (isRunning)
                                    {
                                        // Is running
                                        foreach (var process in Process.GetProcessesByName("Terraria"))
                                        {
                                            process.Kill();

                                            // Log Item
                                            if (LogActions_CheckBox.Checked)
                                            {
                                                Console.WriteLine("The Terraria process was killed to continue operations.");
                                            }
                                        }
                                    }

                                    // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                    string currentVersion = File.ReadLines(OutDir + @"\Terraria\changelog.txt").First().Split(' ')[1].ToString();
                                    currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                    // Delete Folder
                                    try
                                    {
                                        // Ensure the target directory does not already exist.
                                        // Fix 1.8.5.8: Prevent empty game folders from being backed up. 
                                        if (!Directory.Exists(OutDirParent + @"\Terraria-v" + currentVersion) && currentVersion != "Changelog!")
                                        {
                                            DirectoryInfo dir = new DirectoryInfo(OutDir + @"\Terraria");
                                            dir.MoveTo(OutDirParent + @"\Terraria-v" + currentVersion);
                                            Directory.Delete(OutDir + @"\Terraria", true); // Encase any files where left behind.
                                        }
                                        else
                                        {
                                            // This version already exists, delete parent.
                                            Directory.Delete(OutDir + @"\Terraria", true);
                                        }
                                    }
                                    catch (Exception) { }
                                    Directory.CreateDirectory(OutDir + @"\Terraria"); // Update 1.8.2 Fix

                                    // Check if the desired version already exists, otherwise download it.
                                    if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                                    {
                                        try
                                        {
                                            // Rename target version to "Terraria".
                                            DirectoryInfo dir = new DirectoryInfo(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                            dir.MoveTo(OutDir + @"\Terraria");
                                            Directory.Delete(OutDir + @"\Terraria", true); // Encase any files where left behind.
                                        }
                                        catch (Exception) { }
                                        Directory.CreateDirectory(OutDir + @"\Terraria"); // Update 1.8.2 Fix

                                        // Reload List
                                        ReloadList();

                                        // End the sub and prevent further downloads.
                                        return;
                                    }
                                }

                                // Check to see if this is a GitHub repo.
                                if (itemRow.SubItems[1].Text.ToLower().Contains("github"))
                                {
                                    try
                                    {
                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("GitHub download for Terraria-v" + itemRow.SubItems[0].Text + " initiated.");
                                        }

                                        if (UseSteamDirectory_CheckBox.Checked) // Use Steam Directory
                                        {
                                            // Folder system was already handled.
                                            // OutDir = Directory.GetParent(Properties.Settings.Default.DepotPath).ToString() + @"\Terraria";
                                            OutDir = Path.Combine(Properties.Settings.Default.DepotPath, "Terraria");
                                        }

                                        // Create an out path.
                                        Directory.CreateDirectory(OutDir);

                                        // Extract the owner and repo names.
                                        string repoOwner = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[1]; // Fix 1.8.5.4: Added Filter For "\t" To Separate Git From GUI
                                        string repoName = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[2];  // Fix 1.8.5.4: Added Filter For "\t" To Separate Git From GUI

                                        // Get the path name to the desired repo sub-directory.
                                        Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(repoName));
                                        var repositoryReadme = await client.Repository.Content.GetReadme(repoOwner, repoName);

                                        // Show Warning
                                        if (MessageBox.Show(repositoryReadme.Content + "\n" + "Do you wish to continue ?", "Repository Readme:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                        {
                                            // Log Item
                                            if (LogActions_CheckBox.Checked)
                                            {
                                                Console.WriteLine("User declined the readme terms.");
                                            }

                                            // Clean files.
                                            try
                                            {
                                                Directory.Delete(OutDir, true);
                                            }
                                            catch (Exception) { }
                                            // Directory.CreateDirectory(OutDir);

                                            // Fix 1.8.5.7: Reload list to generate readme.
                                            ReloadList();

                                            return;
                                        }

                                        var repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName);
                                        var directoryName = repositoryContents.FirstOrDefault(c => c.Path == "Terraria-v" + itemRow.SubItems[0].Text).Name;

                                        // Get the url to the desired file to download.
                                        repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName, directoryName);
                                        var downloadUrl = repositoryContents.FirstOrDefault().DownloadUrl;

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " found! Downloading." + downloadUrl);
                                        }

                                        // Start GitHub download.
                                        WebClient webClient = new WebClient();
                                        // webClient.Headers.Add("user-agent", "Anything");
                                        await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), OutDir + @"\" + directoryName + ".zip");

                                        // Extract ZIP Into Dir
                                        using (ZipFile archive = new ZipFile(@"" + OutDir + @"\" + directoryName + ".zip"))
                                        {
                                            // Async file extraction.
                                            await Task.Run(() => archive.ExtractAll(@"" + OutDir, ExtractExistingFileAction.OverwriteSilently));
                                        }

                                        // Clean zip files.
                                        try
                                        {
                                            // Async remove zip file.
                                            await Task.Run(() => File.Delete(OutDir + @"\" + directoryName + ".zip"));

                                            // Move files out of downloaded sub-directory.
                                            if (Directory.Exists(OutDir + @"\" + directoryName))
                                            {
                                                // Async file renaming.
                                                await Task.Run(() => RenameFolderAsync((OutDir + @"\" + directoryName).TrimEnd('\\', ' '), OutDir.TrimEnd('\\', ' ')));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(OutDir + @"\" + directoryName, true));
                                            }
                                            else
                                            {
                                                MessageBox.Show("DEBUG: " + OutDir + @"\" + directoryName);
                                            }
                                        }
                                        catch (Exception) { }

                                        // Reload List
                                        ReloadList();

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " download completed!");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // No repo file found, log it.
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("ERROR: This repository contains no versions that match: \"" + itemRow.SubItems[0].Text + "\"!");
                                        }

                                        // Process Failed, Delete Folder
                                        try
                                        {
                                            Directory.Delete(OutDir, true);
                                        }
                                        catch (Exception) { }
                                        Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix.
                                    }
                                }
                                else
                                {
                                    // Proceed to download through steam.
                                    String ManifestID = itemRow.SubItems[1].Text;
                                    String EscapedPassword = Regex.Replace(Password_TextBox.Text, @"[%|<>&^]", @"^$&"); // Escape Any CMD Special Characters If Any Exist // Update 1.8.5.2 Fix
                                    String Arg = "dotnet " + "\"" + DLLLocation + "\"" + " -app 105600 -depot 105601 -manifest " + ManifestID + " -username " + AccountName_TextBox.Text + " -password " + EscapedPassword + " -dir " + "\"" + OutDir + "\"" + ((RememberLogin_CheckBox.Checked) ? " -remember-password" : "");

                                    // Start Download
                                    try
                                    {
                                        // First create the directory (This forces ReloadList() to actually capture this download).
                                        Directory.CreateDirectory(OutDir);
                                        DownloaderIsBusy = true; // Ensure this empty directory does not get cleaned before download starts.

                                        // Start Download Process
                                        ExecuteCmd.ExecuteCommandAsync(Arg);

                                        // Reload List
                                        ReloadList();

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("Download prompt started for Terraria-v" + itemRow.SubItems[0].Text);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // Process Failed, Delete Folder
                                        try
                                        {
                                            Directory.Delete(OutDir, true);
                                        }
                                        catch (Exception) { }
                                        Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
                                    }
                                    finally
                                    {
                                        // Release the downloader is busy gate.
                                        DownloaderIsBusy = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Disable Button
                            Launch_Button.Enabled = false;

                            // Display Error
                            Console.WriteLine("ERROR: Please enter steam username / password");
                        }
                    }
                }
            }
        }
        #endregion

        #region Download Via ToolStrip

        // Download App Via ToolStrip
        private async void DownloadApp_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.Main_ListView.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If User & Pass Are Populated
                    if (AccountName_TextBox.Text != "" && Password_TextBox.Text != "")
                    {
                        // Check If Already Downloaded
                        if (itemRow.SubItems[2].Text == "No")
                        {
                            // Disable Button
                            Launch_Button.Enabled = false;

                            // Select Tab Control
                            // tabControl1.SelectedIndex = 2;

                            // Download Version
                            String DLLLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\DepotDownloader.dll";
                            String DotNetLocation = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\dotnet.exe";
                            // Update 1.5.0, Check If Overwrite To Steam Directory Is Enabled
                            String OutDir = Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text;

                            // Use Steam Directory.
                            if (UseSteamDirectory_CheckBox.Checked)
                            {
                                OutDir = Properties.Settings.Default.DepotPath;
                                string OutDirParent = OutDir; // Directory.GetParent(OutDir).ToString();

                                // Check If Client Is Already Running - Update 1.8.3
                                bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(OutDir, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                                if (isRunning)
                                {
                                    // Is running
                                    foreach (var process in Process.GetProcessesByName("Terraria"))
                                    {
                                        process.Kill();

                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("The Terraria process was killed to continue operations.");
                                        }
                                    }
                                }

                                // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                string currentVersion = File.ReadLines(OutDir + @"\Terraria\changelog.txt").First().Split(' ')[1].ToString();
                                currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                // Delete Folder
                                try
                                {
                                    // Ensure the target directory does not already exist.
                                    // Fix 1.8.5.8: Prevent empty game folders from being backed up. 
                                    if (!Directory.Exists(OutDirParent + @"\Terraria-v" + currentVersion) && currentVersion != "Changelog!")
                                    {
                                        DirectoryInfo dir = new DirectoryInfo(OutDir + @"\Terraria");
                                        dir.MoveTo(OutDirParent + @"\Terraria-v" + currentVersion);
                                        Directory.Delete(OutDir + @"\Terraria", true); // Encase any files where left behind.
                                    }
                                    else
                                    {
                                        // This version already exists, delete parent.
                                        Directory.Delete(OutDir + @"\Terraria", true);
                                    }
                                }
                                catch (Exception) { }
                                Directory.CreateDirectory(OutDir + @"\Terraria"); // Update 1.8.2 Fix

                                // Check if the desired version already exists, otherwise download it.
                                if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                                {
                                    try
                                    {
                                        // Rename target version to "Terraria".
                                        DirectoryInfo dir = new DirectoryInfo(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                        dir.MoveTo(OutDir + @"\Terraria");
                                        Directory.Delete(OutDir + @"\Terraria", true); // Encase any files where left behind.
                                    }
                                    catch (Exception) { }
                                    Directory.CreateDirectory(OutDir + @"\Terraria"); // Update 1.8.2 Fix

                                    // Reload List
                                    ReloadList();

                                    // End the sub and prevent further downloads.
                                    return;
                                }
                            }

                            // Check to see if this is a GitHub repo.
                            if (itemRow.SubItems[1].Text.ToLower().Contains("github"))
                            {
                                try
                                {
                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("GitHub download for Terraria-v" + itemRow.SubItems[0].Text + " initiated.");
                                    }

                                    if (UseSteamDirectory_CheckBox.Checked) // Use Steam Directory
                                    {
                                        // Folder system was already handled.
                                        // OutDir = Directory.GetParent(Properties.Settings.Default.DepotPath).ToString() + @"\Terraria";
                                        OutDir = Path.Combine(Properties.Settings.Default.DepotPath, "Terraria");
                                    }

                                    // Create an out path.
                                    Directory.CreateDirectory(OutDir);

                                    // Extract the owner and repo names.
                                    string repoOwner = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[1]; // Fix 1.8.5.4: Added Filter For "\t" To Separate Git From GUI
                                    string repoName = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[2];  // Fix 1.8.5.4: Added Filter For "\t" To Separate Git From GUI

                                    // Get the path name to the desired repo sub-directory.
                                    Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(repoName));
                                    var repositoryReadme = await client.Repository.Content.GetReadme(repoOwner, repoName);

                                    // Show Warning
                                    if (MessageBox.Show(repositoryReadme.Content + "\n" + "Do you wish to continue ?", "Repository Readme:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    {
                                        // Log Item
                                        if (LogActions_CheckBox.Checked)
                                        {
                                            Console.WriteLine("User declined the readme terms.");
                                        }

                                        // Clean files.
                                        try
                                        {
                                            Directory.Delete(OutDir, true);
                                        }
                                        catch (Exception) { }
                                        // Directory.CreateDirectory(OutDir);

                                        // Fix 1.8.5.7: Reload list to generate readme.
                                        ReloadList();

                                        return;
                                    }

                                    var repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName);
                                    var directoryName = repositoryContents.FirstOrDefault(c => c.Path == "Terraria-v" + itemRow.SubItems[0].Text).Name;

                                    // Get the url to the desired file to download.
                                    repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName, directoryName);
                                    var downloadUrl = repositoryContents.FirstOrDefault().DownloadUrl;

                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " found! Downloading." + downloadUrl);
                                    }

                                    // Start GitHub download.
                                    WebClient webClient = new WebClient();
                                    // webClient.Headers.Add("user-agent", "Anything");
                                    await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), OutDir + @"\" + directoryName + ".zip");

                                    // Extract ZIP Into Dir
                                    using (ZipFile archive = new ZipFile(@"" + OutDir + @"\" + directoryName + ".zip"))
                                    {
                                        // Async file extraction.
                                        await Task.Run(() => archive.ExtractAll(@"" + OutDir, ExtractExistingFileAction.OverwriteSilently));
                                    }

                                    // Clean zip files.
                                    try
                                    {
                                        // Async remove zip file.
                                        await Task.Run(() => File.Delete(OutDir + @"\" + directoryName + ".zip"));

                                        // Move files out of downloaded sub-directory.
                                        if (Directory.Exists(OutDir + @"\" + directoryName))
                                        {
                                            // Async file renaming.
                                            await Task.Run(() => RenameFolderAsync((OutDir + @"\" + directoryName).TrimEnd('\\', ' '), OutDir.TrimEnd('\\', ' ')));

                                            // Async delete directory.
                                            await Task.Run(() => Directory.Delete(OutDir + @"\" + directoryName, true));
                                        }
                                        else
                                        {
                                            MessageBox.Show("DEBUG: " + OutDir + @"\" + directoryName);
                                        }
                                    }
                                    catch (Exception) { }

                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " download completed!");
                                    }
                                }
                                catch (Exception)
                                {
                                    // No repo file found, log it.
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("ERROR: This repository contains no versions that match: \"" + itemRow.SubItems[0].Text + "\"!");
                                    }

                                    // Process Failed, Delete Folder
                                    try
                                    {
                                        Directory.Delete(OutDir, true);
                                    }
                                    catch (Exception) { }
                                    Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix.
                                }
                            }
                            else
                            {
                                // Proceed to download through steam.
                                String ManifestID = itemRow.SubItems[1].Text;
                                String EscapedPassword = Regex.Replace(Password_TextBox.Text, @"[%|<>&^]", @"^$&"); // Escape Any CMD Special Characters If Any Exist // Update 1.8.5.2 Fix
                                String Arg = "dotnet " + "\"" + DLLLocation + "\"" + " -app 105600 -depot 105601 -manifest " + ManifestID + " -username " + AccountName_TextBox.Text + " -password " + EscapedPassword + " -dir " + "\"" + Path.Combine(Properties.Settings.Default.DepotPath, "Terraria") + "\"" + ((RememberLogin_CheckBox.Checked) ? " -remember-password" : "");

                                // Start Download
                                try
                                {
                                    // Start Download Process
                                    ExecuteCmd.ExecuteCommandAsync(Arg);

                                    // Log Item
                                    if (LogActions_CheckBox.Checked)
                                    {
                                        Console.WriteLine("Download prompt started for Terraria-v" + itemRow.SubItems[0].Text);
                                    }
                                }
                                catch (Exception)
                                {
                                    // Process Failed, Delete Folder
                                    try
                                    {
                                        Directory.Delete(OutDir, true);
                                    }
                                    catch (Exception) { }
                                    Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
                                }
                            }

                            // Reload List
                            ReloadList();
                        }
                    }
                    else
                    {
                        // Disable Button
                        Launch_Button.Enabled = false;

                        // Display Error
                        Console.WriteLine("ERROR: Please enter steam username / password");
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Checkbox Settings

        // Update Checkbox Config
        private void LogActions_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (LogActions_CheckBox.Checked)
            {
                Properties.Settings.Default.LogActions = true;
            }
            else if (!LogActions_CheckBox.Checked)
            {
                Properties.Settings.Default.LogActions = false;
            }
        }

        // Show Prompt Warning
        private void UseSteamDirectory_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UseSteamDirectory_CheckBox.Checked && Properties.Settings.Default.UseSteamDir == false)
            {
                // Show Warning
                if (MessageBox.Show("This will download game versions to your steamapps." + "\n" + "Do you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // Cancel Prompt
                    UseSteamDirectory_CheckBox.Checked = false;

                    // Enable Path Changing
                    // button6.Enabled = true;

                    // Update Settings
                    Properties.Settings.Default.UseSteamDir = false;
                    Properties.Settings.Default.PathChangeEnabled = true;

                    // Update Forum

                    // Log Item
                    if (LogActions_CheckBox.Checked)
                    {
                        Console.WriteLine("Use steam directory mode cancelled!");
                    }
                    ReloadList();
                }
                else
                {
                    // Prompt Yes, Create Directory, Change Textbox
                    // Define the game path based on the registry rather then a hardcoded path encase game was installed else where. - Added 1.8.5.4.

                    // Define variables.
                    string backupLocation = BaseDepotDirectory_TextBox.Text;
                    string installLocation;

                    // Try to read registry key.
                    try
                    {
                        // Check if game location is found.
                        string gameLocation = GetGameLocation();
                        if (gameLocation != "" && gameLocation != "null" && gameLocation != "missing")
                        {
                            // Update install location.
                            installLocation = gameLocation;
                        }
                        else
                        {
                            // Define message based on error code.
                            string errorMessage = (gameLocation == "null")    ? "ERROR: Unable to find the default install location! Is steam even installed?" :
                                                  (gameLocation == "missing") ? "ERROR: Unable to find the default install location! Manually install the game at least once!" :
                                                                                "ERROR: Unable to find the default install location! An unknown error was returned.";

                            // Cancel operations and exit void.
                            MessageBox.Show(errorMessage, "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                            // Key does not exist, log item.
                            if (LogActions_CheckBox.Checked)
                            {
                                Console.WriteLine(errorMessage);
                            }

                            UseSteamDirectory_CheckBox.Checked = false;
                            BaseDepotDirectory_TextBox.Text = backupLocation;
                            return;
                        }

                        BaseDepotDirectory_TextBox.Text = backupLocation;
                    }
                    catch (Exception) // Handle exception.
                    {
                        // Key does not exist, log item.
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("ERROR: Something went wrong while reading the registry! Is steam even installed?");
                        }

                        // Cancel operations and exit void.
                        UseSteamDirectory_CheckBox.Checked = false;
                        BaseDepotDirectory_TextBox.Text = backupLocation;
                        return;
                    }

                    // Prompt Yes, Create Directory, Change Textbox
                    if (!Directory.Exists(Directory.GetParent(installLocation).FullName))
                    {
                        Directory.CreateDirectory(Directory.GetParent(installLocation).FullName);
                    }
                    BaseDepotDirectory_TextBox.Text = Directory.GetParent(installLocation).FullName;

                    // Disable Path Changing
                    // button6.Enabled = false;

                    // Update Settings
                    Properties.Settings.Default.DepotPath = Directory.GetParent(installLocation).FullName;
                    Properties.Settings.Default.UseSteamDir = true;
                    Properties.Settings.Default.PathChangeEnabled = false;

                    // Update Forum

                    // Log Item
                    if (LogActions_CheckBox.Checked)
                    {
                        Console.WriteLine("Use steam directory mode enabled!");
                    }
                    ReloadList();
                }
            }
            if (!UseSteamDirectory_CheckBox.Checked && Properties.Settings.Default.UseSteamDir == true)
            {
                // Checkbox Unchecked, Reset Textbox To default Dir
                BaseDepotDirectory_TextBox.Text = Application.StartupPath + @"\TerrariaDepots";

                // Enable Path Changing
                Browse_Button.Enabled = true;

                // Update Settings
                Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
                Properties.Settings.Default.UseSteamDir = false;
                Properties.Settings.Default.PathChangeEnabled = true;

                // Update Forum

                // Log Item
                if (LogActions_CheckBox.Checked)
                {
                    Console.WriteLine("Use steam directory mode disabled!");
                }
                ReloadList();
            }
        }

        // Tooltip Controls
        private void ShowTooltips_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Tooltips
            if (ShowTooltips_CheckBox.Checked)
            {
                // Enable Tooltips
                Properties.Settings.Default.ToolTips = true;
                Tooltips.Active = true;
            }
            else
            {
                // Disable Tooltips
                Properties.Settings.Default.ToolTips = false;
                Tooltips.Active = false;
            }
        }

        // Skip Update Controls
        private void SkipUpdateCheck_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Update Check
            if (SkipUpdateCheck_CheckBox.Checked)
            {
                // Enable Check
                Properties.Settings.Default.SkipUpdate = true;
            }
            else
            {
                // Disable Check
                Properties.Settings.Default.SkipUpdate = false;
            }
        }

        // Update Save Login
        private void RememberLogin_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable SaveLogin
            if (RememberLogin_CheckBox.Checked)
            {
                // Enable SaveLogin
                Properties.Settings.Default.SaveLogin = true;
            }
            else
            {
                // Disable SaveLogin
                Properties.Settings.Default.SaveLogin = false;
            }
        }

        // Update darkmode.
        private void DarkMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Check if checkbox was checked or not.
            if (DarkMode_CheckBox.Checked)
            {
                // Enable darkmode.
                DarkMode(true);

                // Save darkmode setting.
                Properties.Settings.Default.DarkMode = true;
            }
            else
            {
                // Disable darkmode.
                DarkMode(false);

                // Save darkmode setting.
                Properties.Settings.Default.DarkMode = false;
            }
        }

        // Enable or disable collectors edition.
        private void EnableCollectorsEdition_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Define the registry paths.
            const string keyPath = @"Software\Terraria";
            const string subKeyName = "Bunny";

            // Check if checkbox was checked or not.
            if (EnableCollectorsEdition_CheckBox.Checked)
            {
                // Enable collectors edition.

                // Open the parent key.
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                {
                    if (key == null)
                    {
                        // The key doesn't exist, create it.
                        using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(keyPath))
                        {
                            // Add the value under the new subkey.
                            newKey.SetValue(subKeyName, "1");

                            // Do Logging If Enabled
                            if (LogActions_CheckBox.Checked)
                            {
                                Console.WriteLine("Successfully enabled the collectors edition!");
                            }
                        }
                    }
                    else
                    {
                        // Check if the value "Bunny" exists.
                        object value = key.GetValue(subKeyName);
                        if (value == null)
                        {
                            // The subkey doesn't exist, create it and set its value.
                            key.SetValue(subKeyName, "1");

                            // Do Logging If Enabled
                            if (LogActions_CheckBox.Checked)
                            {
                                Console.WriteLine("Successfully enabled the collectors edition!");
                            }
                        }
                    }
                }
            }
            else
            {
                // Disable collectors edition.

                // Open the parent key.
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                {
                    if (key != null)
                    {
                        // Add the value under the new subkey.
                        key.DeleteValue(subKeyName, false);

                        // Do Logging If Enabled
                        if (LogActions_CheckBox.Checked)
                        {
                            Console.WriteLine("Successfully disabled the collectors edition!");
                        }
                    }
                }
            }
        }

        // User separate configs for each version.
        private async void UseSeparateConfigs_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Tooltips
            if (UseSeparateConfigs_CheckBox.Checked)
            {
                // Enable Tooltips
                Properties.Settings.Default.UseSeparateConfigs = true;
            }
            else
            {
                // Disable Tooltips
                Properties.Settings.Default.UseSeparateConfigs = false;

                #region Load default Configuration

                // Define config directory path.
                string configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games";
                string lastGameVersion = "0.0.0.0";

                // Read the last loaded configuration version.
                if (File.Exists(configPath + @"\TerrariaDepotDownloaderData.txt"))
                {
                    // Open the file for reading
                    using (StreamReader reader = new StreamReader(configPath + @"\TerrariaDepotDownloaderData.txt"))
                    {
                        // Read the first line.
                        lastGameVersion = reader.ReadLine();

                        // Close the reader.
                        reader.Close();
                    }
                }
                else
                {
                    // No data file found. Create new file.
                    await Task.Run(() => File.Create(configPath + @"\TerrariaDepotDownloaderData.txt"));
                }

                // Switch back to the original games config.
                try
                {
                    // Ensure the desired config already exist.
                    if (!Directory.Exists(configPath + @"\Terraria-v" + lastGameVersion) && lastGameVersion != "0.0.0.0")
                    {
                        // Rename the existing config to its previous version.
                        await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-v" + lastGameVersion));

                        // Async delete directory.
                        await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));
                    }
                    else
                    {
                        // It already has a backup, ignore.

                        // Async delete directory.
                        if (lastGameVersion != "0.0.0.0")
                            await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));
                    }

                    // Restore the original directory.
                    // Check if the desired config folder exists or not.
                    if (Directory.Exists(configPath + @"\Terraria-Original"))
                    {
                        // Async file renaming.
                        await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria-Original", configPath + @"\Terraria"));

                        // Async delete directory.
                        await Task.Run(() => Directory.Delete(configPath + @"\Terraria-Original", true));
                    }
                    else
                    {
                        // No default directory found. Create one.
                        await Task.Run(() => Directory.CreateDirectory(configPath + @"\Terraria"));
                    }

                    // Log Item
                    if (LogActions_CheckBox.Checked)
                    {
                        if (lastGameVersion != "0.0.0.0")
                            Console.WriteLine("Switched game config from Terraria-v" + lastGameVersion + " to Terraria-Original!");
                    }

                    // Log last config loaded.
                    using (StreamWriter writer = new StreamWriter(configPath + @"\TerrariaDepotDownloaderData.txt", false))
                    {
                        // Write the last loaded game version to file.
                        writer.WriteLine("0.0.0.0");

                        // Close the writer.
                        writer.Close();
                    }
                }
                catch (Exception)
                {
                    // Log Item
                    Console.WriteLine("Failed to switch the game configs!");
                }
                #endregion
            }
        }
        #endregion

        #region Password Manager

        // Encrypts a string using AES encryption with a given key.
        public static string EncryptString(string plainText, string key)
        {
            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                // Derive a fixed-size key from the input key using PBKDF2.
                aesAlg.Key = DeriveKey(key, aesAlg.KeySize / 8);
                aesAlg.GenerateIV(); // Generate a random initialization vector (IV).

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Prepend the IV to the encrypted bytes.
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write the plaintext data to the stream, which gets encrypted.
                            swEncrypt.Write(plainText);
                        }
                    }
                    // Get the encrypted bytes from the memory stream.
                    encryptedBytes = msEncrypt.ToArray();
                }
            }
            // Return the encrypted bytes as a base64 encoded string.
            return Convert.ToBase64String(encryptedBytes);
        }

        // Decrypts a string encrypted with AES encryption using a given key.
        static string DecryptString(string cipherText, string key)
        {
            try
            {
                // Convert the base64 encoded ciphertext to bytes.
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                string plaintext;

                // Create a new AES instance.
                using (Aes aesAlg = Aes.Create())
                {
                    // Derive the encryption key using PBKDF2.
                    aesAlg.Key = DeriveKey(key, aesAlg.KeySize / 8);

                    // Extract the initialization vector (IV) from the beginning of the ciphertext.
                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    Array.Copy(cipherTextBytes, iv, iv.Length);
                    aesAlg.IV = iv;

                    // Create a decryptor to perform the decryption operation.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create streams for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes, iv.Length, cipherTextBytes.Length - iv.Length))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted plaintext from the stream.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                // Return the decrypted plaintext.
                return plaintext;
            }
            catch (Exception)
            {
                // Return a blank string if decryption fails.
                return string.Empty;
            }
        }

        // Derive a fixed-size key from the input key using PBKDF2.
        static byte[] DeriveKey(string key, int keySize)
        {
            const int iterations = 10000; // Number of iterations for PBKDF2.
            byte[] salt = Encoding.UTF8.GetBytes("salt123456"); // Salt should be at least 8 bytes.
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(key, salt, iterations))
            {
                return pbkdf2.GetBytes(keySize);
            }
        }

        // Function to check if a string is base64 encoded.
        public static bool IsBase64String(string base64String)
        {
            try
            {
                // Decoding the base64 string.
                byte[] data = Convert.FromBase64String(base64String);
                // If decoding succeeds, then it's a valid base64 string.
                return true;
            }
            catch (FormatException)
            {
                // If decoding fails, then it's not a valid base64 string.
                return false;
            }
        }
        #endregion
    }
}
