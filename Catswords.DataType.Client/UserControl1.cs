using Catswords.DataType.Client.Helper;
using Catswords.DataType.Client.Model;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class UserControl1 : UserControl
    {
        private ImageList imageList = new ImageList();
        private Form parent;

        public string filePath;
        public string fileMagic;
        public string fileName;
        public string fileExtension;

        public UserControl1(Form parent)
        {
            InitializeComponent();

            // set parent form
            this.parent = parent;

            // Set image size
            imageList.Images.Add(Properties.Resources.data_database_icon_177024);
            imageList.Images.Add(Properties.Resources.message_bubble_conversation_speech_communication_talk_chat_icon_219299);

            // set image list
            listView1.SmallImageList = imageList;

            // Store the file path.
            filePath = OpenFileDialog();
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Failed to get a file name", "Catswords.DataType.Client");
                parent.Close();
                return;
            }

            // Get first 4 bytes from the file.
            fileMagic = Helper.FileMagic.Read(filePath);

            // Show file magic to the label
            label1.Text = "#0x" + fileMagic;
            if (Helper.FileMagic.Error != string.Empty)
            {
                textBox1.Text = Helper.FileMagic.Error;
            }

            // Get file name and file extension
            try
            {
                fileExtension = Path.GetExtension(filePath);
                fileName = Path.GetFileName(filePath);
                if (fileExtension.Length > 0 && fileExtension.Substring(0, 1) == ".")
                {
                    fileExtension = fileExtension.Substring(1);
                }
            }
            catch
            {
                fileExtension = "";
                fileName = "";
            }

            // Get data from file extension database
            FetchFromFileExtensionDB();

            // Get data from timeline
            FetchFromTimeline();

            // Get data from Android manifest
            FetchFromAndroidManifest();
        }

        private void FetchFromFileExtensionDB()
        {
            var search = new Helper.FileExtensionDB();
            search.Fetch(fileExtension);
            foreach (Indicator ind in search.Indicators)
            {
                listView1.Items.Add(new ListViewItem(new string[] { ind.CreatedAt, ind.Content }, 0));
            }
        }

        private void FetchFromTimeline()
        {
            // Request a timeline
            var search = new Helper.Timeline(Config.MASTODON_HOST, Config.MASTODON_ACCESS_TOKEN);

            // fetch data by file magic
            search.Fetch("0x" + fileMagic);

            // if PE format (ImpHash)
            if (fileMagic.StartsWith("4d5a"))
            {
                try
                {
                    string imphash = Helper.ImpHash.Calculate(filePath);
                    search.Fetch(imphash);

                    string companyInfo = Helper.FileCompany.Read(filePath);
                    search.Fetch(companyInfo);

                    textBox1.Text = "ImpHash=" + imphash + "; CompanyInfo=" + companyInfo;
                }
                catch (Exception ex)
                {
                    textBox1.Text = ex.Message;
                }
            }

            // fetch data by file extension
            if (fileExtension.Length > 0)
            {
                search.Fetch(fileExtension);

                // if Office365 format
                if (fileExtension.StartsWith("xls") || fileExtension.StartsWith("ppt") || fileExtension.StartsWith("doc"))
                {
                    search.Fetch("office365");
                }
            }

            // if it contains ransomware keywords
            if (fileName.ToLower().Contains("readme") || fileName.ToLower().Contains("decrypt"))
            {
                search.Fetch("ransomware");
            }

            // if IoC (Indicators of Compomise) mode
            if (fileMagic == "58354f")    // EICAR test file header
            {
                search.Fetch("malware");
            }

            // Show the timeline
            foreach (Indicator ind in search.Indicators)
            {
                listView1.Items.Add(new ListViewItem(new string[] { ind.CreatedAt, ind.Content }, 1));
            }
        }

        private void FetchFromAndroidManifest()
        {
            if (fileExtension == "apk")
            {
                var extractor = new ApkManifestExtractor(filePath);
                foreach (AndroidPermission perm in extractor.GetPermissions())
                {
                    listView1.Items.Add(new ListViewItem(new string[] { perm.Name, perm.Description }));
                }
            }
        }

        public string OpenFileDialog()
        {
            string filePath = null;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }

            return filePath;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://catswords.social/auth/sign_up");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form newForm = new Form2(this);
            newForm.Show();
        }
    }
}
