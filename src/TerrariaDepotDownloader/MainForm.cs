﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;
using Microsoft.Win32;

namespace TerrariaDepotDownloader
{
    public partial class MainForm : Form
    {
        // Say Hello To Decompilers
        private static readonly string HelloThere = "Hello There Fellow Decompiler, This Program Was Made By Discord:dannyruss (xXCrypticNightXx).";

        #region Form Load

        public MainForm()
        {
            InitializeComponent();
            Console.SetOut(new MultiTextWriter(new ControlWriter(richTextBox1), Console.Out));
        }

        // Toggle the forms darkmode.
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Toggle darkmode.
            if (Properties.Settings.Default.DarkMode)
                DarkMode(true);

            // Update checkboxes.
            checkBox6.Checked = Properties.Settings.Default.DarkMode;
        }

        // Do Loading Events
        public ToolTip Tooltips = new ToolTip();
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Varify .NET 6.0 Or Later Exists - Update 1.8.3
            var dotnet86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\dotnet\host\fxr";
            var dotnet64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\host\fxr";
            var dotnet86SDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\dotnet\sdk";
            var dotnet64SDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\sdk";

            // Check If A Single Paths Exists
            if (!Directory.Exists(dotnet86) && !Directory.Exists(dotnet64) && !Directory.Exists(dotnet86SDK) && !Directory.Exists(dotnet64SDK))
            {
                MessageBox.Show(".NET 6.0 Is Required! Please Install And Try Agian. \n \n https://dotnet.microsoft.com/download/dotnet/6.0", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Close Application
                Application.Exit();
            }

            // Check Value
            try
            {
                // Create A List For Versions
                List<String> versionList = new List<string> { };

                // Check If x86 Path Exists
                if (Directory.Exists(dotnet86))
                {
                    // Add Folder Names To List
                    foreach (string version in Directory.GetDirectories(dotnet86).Select(Path.GetFileName).ToArray())
                    {
                        // Remove Any "-" From Name
                        versionList.Add(version.Split('-')[0]);
                    }
                }

                // Check If x64 Path Exists
                if (Directory.Exists(dotnet64))
                {
                    // Add Folder Names To List
                    foreach (string version in Directory.GetDirectories(dotnet64).Select(Path.GetFileName).ToArray())
                    {
                        // Remove Any "-" From Name
                        versionList.Add(version.Split('-')[0]);
                    }
                }

                // Check If x86 SDK Path Exists
                if (Directory.Exists(dotnet86SDK))
                {
                    // Add Folder Names To List
                    foreach (string version in Directory.GetDirectories(dotnet86SDK).Select(Path.GetFileName).ToArray())
                    {
                        // Remove Any "-" From Name
                        versionList.Add(version.Split('-')[0]);
                    }
                }

                // Check If x64 SDK Path Exists
                if (Directory.Exists(dotnet64SDK))
                {
                    // Add Folder Names To List
                    foreach (string version in Directory.GetDirectories(dotnet64SDK).Select(Path.GetFileName).ToArray())
                    {
                        // Remove Any "-" From Name
                        versionList.Add(version.Split('-')[0]);
                    }
                }

                // Check If Version Is Above or Equal 6.0.0
                if (!versionList.Any(x => Version.Parse(x) >= Version.Parse("6.0.0")))
                {
                    // Log .NET Version
                    Console.WriteLine("ERROR: Highest .NET version found: " + versionList.Max());

                    // Version Not Found
                    MessageBox.Show(".NET 6.0+ is required! Please install and try agian. \n \n https://dotnet.microsoft.com/download/dotnet/6.0 \n \n Highest .NET version found: " + versionList.Max(), "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

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

            // Load Steam Textbox Data
            textBox2.Text = Properties.Settings.Default.SteamUser;
            textBox3.Text = Properties.Settings.Default.SteamPass;

            // Create Depot Folder
            if (!Directory.Exists(Application.StartupPath + @"\TerrariaDepots"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\TerrariaDepots");
                Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
            }
            textBox1.Text = Application.StartupPath + @"\TerrariaDepots";

            // Populate Depot Setting Path
            if (Directory.Exists(Application.StartupPath + @"\TerrariaDepots") && Properties.Settings.Default.DepotPath == "")
            {
                // Check If Use Steam Directory Is Enabled
                if (checkBox2.Checked)
                {
                    // Use Steam Directory Enabled
                    if (GetGameLocation() != "")
                    {
                        // Update install location.
                        Properties.Settings.Default.DepotPath = GetGameLocation();
                    }
                    else
                    {
                        // Steam game not found, use startup path instead!
                        checkBox2.Checked = false;
                        Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";

                        // Log Item
                        if (checkBox1.Checked)
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
                    checkBox2.Checked = false;
                    Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";

                    // Log Item
                    if (checkBox1.Checked)
                    {
                        Console.WriteLine("ERROR: The existing game install location appears to no longer exist!");
                    }
                }
            }

            // Update Depot Path & Create Folder
            textBox1.Text = Properties.Settings.Default.DepotPath;
            if (!Directory.Exists(Properties.Settings.Default.DepotPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.DepotPath);
            }

            // Update Checkboxes
            checkBox1.Checked = Properties.Settings.Default.LogActions;
            checkBox2.Checked = Properties.Settings.Default.UseSteamDir;
            checkBox3.Checked = Properties.Settings.Default.ToolTips;
            checkBox4.Checked = Properties.Settings.Default.SkipUpdate;
            checkBox5.Checked = Properties.Settings.Default.SaveLogin;
            checkBox8.Checked = Properties.Settings.Default.UseSeparateConfigs;

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
                            checkBox7.Checked = true;
                        }
                        else
                        {
                            // Value does not exists. Disable checkbox.
                            checkBox7.Checked = false;
                        }
                    }
                    else
                    {
                        // Value does not exists. Disable checkbox.
                        checkBox7.Checked = false;
                    }
                }
                else
                {
                    // Value does not exists. Disable checkbox.
                    checkBox7.Checked = false;
                }
            }

            // Add Tooltips - Update 1.8.5
            Tooltips.InitialDelay = 1000;
            Tooltips.SetToolTip(button1, "Close game and application");
            Tooltips.SetToolTip(button2, "Download / Launch Terraria version");
            Tooltips.SetToolTip(button3, "Reload all installed versions");
            Tooltips.SetToolTip(button4, "Clear log of all entries");
            Tooltips.SetToolTip(button5, "Remove selected version");
            Tooltips.SetToolTip(button6, "Browse for a new install directory");
            Tooltips.SetToolTip(button7, "Temporarily show your password");
            Tooltips.SetToolTip(button8, "Remove all games from the list");
            Tooltips.SetToolTip(button9, "Open current base directory");

            Tooltips.SetToolTip(checkBox1, "Log all actions to the output log");
            Tooltips.SetToolTip(checkBox2, "All installs use the Steam directory");
            Tooltips.SetToolTip(checkBox3, "Show or hide tooltips");
            Tooltips.SetToolTip(checkBox4, "Skip API update check");
            Tooltips.SetToolTip(checkBox5, "Remember the password and steam key for this user");
            Tooltips.SetToolTip(checkBox6, "Enable or disable the dark mode theme");
            Tooltips.SetToolTip(checkBox7, "Enable or disable the collectors edition");
            Tooltips.SetToolTip(checkBox8, "Use a separate config folder for each game version");

            // Enable or Disable Tooltips
            if (checkBox3.Checked)
            {
                // Enable Tooltips
                Tooltips.Active = true;
            }
            else
            {
                // Disable Tooltips
                Tooltips.Active = false;
            }

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
                foreach (string line in File.ReadAllLines(Application.StartupPath + @"\ManifestVersions.cfg"))
                {
                    try
                    {
                        // Check If String Contains "null"
                        if (line.Substring(line.LastIndexOf(' ') + 1) == "null")
                        {
                            // String Contains "null", Add Context
                            listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), "(no manifests database exists)", "N/A" }));
                        }
                        else
                        {
                            // Check If Game Version Folder Exists
                            if (Directory.Exists(Properties.Settings.Default.DepotPath + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ','))))
                            {
                                // String Does Not Contain "null", Record Like Normal
                                listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                            }
                            else
                            {
                                // String Does Not Contain "null", Record Like Normal
                                listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
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

            // Check For Updates
            if (!Properties.Settings.Default.SkipUpdate)
            {
                try
                {
                    // Check Github For DepotDownloader Update
                    Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("DepotDownloader"));
                    var releases = await client.Repository.Release.GetAll("SteamRE", "DepotDownloader");
                    var latestGithubRelease = releases[0].TagName;

                    // Get Current DepotDownload.dll Version
                    var currentVersionInfo = FileVersionInfo.GetVersionInfo(Application.StartupPath + @"\DepotDownloader.dll");
                    // Console.WriteLine(latestGithubRelease.ToString() + " | " + "DepotDownloader_" + currentVersionInfo.ProductVersion.ToString());

                    // Do Version Check
                    if (latestGithubRelease.ToString() != "DepotDownloader_" + currentVersionInfo.ProductVersion.ToString())
                    {
                        // New Version Found
                        // Log Item
                        if (checkBox1.Checked)
                        {
                            Console.WriteLine("New DepotDownloader API is available.");
                        }

                        // Ask For Install
                        if (MessageBox.Show("You have an outdated version of the DepotDownloader API" + "\n" + "Do you wish to download and install the update?", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            // Install Update
                            try
                            {
                                // Download From Github
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
                                if (checkBox1.Checked)
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
                        if (checkBox1.Checked)
                        {
                            Console.WriteLine("DepotDownloader API is up to date.");
                        }
                        return;
                    }
                }
                catch (Exception)
                {
                    // Error Checking Version
                    Console.WriteLine("ERROR: Unable to check DepotDownloader API's Github version!");
                    return;
                }
            }
            else
            {
                // Log Event
                if (checkBox1.Checked)
                {
                    Console.WriteLine("DepotDownloader API new vesion check was skipped!");
                }
            }
        }
        #endregion

        #region Form Controls

        // Open Browse Dialogue
        private void Button6_Click(object sender, EventArgs e)
        {
            // Check if steam directory is enabled or not. // Fix 1.8.5.7: Allow people to change their default steam location.
            if (!Properties.Settings.Default.PathChangeEnabled)
            {
                // Conformation Box
                if (MessageBox.Show("Warning! Are you sure you want to force change your defualt steam location? TerrariaDepotDownloader is supposed to automatically find your games correct location!\n\nThis is only reccomended for advanced users!\nYes or No", "WARNING: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Launch path dialog.
                    using (var fbd = new FolderBrowserDialog())
                    {
                        DialogResult result = fbd.ShowDialog();

                        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            textBox1.Text = fbd.SelectedPath;
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
                        textBox1.Text = fbd.SelectedPath;
                        Properties.Settings.Default.DepotPath = fbd.SelectedPath;
                    }
                }
            }
        }

        // Close Games & Application
        private void Button1_Click(object sender, EventArgs e)
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
            Properties.Settings.Default.SteamUser = textBox2.Text;
            Properties.Settings.Default.SteamPass = textBox3.Text;

            // Save Settings
            Properties.Settings.Default.Save();

            // Close Application
            Application.Exit();
        }

        // Form Closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Gather Steam Data
            Properties.Settings.Default.SteamUser = textBox2.Text;
            Properties.Settings.Default.SteamPass = textBox3.Text;

            // Save Settings
            Properties.Settings.Default.Save();

            // Close Application
            Application.Exit();
        }

        // Clear Log
        private void Button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Update();
        }

        // Close Via ToolStrip
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Gather Steam Data
            Properties.Settings.Default.SteamUser = textBox2.Text;
            Properties.Settings.Default.SteamPass = textBox3.Text;

            // Save Settings
            Properties.Settings.Default.Save();

            // Close Application
            Application.Exit();
        }

        // Open Info Tab
        private void ToolStripDropDownButton1_MouseUp(object sender, MouseEventArgs e)
        {
            // Open New Form2
            About frm2 = new About();
            frm2.ShowDialog();
        }

        // Show Password
        private void Button7_MouseDown(object sender, MouseEventArgs e)
        {
            textBox3.PasswordChar = '\u0000';
        }

        // Hide Password
        private void Button7_MouseUp(object sender, MouseEventArgs e)
        {
            textBox3.PasswordChar = '*';
        }

        // Open Context Menu
        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listView1.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        // Open Depot Folder Directory
        private void Button9_Click(object sender, EventArgs e)
        {
            try
            {
                // Open Folder
                Process.Start(Properties.Settings.Default.DepotPath);

                // Log Action
                if (checkBox1.Checked)
                {
                    Console.WriteLine("Opened depot directory");
                }
            }
            catch (Win32Exception win32Exception)
            {
                //The system cannot find the file specified...
                Console.WriteLine(win32Exception.Message);
            }
        }

        // Auto Scroll To End
        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            richTextBox1.SelectionStart = richTextBox1.Text.Length;

            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }
        #endregion

        #region Main

        // Reload List
        private void Button3_Click(object sender, EventArgs e)
        {
            ReloadList();
            if (checkBox1.Checked)
            {
                Console.WriteLine("App list reloaded");
            }
        }

        public void ReloadList()
        {
            // Clear ListView
            listView1.Items.Clear();
            listView1.Refresh();

            // Check If Directory Contains A ChangeLog If Use Steam Directory Is Enabled
            if (checkBox2.Checked)
            {
                // Ensure directory exists.
                if (!Directory.Exists(Properties.Settings.Default.DepotPath))
                {
                    Directory.CreateDirectory(Properties.Settings.Default.DepotPath);
                    Console.WriteLine("Terraria steam directory not found! Generating a new one.");
                }

                // Check If Directory Contains A ChangeLog
                if (!File.Exists(Properties.Settings.Default.DepotPath + @"\changelog.txt"))
                {
                    File.WriteAllText(Properties.Settings.Default.DepotPath + @"\changelog.txt", "Empty Changelog!");
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
            button2.Text = "Download";
            button2.Enabled = false; // Fix 1.8.5.5.
            button5.Enabled = false;

            // Make Sure Database Is Populated
            if (File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != null && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != "" && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First() != "Version, Manifest-ID" && File.ReadLines(Application.StartupPath + @"\ManifestVersions.cfg").First().Contains(","))
            {
                // Get Database To List
                List<string> manifests = new List<string>() { };
                foreach (string line in File.ReadAllLines(Application.StartupPath + @"\ManifestVersions.cfg"))
                {
                    try
                    {
                        // Check If String Contains "null"
                        if (line.Substring(line.LastIndexOf(' ') + 1) == "null")
                        {
                            // String Contains "null", Add Context
                            listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), "(no manifests database exists)", "N/A" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                        }
                        else
                        {
                            // Check If Use Steam Directory Is Enabled
                            if (checkBox2.Checked)
                            {
                                // Check for backup folders.
                                if (Directory.Exists(Directory.GetParent(Properties.Settings.Default.DepotPath) + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ','))) || Directory.Exists(Properties.Settings.Default.DepotPath + @"\Terraria"))
                                {
                                    // Check If Folder Is Not Empty - Update Feature
                                    if (Directory.EnumerateFileSystemEntries(Directory.GetParent(Properties.Settings.Default.DepotPath) + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ','))).Any())
                                    {
                                        // String Does Not Contain "null", Record Like Normal
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                    }
                                    else
                                    {
                                        if (Directory.Exists(Properties.Settings.Default.DepotPath) && File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == String.Concat(line.TakeWhile(c => c != ',')) || File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.3" && String.Concat(line.TakeWhile(c => c != ',')) == "1.3.0.1" || File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.4" && String.Concat(line.TakeWhile(c => c != ',')) == "1.4.0.1")
                                        {
                                            // String Does Not Contain "null", Record Like Normal
                                            listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                        }
                                        else
                                        {
                                            // Delete Folder
                                            Directory.Delete(Directory.GetParent(Properties.Settings.Default.DepotPath) + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ',')), true);

                                            // Log Item
                                            if (checkBox1.Checked)
                                            {
                                                Console.WriteLine("Removed empty folder: " + Directory.GetParent(Properties.Settings.Default.DepotPath) + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ',')));
                                            }

                                            // String Does Not Contain "null", Record Like Normal
                                            listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                        }
                                    }
                                }
                                else
                                {
                                    // Added Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1 - Update 1.8.4 // if changelog.txt reads 1.3 & array version reads 1.3.0.1
                                    if (File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == String.Concat(line.TakeWhile(c => c != ',')) || File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.3" && String.Concat(line.TakeWhile(c => c != ',')) == "1.3.0.1" || File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.4" && String.Concat(line.TakeWhile(c => c != ',')) == "1.4.0.1")
                                    {
                                        // String Does Not Contain "null", Record Like Normal
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                    }
                                    else
                                    {
                                        // String Does Not Contain "null", Record Like Normal
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.

                                    }
                                }
                            }
                            else
                            {
                                // Check If Game Version Folder Exists
                                if (Directory.Exists(Properties.Settings.Default.DepotPath + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ','))))
                                {
                                    // Check If Folder Is Not Empty - Update Feature
                                    if (Directory.EnumerateFileSystemEntries(Properties.Settings.Default.DepotPath + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ','))).Any())
                                    {
                                        // String Does Not Contain "null", Record Like Normal
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "Yes" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                    }
                                    else
                                    {
                                        // Delete Folder
                                        Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ',')), true);

                                        // Log Item
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("Removed empty folder: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + String.Concat(line.TakeWhile(c => c != ',')));
                                        }

                                        // String Does Not Contain "null", Record Like Normal
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                    }
                                }
                                else
                                {
                                    // String Does Not Contain "null", Record Like Normal
                                    listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1).ToLower().Contains("github") ? "GitHub - Unofficial Patch\t                    \t" + line.Substring(line.LastIndexOf(' ') + 1) : line.Substring(line.LastIndexOf(' ') + 1), "No" })); // Fix v1.8.5.4: Add Check For GitHub Links.
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Error, No Updated Manifests
                        MessageBox.Show("ERROR: The manifest file contains an error!", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        // Remove App Tool Via ToolStrip
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Check If Removal Is Avalible
            if (!button5.Enabled)
            {
                return;
            }

            // Disable If Use Steam Directory Enabled
            if (checkBox2.Checked)
            {
                MessageBox.Show("You cannot use this feature while \"Use Steam Directory\" feature is enabled.", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            // Get Each Row
            foreach (ListViewItem itemRow in this.listView1.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Check If Client Is Currently Running - Update 1.8.3
                        bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                        if (isRunning)
                        {
                            // Is running
                            foreach (var process in Process.GetProcessesByName("Terraria"))
                            {
                                process.Kill();

                                // Log Item
                                if (checkBox1.Checked)
                                {
                                    Console.WriteLine("The Terraria process was killed to continue operations.");
                                }
                            }
                        }

                        // Delete Folder
                        Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                        // Log Item
                        if (checkBox1.Checked)
                        {
                            Console.WriteLine("Removed: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text);
                        }

                        // Update Forum
                        ReloadList();
                    }
                }
            }
        }

        // Update Button
        private void ListView1_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.listView1.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Edit Launch Button
                        button2.Enabled = true;
                        button2.Text = "Launch";

                        // Edit Remove Button
                        button5.Enabled = true;
                    }
                    else if (itemRow.SubItems[2].Text == "No")
                    {
                        // Edit Launch Button
                        button2.Enabled = true;
                        button2.Text = "Download";

                        // Edit Remove Button
                        button5.Enabled = false;
                    }
                    else if (itemRow.SubItems[2].Text == "N/A")
                    {
                        button2.Text = "N/A";
                        button2.Enabled = false;

                        // Edit Remove Button // Fix 1.8.5.7.
                        button5.Enabled = false;
                    }
                }
            }
        }

        // Remove All Games
        private void Button8_Click(object sender, EventArgs e)
        {
            // Check For Any Open Clients - Update 1.8.3
            // Check if use steam directory.
            if (!checkBox2.Checked)
            {
                if (Process.GetProcessesByName("Terraria").Length > 0)
                {
                    // Is running
                    foreach (var process in Process.GetProcessesByName("Terraria"))
                    {
                        process.Kill();

                        // Log Item
                        if (checkBox1.Checked)
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
                foreach (ListViewItem itemRow in this.listView1.Items)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Check if use steam directory.
                        if (checkBox2.Checked)
                        {
                            // Get the parent directory.
                            string OutDirParent = Directory.GetParent(Properties.Settings.Default.DepotPath).ToString();

                            // Check if directory exists, if not, its the current version.
                            if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                            {
                                // Exists, delete it.
                                Directory.Delete(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                                // Log Item
                                if (checkBox1.Checked)
                                {
                                    Console.WriteLine("Removed: " + OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                }
                            }
                        }
                        else
                        {
                            // Delete Folder
                            Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                            // Log Item
                            if (checkBox1.Checked)
                            {
                                Console.WriteLine("Removed: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text);
                            }
                        }
                    }
                }
                // Update Forum
                ReloadList();

                // Log Item
                if (checkBox1.Checked)
                {
                    Console.WriteLine("All apps removed");
                }
            }
        }

        // Remove App
        private void Button5_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.listView1.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If Already Downloaded
                    if (itemRow.SubItems[2].Text == "Yes")
                    {
                        // Check If Client Is Currently Running - Update 1.8.3
                        // Check if use steam directory.
                        if (!checkBox2.Checked)
                        {
                            bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                            if (isRunning)
                            {
                                // Is running
                                foreach (var process in Process.GetProcessesByName("Terraria"))
                                {
                                    process.Kill();

                                    // Log Item
                                    if (checkBox1.Checked)
                                    {
                                        Console.WriteLine("The Terraria process was killed to continue operations.");
                                    }
                                }
                            }
                        }

                        // Check if use steam directory.
                        if (checkBox2.Checked)
                        {
                            // Get the parent directory.
                            string OutDirParent = Directory.GetParent(Properties.Settings.Default.DepotPath).ToString();

                            // Check if directory exists, if not, its the current version.
                            if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                            {
                                // Exists, delete it.
                                Directory.Delete(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                                // Log Item
                                if (checkBox1.Checked)
                                {
                                    Console.WriteLine("Removed: " + OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                }
                            }
                            else
                            {
                                // Does not exist, log it.
                                // Log Item
                                if (checkBox1.Checked)
                                {
                                    Console.WriteLine("ERROR: Cannot delete Terraria-v" + itemRow.SubItems[0].Text + ". This version is currently active.");
                                }

                                // Display error.
                                MessageBox.Show("ERROR: Cannot delete Terraria - v" + itemRow.SubItems[0].Text + ".\nThis version is currently active.", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            // Delete Folder
                            Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                            // Log Item
                            if (checkBox1.Checked)
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
            button5.Enabled = false;
        }

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

                // Turn on dark mode for maiin form.
                MainForm.ActiveForm.BackColor = formBackColor;
                MainForm.ActiveForm.ForeColor = controlForeColor;

                // Turn on dark mode for remaining controls.
                foreach (Control component in this.Controls)
                {
                    // Check if componet is a picturebox.
                    if (component is PictureBox)
                    {
                        pictureBox1.BackColor = formBackColor;
                    }
                    else
                    {
                        // All other controls.
                        component.BackColor = controlBackColor;
                        component.ForeColor = controlForeColor;
                    }
                }

                // Recolor each tabpage.
                for (int a = 0; a < tabControl1.TabPages.Count; a++)
                {
                    tabControl1.TabPages[a].BackColor = tabControlBackColor;
                    tabControl1.TabPages[a].ForeColor = controlForeColor;
                }

                // Turn on dark mode for remaining controls.
                foreach (TabPage tab in tabControl1.TabPages)
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
                // Turn off dark mode for maiin form.
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
                for (int a = 0; a < tabControl1.TabPages.Count; a++)
                {
                    tabControl1.TabPages[a].BackColor = DefaultBackColor;
                    tabControl1.TabPages[a].ForeColor = DefaultForeColor;
                }

                // Turn on dark mode for remaining controls.
                foreach (TabPage tab in tabControl1.TabPages)
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

        // Get game location.
        public string GetGameLocation()
        {
            using (var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                string subKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 105600";
                using (var key = root.OpenSubKey(subKey)) // False is important!
                {
                    var s = key.GetValue("InstallLocation") as string;
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        // Define path; should be the games initial install directory.
                        return s + @"\Terraria";
                    }
                    else
                    {
                        // Key does not exist, log item.
                        if (checkBox1.Checked)
                        {
                            // Console.WriteLine("ERROR: Unable to find the default install location! Try reinstalling your game!");
                        }

                        // Cancle operations and exit void.
                        // checkBox2.Checked = false;
                        return "";
                    }
                }
            }
        }
        #endregion

        #region Launch / Download

        // Launch Button
        private async void Button2_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.listView1.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check Options
                    if (button2.Text == "Launch")
                    {
                        // Launch App
                        //
                        // Check If Use Steam Directory Is Enabled
                        if (checkBox2.Checked)
                        {
                            try
                            {
                                // Get the correct directory and move it to "Terraria".
                                string OutDir = Properties.Settings.Default.DepotPath;
                                string OutDirParent = Directory.GetParent(OutDir).ToString();

                                // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                string currentVersion = File.ReadLines(OutDir + @"\changelog.txt").First().Split(' ')[1].ToString();
                                currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                // Check if version is different from the selected.
                                if (currentVersion != itemRow.SubItems[0].Text)
                                {
                                    try
                                    {
                                        // Check if the existing game version already exists or not.
                                        if (!Directory.Exists(OutDirParent + @"\Terraria-v" + currentVersion))
                                        {
                                            // Move to a backup.
                                            DirectoryInfo dir = new DirectoryInfo(OutDir);
                                            dir.MoveTo(OutDirParent + @"\Terraria-v" + currentVersion);
                                            Directory.Delete(OutDir, true); // Encase any files where left behind.
                                        }
                                        else
                                        {
                                            // This version already exists, delete parent.
                                            Directory.Delete(OutDir, true);
                                        }
                                    }
                                    catch (Exception) { }

                                    // Grab and move desired version.
                                    try
                                    {
                                        // Rename target version to "Terraria".
                                        DirectoryInfo dir = new DirectoryInfo(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                        dir.MoveTo(OutDir);
                                        Directory.Delete(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text, true); // Encase any files where left behind.
                                    }
                                    catch (Exception) { }
                                }

                                #region Load Seperate Configurations

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
                                if (checkBox8.Checked)
                                {
                                    // Switch to the defined games config.
                                    try
                                    {
                                        // Check if defualt folder exists or not already.
                                        if (!Directory.Exists(configPath + @"\Terraria-Original"))
                                        {
                                            // Ensure an original even exists.
                                            if (Directory.Exists(configPath + @"\Terraria"))
                                            {
                                                // Async file renaming. // Backup the original file.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-Original"));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));

                                                // Asyn create directory.
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

                                            // Asyn create directory.
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
                                        if (checkBox1.Checked)
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
                                if (checkBox1.Checked)
                                {
                                    Console.WriteLine("Successfully launched Terraria v" + File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() + " Through Steam!");
                                }
                            }
                            catch (Exception error)
                            {
                                Console.WriteLine("Failed to launch Terraria v" + File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() + ": " + error.Message.ToString());
                            }
                        }
                        else
                        {
                            try
                            {
                                #region Load Seperate Configurations

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
                                if (checkBox8.Checked)
                                {
                                    // Switch to the defined games config.
                                    try
                                    {
                                        // Check if defualt folder exists or not already.
                                        if (!Directory.Exists(configPath + @"\Terraria-Original"))
                                        {
                                            // Ensure an original even exists.
                                            if (Directory.Exists(configPath + @"\Terraria"))
                                            {
                                                // Async file renaming. // Backup the original file.
                                                await Task.Run(() => RenameFolderAsync(configPath + @"\Terraria", configPath + @"\Terraria-Original"));

                                                // Async delete directory.
                                                await Task.Run(() => Directory.Delete(configPath + @"\Terraria", true));

                                                // Asyn create directory.
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

                                            // Asyn create directory.
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
                                        if (checkBox1.Checked)
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
                                if (checkBox1.Checked)
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
                        button2.Enabled = false;
                    }
                    else if (button2.Text == "Download")
                    {
                        // Check If User & Pass Are Populated
                        if (textBox2.Text != "" && textBox3.Text != "")
                        {
                            // Check If Already Downloaded
                            if (itemRow.SubItems[2].Text == "No")
                            {
                                // Disable Button
                                button2.Enabled = false;

                                // Select Tab Control
                                // tabControl1.SelectedIndex = 2;

                                // Download Version
                                String DLLLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\DepotDownloader.dll";
                                String DotNetLocation = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\dotnet.exe";
                                // Update 1.5.0, Check If Everwrite To Steam Directory Is Enabled
                                String OutDir = Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text;
                                if (checkBox2.Checked) // Use Steam Directory
                                {
                                    OutDir = Properties.Settings.Default.DepotPath;
                                    string OutDirParent = Directory.GetParent(OutDir).ToString();

                                    // Check If Client Is Already Running - Update 1.8.3
                                    bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(OutDir, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                                    if (isRunning)
                                    {
                                        // Is running
                                        foreach (var process in Process.GetProcessesByName("Terraria"))
                                        {
                                            process.Kill();

                                            // Log Item
                                            if (checkBox1.Checked)
                                            {
                                                Console.WriteLine("The Terraria process was killed to continue operations.");
                                            }
                                        }
                                    }

                                    // Get the current games version. Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1.
                                    string currentVersion = File.ReadLines(OutDir + @"\changelog.txt").First().Split(' ')[1].ToString();
                                    currentVersion = currentVersion == "1.3" ? "1.3.0.1" : currentVersion == "1.4" ? "1.4.0.1" : currentVersion;

                                    // Delete Folder
                                    try
                                    {
                                        // Ensure the target directory does not already exist.
                                        if (!Directory.Exists(OutDirParent + @"\Terraria-v" + currentVersion))
                                        {
                                            DirectoryInfo dir = new DirectoryInfo(OutDir);
                                            dir.MoveTo(OutDirParent + @"\Terraria-v" + currentVersion);
                                            Directory.Delete(OutDir, true); // Encase any files where left behind.
                                        }
                                        else
                                        {
                                            // This version already exists, delete parent.
                                            Directory.Delete(OutDir, true);
                                        }
                                    }
                                    catch (Exception) { }
                                    Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix

                                    // Check if the desired version already exists, otherwise download it.
                                    if (Directory.Exists(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text))
                                    {
                                        try
                                        {
                                            // Rename target version to "Terraria".
                                            DirectoryInfo dir = new DirectoryInfo(OutDirParent + @"\Terraria-v" + itemRow.SubItems[0].Text);
                                            dir.MoveTo(OutDir);
                                            Directory.Delete(OutDir, true); // Encase any files where left behind.
                                        }
                                        catch (Exception) { }
                                        Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix

                                        // Reload List
                                        ReloadList();

                                        // End the sub and prevent further downloads.
                                    }
                                }

                                // Check to see if this is a github repo.
                                if (itemRow.SubItems[1].Text.ToLower().Contains("github"))
                                {
                                    try
                                    {
                                        // Log Item
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("Github download for Terraria-v" + itemRow.SubItems[0].Text + " initiated.");
                                        }

                                        // Create an outpath.
                                        Directory.CreateDirectory(OutDir);

                                        // Extract the owwner and repo names.
                                        string repoOwner = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[1]; // Fix 1.8.5.4: Added Filter For "\t" To Seperate Git From GUI
                                        string repoName = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[2]; // Fix 1.8.5.4: Added Filter For "\t" To Seperate Git From GUI

                                        // Get the path name to the desired repo sub-directory.
                                        Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(repoName));
                                        var repositoryReadme = await client.Repository.Content.GetReadme(repoOwner, repoName);

                                        // Show Warning
                                        if (MessageBox.Show(repositoryReadme.Content + "\n" + "Do you wish to continue ?", "Repository Readme:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                        {
                                            // Log Item
                                            if (checkBox1.Checked)
                                            {
                                                Console.WriteLine("User declined the readme terms.");
                                            }

                                            // Clean files.
                                            try
                                            {
                                                Directory.Delete(OutDir, true);
                                            }
                                            catch (Exception) { }
                                            Directory.CreateDirectory(OutDir);

                                            return;
                                        }

                                        var repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName);
                                        var directoryName = repositoryContents.FirstOrDefault(c => c.Path == "Terraria-v" + itemRow.SubItems[0].Text).Name;

                                        // Get the url to the desired file to download.
                                        repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName, directoryName);
                                        var downloadUrl = repositoryContents.FirstOrDefault().DownloadUrl;

                                        // Log Item
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " found! Downloading." + downloadUrl);
                                        }

                                        // Start github download.
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
                                                MessageBox.Show(OutDir + @"\" + directoryName);
                                            }
                                        }
                                        catch (Exception) { }

                                        // Log Item
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " download completed!");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // No repo file found, log it.
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("ERROR: This repository contans no versions that match: \"" + itemRow.SubItems[0].Text + "\"!");
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
                                    String EscapedPassword = Regex.Replace(textBox3.Text, @"[%|<>&^]", @"^$&"); // Escape Any CMD Special Characters If Any Exist // Update 1.8.5.2 Fix
                                    String Arg = "dotnet " + "\"" + DLLLocation + "\"" + " -app 105600 -depot 105601 -manifest " + ManifestID + " -username " + textBox2.Text + " -password " + EscapedPassword + " -dir " + "\"" + OutDir + "\"" + ((checkBox5.Checked) ? " -remember-password" : "");

                                    // Start Download
                                    try
                                    {
                                        // Start Download Process
                                        ExecuteCmd.ExecuteCommandAsync(Arg);

                                        // Log Item
                                        if (checkBox1.Checked)
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
                            button2.Enabled = false;

                            // Display Error
                            Console.WriteLine("ERROR: Please enter steam username / password");
                        }
                    }
                }
            }
        }

        // Download App Via ToolStrip
        private async void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Get Each Row
            foreach (ListViewItem itemRow in this.listView1.Items)
            {
                // Get Selected Item
                if (itemRow.Focused)
                {
                    // Check If User & Pass Are Populated
                    if (textBox2.Text != "" && textBox3.Text != "")
                    {
                        // Check If Already Downloaded
                        if (itemRow.SubItems[2].Text == "No")
                        {
                            // Disable Button
                            button2.Enabled = false;

                            // Select Tab Control
                            // tabControl1.SelectedIndex = 2;

                            // Download Version
                            String DLLLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\DepotDownloader.dll";
                            String DotNetLocation = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\dotnet\dotnet.exe";
                            // Update 1.5.0, Check If Everwrite To Steam Directory Is Enabled
                            String OutDir = Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text;
                            if (checkBox2.Checked) // Use Steam Directory
                            {
                                OutDir = Properties.Settings.Default.DepotPath;

                                // Check If Client Is Already Running - Update 1.8.3
                                bool isRunning = Process.GetProcessesByName("Terraria").FirstOrDefault(p => p.MainModule.FileName.StartsWith(OutDir, StringComparison.InvariantCultureIgnoreCase)) != default(Process);
                                if (isRunning)
                                {
                                    // Is running
                                    foreach (var process in Process.GetProcessesByName("Terraria"))
                                    {
                                        process.Kill();

                                        // Log Item
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("The Terraria process was killed to continue operations.");
                                        }
                                    }
                                }

                                // Delete Folder
                                try
                                {
                                    Directory.Delete(OutDir, true);
                                }
                                catch (Exception) { }
                                Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
                            }

                            // Check to see if this is a github repo.
                            if (itemRow.SubItems[1].Text.ToLower().Contains("github"))
                            {
                                try
                                {
                                    // Log Item
                                    if (checkBox1.Checked)
                                    {
                                        Console.WriteLine("Github download for Terraria-v" + itemRow.SubItems[0].Text + " initiated.");
                                    }

                                    // Create an outpath.
                                    Directory.CreateDirectory(OutDir);

                                    // Extract the owwner and repo names.
                                    string repoOwner = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[1]; // Fix 1.8.5.4: Added Filter For "\t" To Seperate Git From GUI
                                    string repoName = itemRow.SubItems[1].Text.Split('\t')[2].Split('/')[2]; // Fix 1.8.5.4: Added Filter For "\t" To Seperate Git From GUI

                                    // Get the path name to the desired repo sub-directory.
                                    Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(repoName));
                                    var repositoryReadme = await client.Repository.Content.GetReadme(repoOwner, repoName);

                                    // Show Warning
                                    if (MessageBox.Show(repositoryReadme.Content + "\n" + "Do you wish to continue ?", "Repository Readme:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    {
                                        // Log Item
                                        if (checkBox1.Checked)
                                        {
                                            Console.WriteLine("User declined the readme terms.");
                                        }

                                        // Clean files.
                                        try
                                        {
                                            Directory.Delete(OutDir, true);
                                        }
                                        catch (Exception) { }
                                        Directory.CreateDirectory(OutDir);

                                        return;
                                    }

                                    var repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName);
                                    var directoryName = repositoryContents.FirstOrDefault(c => c.Path == "Terraria-v" + itemRow.SubItems[0].Text).Name;

                                    // Get the url to the desired file to download.
                                    repositoryContents = await client.Repository.Content.GetAllContents(repoOwner, repoName, directoryName);
                                    var downloadUrl = repositoryContents.FirstOrDefault().DownloadUrl;

                                    // Log Item
                                    if (checkBox1.Checked)
                                    {
                                        Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " found! Downloading." + downloadUrl);
                                    }

                                    // Start github download.
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
                                            MessageBox.Show(OutDir + @"\" + directoryName);
                                        }
                                    }
                                    catch (Exception) { }

                                    // Log Item
                                    if (checkBox1.Checked)
                                    {
                                        Console.WriteLine("Terraria-v" + itemRow.SubItems[0].Text + " download completed!");
                                    }
                                }
                                catch (Exception)
                                {
                                    // No repo file found, log it.
                                    if (checkBox1.Checked)
                                    {
                                        Console.WriteLine("ERROR: This repository contans no versions that match: \"" + itemRow.SubItems[0].Text + "\"!");
                                    }

                                    // Process Failed, Delete Folder
                                    try
                                    {
                                        Directory.Delete(OutDir, true);
                                    }
                                    catch (Exception) { }
                                    Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix}
                                }
                            }
                            else
                            {
                                // Proceed to download through steam.
                                String ManifestID = itemRow.SubItems[1].Text;
                                String EscapedPassword = Regex.Replace(textBox3.Text, @"[%|<>&^]", @"^$&"); // Escape Any CMD Special Characters If Any Exist // Update 1.8.5.2 Fix
                                String Arg = "dotnet " + "\"" + DLLLocation + "\"" + " -app 105600 -depot 105601 -manifest " + ManifestID + " -username " + textBox2.Text + " -password " + EscapedPassword + " -dir " + "\"" + OutDir + "\"" + ((checkBox5.Checked) ? " -remember-password" : "");

                                // Start Download
                                try
                                {
                                    // Start Download Process
                                    ExecuteCmd.ExecuteCommandAsync(Arg);

                                    // Log Item
                                    if (checkBox1.Checked)
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
                        button2.Enabled = false;

                        // Display Error
                        Console.WriteLine("ERROR: Please enter steam username / password");
                    }
                }
            }
        }
        #endregion

        #region Checkbox Settings

        // Update Checkbox Config
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Properties.Settings.Default.LogActions = true;
            }
            else if (!checkBox1.Checked)
            {
                Properties.Settings.Default.LogActions = false;
            }
        }

        // Show Prompt Warning
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && Properties.Settings.Default.UseSteamDir == false)
            {
                // Show Warning
                if (MessageBox.Show("This will download game versions to your steamapps." + "\n" + "Do you want to continue ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // Cancle Prompt
                    checkBox2.Checked = false;

                    // Enable Path Changing
                    // button6.Enabled = true;

                    // Update Settings
                    Properties.Settings.Default.UseSteamDir = false;
                    Properties.Settings.Default.PathChangeEnabled = true;

                    // Update Forum

                    // Log Item
                    if (checkBox1.Checked)
                    {
                        Console.WriteLine("Use steam directory mode cancled.");
                    }
                    ReloadList();
                }
                else
                {
                    // Prompt Yes, Create Directory, Change Textbox
                    // Define the game path based on the registry rather then a hardcoded path encase game was installed elseware. - Added 1.8.5.4.

                    // Define varibles.
                    string backupLocation = textBox1.Text;
                    string installLocation;

                    // Try to read registry key.
                    try
                    {
                        // Check if game location is found.
                        if (GetGameLocation() != "")
                        {
                            // Update install location.
                            installLocation = GetGameLocation();
                        }
                        else
                        {
                            // Cancle operations and exit void.
                            MessageBox.Show("ERROR: Unable to find the default install location! Try reinstalling your game!", "ERROR: TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                            // Key does not exist, log item.
                            if (checkBox1.Checked)
                            {
                                Console.WriteLine("ERROR: Unable to find the default install location! Try reinstalling your game!");
                            }

                            checkBox2.Checked = false;
                            textBox1.Text = backupLocation;
                            return;
                        }

                        textBox1.Text = backupLocation;
                    }
                    catch (Exception) // Handle exception.
                    {
                        // Key does not exist, log item.
                        if (checkBox1.Checked)
                        {
                            Console.WriteLine("ERROR: Something went wrong while reading the registry! Try reinstalling your game!");
                        }

                        // Cancle operations and exit void.
                        checkBox2.Checked = false;
                        textBox1.Text = backupLocation;
                        return;
                    }

                    // Prompt Yes, Create Directory, Change Textbox
                    if (!Directory.Exists(Directory.GetParent(installLocation).FullName))
                    {
                        Directory.CreateDirectory(Directory.GetParent(installLocation).FullName);
                    }
                    textBox1.Text = Directory.GetParent(installLocation).FullName;

                    // Disable Path Changing
                    // button6.Enabled = false;

                    // Update Settings
                    Properties.Settings.Default.DepotPath = Directory.GetParent(installLocation).FullName;
                    Properties.Settings.Default.UseSteamDir = true;
                    Properties.Settings.Default.PathChangeEnabled = false;

                    // Update Forum

                    // Log Item
                    if (checkBox1.Checked)
                    {
                        Console.WriteLine("Use steam directory mode enabled!");
                    }
                    ReloadList();
                }
            }
            if (!checkBox2.Checked && Properties.Settings.Default.UseSteamDir == true)
            {
                // Checkbox Unchecked, Reset Textbox To Defualt Dir
                textBox1.Text = Application.StartupPath + @"\TerrariaDepots";

                // Enable Path Changing
                button6.Enabled = true;

                // Update Settings
                Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
                Properties.Settings.Default.UseSteamDir = false;
                Properties.Settings.Default.PathChangeEnabled = true;

                // Update Forum

                // Log Item
                if (checkBox1.Checked)
                {
                    Console.WriteLine("Use steam directory mode disabled!");
                }
                ReloadList();
            }
        }

        // Tooltip Contols
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Tooltips
            if (checkBox3.Checked)
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
        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Update Check
            if (checkBox4.Checked)
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
        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable SaveLogin
            if (checkBox5.Checked)
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
        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            // Check if checkbox was checked or not.
            if (checkBox6.Checked)
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
        private void CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            // Define the registry paths.
            const string keyPath = @"Software\Terraria";
            const string subKeyName = "Bunny";

            // Check if checkbox was checked or not.
            if (checkBox7.Checked)
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
                            if (checkBox1.Checked)
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
                            if (checkBox1.Checked)
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
                        if (checkBox1.Checked)
                        {
                            Console.WriteLine("Successfully disabled the collectors edition!");
                        }
                    }
                }
            }
        }

        // User separate configs for each verison.
        private async void CheckBox8_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Tooltips
            if (checkBox8.Checked)
            {
                // Enable Tooltips
                Properties.Settings.Default.UseSeparateConfigs = true;
            }
            else
            {
                // Disable Tooltips
                Properties.Settings.Default.UseSeparateConfigs = false;

                #region Load Defualt Configuration

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
                        // No defualt directory found. Create one.
                        await Task.Run(() => Directory.CreateDirectory(configPath + @"\Terraria"));
                    }

                    // Log Item
                    if (checkBox1.Checked)
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
    }
}