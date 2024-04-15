using Catswords.DataType.Client.Helper;
using Catswords.DataType.Client.Model;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class UserControl1 : UserControl
    {
        private ImageList imageList = new ImageList();

        public string filePath;
        public string fileMagic;
        public string fileName;
        public string fileExtension;

        public UserControl1(Form parent)
        {
            InitializeComponent();

            // Set image size
            imageList.Images.Add(Properties.Resources.data_database_icon_177024);
            imageList.Images.Add(Properties.Resources.message_bubble_conversation_speech_communication_talk_chat_icon_219299);
            imageList.Images.Add(Properties.Resources._2333410_android_os_smartphone_85588);
            imageList.Images.Add(Properties.Resources.office_18907);
            imageList.Images.Add(Properties.Resources.link_symbol_icon_icons_com_56927);

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
            fileMagic = FileMagic.Read(filePath);

            // Show file magic to the label
            label1.Text = "#0x" + fileMagic;
            if (FileMagic.Error != string.Empty)
            {
                textBox1.Text = FileMagic.Error;
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

            // Get data from Android manifest
            ExtractAndroidManifest();

            // Get data from timeline
            FetchFromTimeline();

            // Get links from file binary
            ExtractLink();
        }

        bool IsInternetConnected()
        {
            const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
            const string NCSI_TEST_RESULT = "Microsoft NCSI";
            const string NCSI_DNS = "dns.msftncsi.com";
            const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

            try
            {
                // Check NCSI test link
                var webClient = new WebClient();
                string result = webClient.DownloadString(NCSI_TEST_URL);
                if (result != NCSI_TEST_RESULT)
                {
                    return false;
                }

                // Check NCSI DNS IP
                var dnsHost = Dns.GetHostEntry(NCSI_DNS);
                if (dnsHost.AddressList.Count() < 0 || dnsHost.AddressList[0].ToString() != NCSI_DNS_IP_ADDRESS)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        private void FetchFromFileExtensionDB()
        {
            if (IsInternetConnected())

            {

                var search = new FileExtensionDB();
            search.Fetch(fileExtension);
            foreach (Indicator ind in search.Indicators)
            {
                listView1.Items.Add(new ListViewItem(new string[] { ind.CreatedAt.ToString(), ind.Content }, 0));
            }
            } else
            {
                MessageBox.Show("네트워크 상태 좋지 않습니다.");
            }

        }

        private void FetchFromTimeline()
        {
            // Request a timeline
            var search = new Timeline(Config.MASTODON_HOST, Config.MASTODON_ACCESS_TOKEN);

            // fetch data by file magic
            search.Fetch("0x" + fileMagic);

            // if PE format (ImpHash)
            if (fileMagic.StartsWith("4d5a"))
            {
                try
                {
                    string imphash = ImpHash.Calculate(filePath);
                    search.Fetch(imphash);

                    string companyInfo = FileCompany.Read(filePath);
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
                    if (fileExtension == "xlsx" || fileExtension == "pptx" || fileExtension == "docx")
                    {
                        ExtractOpenXML();
                    }

                    search.Fetch("msoffice");
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
                listView1.Items.Add(new ListViewItem(new string[] { ind.CreatedAt.ToString(), ind.Content }, 1));
            }
        }

        private void ExtractAndroidManifest()
        {
            if (fileExtension == "apk")
            {
                var extractor = new ApkManifestExtractor(filePath);
                extractor.Open();
                foreach (AndroidPermission perm in extractor.GetPermissions())
                {
                    listView1.Items.Add(new ListViewItem(new string[] { perm.CreatedAt.ToString(), perm.Name + ' ' + perm.Description }, 2));
                }
                extractor.Close();
            }
        }

        private void ExtractOpenXML()
        {
            var extractor = new OpenXMLExtractor(filePath);
            extractor.Open();

            var metadata = extractor.GetMetadata();
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Author: " + metadata.Author }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Title: " + metadata.Title }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Subject: " + metadata.Subject }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Category: " + metadata.Category }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Description: " + metadata.Description }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Created: " + metadata.CreatedAt.ToString() }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Last updated: " + metadata.UpdatedAt.ToString() }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Last updated by: " + metadata.LastUpdatedBy }, 3));
            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), "Last printed: " + metadata.LastPrintedAt }, 3));
            extractor.Close();
        }

        private void ExtractLink()
        {
            var extractor = new LinkExtractor(filePath);
            string[] links = extractor.GetStrings();
            foreach (string link in links)
            {
                listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), link }, 4));
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
