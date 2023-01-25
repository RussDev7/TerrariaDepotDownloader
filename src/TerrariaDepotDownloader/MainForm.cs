using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Ionic.Zip;

namespace TerrariaDepotDownloader
{
    public partial class Form1 : Form
    {
        // Say Hello To Decompilers
        private static string HelloThere = "Hello There Fellow Decompiler, This Program Was Made By D.RUSS#2430 (xXCrypticNightXx).";

        #region Main Code
        public Form1()
        {
            InitializeComponent();
            Console.SetOut(new MultiTextWriter(new ControlWriter(richTextBox1), Console.Out));
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
                // Check If Overwrite Steam Directory Is Enabled
                if (checkBox2.Checked)
                {
                    // Overwrite Steam Directory Enabled
                    Properties.Settings.Default.DepotPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam\steamapps\common\Terraria";
                }
                else
                {
                    // Overwrite Steam Directory Disabled
                    Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
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
            checkBox2.Checked = Properties.Settings.Default.OverwriteSteam;
            checkBox3.Checked = Properties.Settings.Default.ToolTips;
            checkBox4.Checked = Properties.Settings.Default.SkipUpdate;

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
            Tooltips.SetToolTip(checkBox2, "All installs overwrites Steam directory");
            Tooltips.SetToolTip(checkBox3, "Show or hide tooltips");
            Tooltips.SetToolTip(checkBox4, "Skip API update check");

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
            button6.Enabled = Properties.Settings.Default.PathChangeEnabled;

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
                                listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "Yes" }));
                            }
                            else
                            {
                                // String Does Not Contain "null", Record Like Normal
                                listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "No" }));
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

        // Reload List
        private void button3_Click(object sender, EventArgs e)
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

            // Check If Directory Contains A ChangeLog If Overwrite Steam Directory Is Enabled
            if (checkBox2.Checked)
            {
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
                            listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), "(no manifests database exists)", "N/A" }));
                        }
                        else
                        {
                            // Check If Overwrite Steam Directory Is Enabled
                            if (checkBox2.Checked)
                            {
                                // Check Game Version Via ChangeLog
                                // Added Check For 1.3 == 1.3.0.1 & 1.4 == 1.4.0.1 - Update 1.8.4 // if changelog.txt reads 1.3 & array version reads 1.3.0.1
                                if (File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == String.Concat(line.TakeWhile(c => c != ',')) || File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.3" && String.Concat(line.TakeWhile(c => c != ',')) == "1.3.0.1" || File.ReadLines(Properties.Settings.Default.DepotPath + @"\changelog.txt").First().Split(' ')[1].ToString() == "1.4" && String.Concat(line.TakeWhile(c => c != ',')) == "1.4.0.1")
                                {
                                    // String Does Not Contain "null", Record Like Normal
                                    listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "Yes" }));
                                }
                                else
                                {
                                    // String Does Not Contain "null", Record Like Normal
                                    listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "No" }));
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
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "Yes" }));
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
                                        listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "No" }));
                                    }
                                }
                                else
                                {
                                    // String Does Not Contain "null", Record Like Normal
                                    listView1.Items.Add(new ListViewItem(new string[] { String.Concat(line.TakeWhile(c => c != ',')), line.Substring(line.LastIndexOf(' ') + 1), "No" }));
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

        // Open Browse Dialogue
        private void button6_Click(object sender, EventArgs e)
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

        // Close Games & Application
        private void button1_Click(object sender, EventArgs e)
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
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Update();
        }

        // Close Via ToolStrip
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
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
        private void toolStripDropDownButton1_MouseUp(object sender, MouseEventArgs e)
        {
            // Open New Form2
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }

        // Show Password
        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            textBox3.PasswordChar = '\u0000';
        }

        // Hide Password
        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            textBox3.PasswordChar = '*';
        }

        // Open Context Menu
        private void listView1_MouseClick(object sender, MouseEventArgs e)
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

        // Remove App Tool Via ToolStrip
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Check If Removal Is Avalible
            if (!button5.Enabled)
            {
                return;
            }

            // Disable If Overwrite Steam Directory Enabled
            if (checkBox2.Checked)
            {
                MessageBox.Show("You cannot use this feature while \"Overwrite Steam Directory\" feature is enabled.", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

        // Remove All Games
        private void Button8_Click(object sender, EventArgs e)
        {
            // Check For Any Open Clients - Update 1.8.3
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

            // Disable If Overwrite Steam Directory Enabled
            if (checkBox2.Checked)
            {
                MessageBox.Show("You cannot use this feature while \"Overwrite Steam Directory\" feature is enabled.", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
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
                        // Delete Folder
                        Directory.Delete(Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text, true);

                        // Log Item
                        if (checkBox1.Checked)
                        {
                            Console.WriteLine("Removed: " + Properties.Settings.Default.DepotPath + @"\Terraria-v" + itemRow.SubItems[0].Text);
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

        // Update Button
        private void listView1_Click(object sender, EventArgs e)
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
                    }
                }
            }
        }

        // Launch Button
        private void Button2_Click(object sender, EventArgs e)
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
                        // Check If Overwrite Steam Directory Is Enabled
                        if (checkBox2.Checked)
                        {
                            try
                            {
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
                                if (checkBox2.Checked) // Overwrite Steam Directory
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
                                    Directory.Delete(OutDir, true);
                                    Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
                                }
                                String ManifestID = itemRow.SubItems[1].Text;
                                String EscapedPassword = Regex.Replace(textBox3.Text, @"[%|<>&^]", @"^$&"); // Escape Any CMD Special Characters If Any Exist // Update 1.8.5.2 Fix
                                String Arg = "dotnet " + "\"" + DLLLocation + "\"" + " -app 105600 -depot 105601 -manifest " + ManifestID + " -username " + textBox2.Text + " -password " + EscapedPassword + " -dir " + "\"" + OutDir + "\"";

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
                                    Directory.Delete(OutDir, true);
                                    Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
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
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
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
                            if (checkBox2.Checked) // Overwrite Steam Directory
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
                                Directory.Delete(OutDir, true);
                                Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
                            }
                            String ManifestID = itemRow.SubItems[1].Text;
                            String EscapedPassword = Regex.Replace(textBox3.Text, @"[%|<>&^]", @"^$&"); // Escape Any CMD Special Characters If Any Exist // Update 1.8.5.2 Fix
                            String Arg = "dotnet " + "\"" + DLLLocation + "\"" + " -app 105600 -depot 105601 -manifest " + ManifestID + " -username " + textBox2.Text + " -password " + EscapedPassword + " -dir " + "\"" + OutDir + "\"";

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
                                Directory.Delete(OutDir, true);
                                Directory.CreateDirectory(OutDir); // Update 1.8.2 Fix
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

        // Remove App
        private void button5_Click(object sender, EventArgs e)
        {
            // Disable If Overwrite Steam Directory Enabled
            if (checkBox2.Checked)
            {
                MessageBox.Show("You cannot use this feature while \"Overwrite Steam Directory\" feature is enabled.", "TerrariaDepotDownloader v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

            // Edit Button
            button5.Enabled = false;
        }

        // Open Depot Folder Directory
        private void button9_Click(object sender, EventArgs e)
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

        // Update Checkbox Config
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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

        // Auto Scroll To End
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            richTextBox1.SelectionStart = richTextBox1.Text.Length;

            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }

        // Show Prompt Warning
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && Properties.Settings.Default.OverwriteSteam == false)
            {
                // Show Warning
                if (MessageBox.Show("This will overwrite your game within steamapps." + "\n" + "Do you want to continue ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // Cancle Prompt
                    checkBox2.Checked = false;

                    // Enable Path Changing
                    button6.Enabled = true;

                    // Update Settings
                    Properties.Settings.Default.OverwriteSteam = false;
                    Properties.Settings.Default.PathChangeEnabled = true;

                    // Update Forum

                    // Log Item
                    if (checkBox1.Checked)
                    {
                        Console.WriteLine("Overwrite steam directory mode cancled.");
                    }
                    ReloadList();
                }
                else
                {
                    // Prompt Yes, Create Directory, Change Textbox
                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam\steamapps\common\Terraria"))
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam\steamapps\common\Terraria");
                    }
                    textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam\steamapps\common\Terraria";

                    // Disable Path Changing
                    button6.Enabled = false;

                    // Update Settings
                    Properties.Settings.Default.DepotPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam\steamapps\common\Terraria";
                    Properties.Settings.Default.OverwriteSteam = true;
                    Properties.Settings.Default.PathChangeEnabled = false;

                    // Update Forum

                    // Log Item
                    if (checkBox1.Checked)
                    {
                        Console.WriteLine("Overwrite steam directory mode enabled!");
                    }
                    ReloadList();
                }
            }
            if (!checkBox2.Checked && Properties.Settings.Default.OverwriteSteam == true)
            {
                // Checkbox Unchecked, Reset Textbox To Defualt Dir
                textBox1.Text = Application.StartupPath + @"\TerrariaDepots";

                // Enable Path Changing
                button6.Enabled = true;

                // Update Settings
                Properties.Settings.Default.DepotPath = Application.StartupPath + @"\TerrariaDepots";
                Properties.Settings.Default.OverwriteSteam = false;
                Properties.Settings.Default.PathChangeEnabled = true;

                // Update Forum

                // Log Item
                if (checkBox1.Checked)
                {
                    Console.WriteLine("Overwrite steam directory mode disabled!");
                }
                ReloadList();
            }
        }

        // Tooltip Contols
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
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
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Tooltips
            if (checkBox4.Checked)
            {
                // Enable Tooltips
                Properties.Settings.Default.SkipUpdate = true;
            }
            else
            {
                // Disable Tooltips
                Properties.Settings.Default.SkipUpdate = false;
            }
        }
        #endregion
    }
}
