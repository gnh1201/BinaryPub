using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            UserControl1 userControl1 = new UserControl1(this);
            this.Controls.Add(userControl1);
        }
    }
}
