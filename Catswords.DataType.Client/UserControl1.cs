using Catswords.DataType.Client.Helper;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class UserControl1 : UserControl
    {
        private ImageList imageList = new ImageList();

        public string FilePath;
        public string FileMagic;
        public string FileName;
        public string FileExtension;

        public UserControl1(Form parent)
        {
            InitializeComponent();

            // Set image size
            imageList.Images.Add(Properties.Resources.data_database_icon_177024);
            imageList.Images.Add(Properties.Resources.message_bubble_conversation_speech_communication_talk_chat_icon_219299);
            imageList.Images.Add(Properties.Resources._2333410_android_os_smartphone_85588);
            imageList.Images.Add(Properties.Resources.office_18907);
            imageList.Images.Add(Properties.Resources.link_symbol_icon_icons_com_56927);
            imageList.Images.Add(Properties.Resources.tags_icon_icons_com_73382);

            // set image list
            listView1.SmallImageList = imageList;

            // Store the file path.
            FilePath = OpenFileDialog();
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("Failed to get a file name", "Catswords.DataType.Client");
                parent.Close();
                return;
            }

            // Get first 4 bytes from the file.
            var extractor = new FileMagicExtractor(FilePath);
            FileMagic = extractor.GetString();

            // Show file magic to the label
            label1.Text = "#0x" + FileMagic;
            if (extractor.GetError() != null)
            {
                ShowStatus(extractor.GetError());
            }

            // Get file name and file extension
            try
            {
                FileExtension = Path.GetExtension(FilePath);
                FileName = Path.GetFileName(FilePath);
                if (FileExtension.Length > 0 && FileExtension.Substring(0, 1) == ".")
                {
                    FileExtension = FileExtension.Substring(1);
                }
            }
            catch
            {
                FileExtension = "";
                FileName = "";
            }

            // Run the worker
            (new Worker1(this)).Run();
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

        public void AddIndicator(DateTime dt, string Description, int ImageIndex)
        {
            if (listView1.InvokeRequired) {
                listView1.Invoke(new MethodInvoker(delegate
                {
                    listView1.Items.Add(new ListViewItem(new string[] { dt.ToString(), Description }, ImageIndex));
                }));
            }
            else
            {
                listView1.Items.Add(new ListViewItem(new string[] { dt.ToString(), Description }, ImageIndex));
            }
            
        }

        public void ShowStatus(string status)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new MethodInvoker(delegate
                {
                    textBox1.Text = status;
                }));
            }
            else
            {
                textBox1.Text = status;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://catswords.social/auth/sign_up");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form = new Form
            {
                Text = "Expert",
                FormBorderStyle = FormBorderStyle.FixedSingle,
                Icon = Properties.Resources.icon,
                MaximizeBox = false,
                MinimizeBox = false,
                Width = 450,
                Height = 560,
                BackColor = System.Drawing.SystemColors.Window
            };
            form.Controls.Add(new UserControl2(this));
            form.Show();
        }
    }
}
