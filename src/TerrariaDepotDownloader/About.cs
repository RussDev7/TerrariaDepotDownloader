using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System;

namespace TerrariaDepotDownloader
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        #region Form Load

        // Update Stats
        private void About_Load(object sender, EventArgs e)
        {
            VersionData_Label.Text = FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion;      // Version.
            CopyrightData_Label.Text = FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).LegalCopyright; // Copyright.
        }
        #endregion

        #region Form Controls

        // Discord Link
        private void Author_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Author_LinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://discordapp.com/users/840645038466793498/");
        }

        // Game Thread
        private void GameThread_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GameThread_LinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://forums.terraria.org/index.php?threads/terrariadepotdownloader-downgrade-to-any-version.107519/");
        }

        // Close Form2
        private void Ok_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Donate
        private void Donate_Button_Click(object sender, EventArgs e)
        {
            string url = "";
            string business = "imthedude030@gmail.com";
            string description = "Donation";
            string country = "US";
            string currency = "USD";
            string text = url;
            url = string.Concat(new string[]
            {
                text,
                "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=",
                business,
                "&lc=",
                country,
                "&item_name=",
                description,
                "&currency_code=",
                currency,
                "&bn=PP%2dDonationsBF"
            });
            Process.Start(url);
        }
        #endregion
    }
}