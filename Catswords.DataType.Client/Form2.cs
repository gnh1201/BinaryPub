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
        private FileHash hashed;

        public Form2(UserControl1 parent)
        {
            InitializeComponent();

            this.parent = parent;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            hashed = Helper.FileHasher.Compute(parent.filePath);
            txtExtension.Text = hashed.Extension;
            txtHashMd5.Text = hashed.MD5;
            txtHashSha1.Text = hashed.SHA1;
            txtHashCrc32.Text = hashed.CRC32;
            txtHashSha256.Text = hashed.SHA256;
            txtMagic.Text = hashed.MAGIC;
            txtInfoHash.Text = hashed.InfoHash;
            txtSsdeep.Text = hashed.SSDEEP;
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
            Process.Start("https://www.virustotal.com/gui/file/" + hashed.SHA256);
        }
    }
}
