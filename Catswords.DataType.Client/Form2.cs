using Catswords.DataType.Client.Model;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class Form2 : Form
    {
        private UserControl1 parent;
        private ComputeModel computedData;

        public Form2(UserControl1 parent)
        {
            InitializeComponent();

            this.parent = parent;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            computedData = Helper.FileCompute.Compute(parent.filePath);
            txtExtension.Text = computedData.Extension;
            txtHashMd5.Text = computedData.MD5;
            txtHashSha1.Text = computedData.SHA1;
            txtHashCrc32.Text = computedData.CRC32;
            txtHashSha256.Text = computedData.SHA256;
            txtMagic.Text = computedData.MAGIC;
            txtInfoHash.Text = computedData.InfoHash;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ByteViewer bv = new ByteViewer();
            bv.SetFile(parent.filePath); // or SetBytes

            Form newForm = new Form();
            newForm.Size = new System.Drawing.Size(650, 600);
            newForm.Text = "ByteViewer";
            newForm.Icon = Properties.Resources.icon;
            newForm.MinimizeBox = false;
            newForm.MaximizeBox = false;
            newForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            newForm.Controls.Add(bv);
            newForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.virustotal.com/gui/file/" + computedData.SHA256);
        }
    }
}
