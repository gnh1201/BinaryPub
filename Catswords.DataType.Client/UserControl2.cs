using Catswords.DataType.Client.Helper;
using Catswords.DataType.Client.Model;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class UserControl2 : UserControl
    {
        private UserControl1 Parent;
        private HashInfo CalculatedHashInfo = new HashInfo();

        public UserControl2(UserControl1 parent)
        {
            InitializeComponent();
            Parent = parent;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            new Task(() =>
            {
                var hasher = new FileHasher(Parent.FilePath);

                txtExtension.Invoke(new MethodInvoker(delegate
                {
                    txtExtension.Text = hasher.GetExtension();
                    CalculatedHashInfo.Extension = txtExtension.Text;
                }));

                txtMagic.Invoke(new MethodInvoker(delegate
                {
                    txtMagic.Text = hasher.GetMagic();
                    CalculatedHashInfo.Extension = txtMagic.Text;
                }));

                txtHashMd5.Invoke(new MethodInvoker(delegate
                {
                    txtHashMd5.Text = hasher.GetMD5();
                    CalculatedHashInfo.MD5 = txtHashMd5.Text;
                }));

                txtHashSha1.Invoke(new MethodInvoker(delegate
                {
                    txtHashSha1.Text = hasher.GetSHA1();
                    CalculatedHashInfo.SHA1 = txtHashSha1.Text;
                }));

                txtHashCrc32.Invoke(new MethodInvoker(delegate
                {
                    txtHashCrc32.Text = hasher.GetCRC32();
                    CalculatedHashInfo.CRC32 = txtHashCrc32.Text;
                }));

                txtHashSha256.Invoke(new MethodInvoker(delegate
                {
                    txtHashSha256.Text = hasher.GetSHA256();
                    CalculatedHashInfo.SHA256 = txtHashSha256.Text;
                }));

                txtInfoHash.Invoke(new MethodInvoker(delegate
                {
                    txtInfoHash.Text = hasher.GetInfoHash();
                    CalculatedHashInfo.InfoHash = txtInfoHash.Text;
                }));

                txtSsdeep.Invoke(new MethodInvoker(delegate
                {
                    txtSsdeep.Text = hasher.GetSSDEEP();
                    CalculatedHashInfo.SSDEEP = txtSsdeep.Text;
                }));

                CalculatedHashInfo.CreatedAt = DateTime.Now;
                CalculatedHashInfo.UpdatedAt = CalculatedHashInfo.CreatedAt;
            }).Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ByteViewer bv = new ByteViewer();
            bv.SetFile(Parent.FilePath); // or SetBytes

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
            Process.Start("https://www.virustotal.com/gui/file/" + CalculatedHashInfo.SHA256);
        }
    }
}
