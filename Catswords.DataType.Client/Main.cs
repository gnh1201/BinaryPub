using System.Windows.Forms;

namespace Catswords.DataType.Client
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
