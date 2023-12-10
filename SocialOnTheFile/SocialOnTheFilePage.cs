using SharpShell.Attributes;
using SharpShell.SharpPropertySheet;
using SocialOnTheFile.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SocialOnTheFile
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public partial class SocialOnTheFilePage: SharpPropertyPage
    {
        private string filePath;
        private string fileMagic;
        private string fileName;
        private string fileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialOnTheFilePage"/> class.
        /// </summary>
        public SocialOnTheFilePage()
        {
            InitializeComponent();

            //  Set the page title.
            PageTitle = "FileToots";

            //  Note: You can also set the icon to be used:
            //  PageIcon = Properties.Resources.SomeIcon;
        }

        /// <summary>
        /// Called when the page is initialised.
        /// </summary>
        /// <param name="parent">The parent property sheet.</param>
        protected override void OnPropertyPageInitialised(SharpPropertySheet parent)
        {
            // Store the file path.
            filePath = parent.SelectedItemPaths.First();

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

            // Request a timeline
            var search = new Helper.Timeline("catswords.social", "HDVTEfLswvSJZq5MRpim2tp7DifTcgKbMl0mBM5-uHw");

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
                search.Fetch("ioc");
            }

            // Show the timeline
            foreach (Indicator ind in search.Indicators)
            {
                listView1.Items.Add(new ListViewItem(new string[] { ind.CreatedAt, ind.Content }));
            }
        }

        /// <summary>
        /// Called when apply is pressed on the property sheet, or the property
        /// sheet is dismissed with the OK button.
        /// </summary>
        protected override void OnPropertySheetApply()
        {
            // code here
        }

        /// <summary>
        /// Called when OK is pressed on the property sheet.
        /// </summary>
        protected override void OnPropertySheetOK()
        {
            // code here
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://catswords.social/");
        }
    }
}
