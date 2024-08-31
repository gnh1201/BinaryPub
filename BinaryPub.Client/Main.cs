using System.Windows.Forms;

namespace BinaryPub.Client
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            UserControl1 userControl1 = new UserControl1(this);
            this.Controls.Add(userControl1);
        }
    }
}
