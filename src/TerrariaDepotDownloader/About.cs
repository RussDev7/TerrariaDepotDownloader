using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TerrariaDepotDownloader
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        // Update Stats
        private void Form2_Load(object sender, EventArgs e)
        {
            label8.Text = FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion; // Version
            label7.Text = FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).LegalCopyright; // Copyright
        }

        // Discord Link
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;
            System.Diagnostics.Process.Start("https://discordapp.com/users/840645038466793498/");
        }

        // Game Thread
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://forums.terraria.org/index.php?threads/terrariadepotdownloader-downgrade-to-any-version.107519/");
        }

        // Close Form2
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Donate
        private void button2_Click(object sender, EventArgs e)
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
    }
}
