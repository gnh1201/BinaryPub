using Catswords.DataType.Client.Helper;
using Catswords.DataType.Client.Model;
using MetadataExtractor;
using System;
using System.Threading.Tasks;

namespace Catswords.DataType.Client
{
    public class Worker1
    {
        private UserControl1 Parent;

        public Worker1(UserControl1 parent)
        {
            Parent = parent;
        }

        public void FromFileExtension()
        {
            var search = new FileExtensionDatabase();
            search.Fetch(Parent.FileExtension);
            foreach (TimelineMessage ind in search.Indicators)
            {
                Parent.AddIndicator(ind.CreatedAt, ind.Content, 0);
            }
        }

        public void FromTimeline()
        {
            // Request a timeline
            var search = new Timeline(Config.MASTODON_HOST, Config.MASTODON_ACCESS_TOKEN);

            // fetch data by file magic
            search.Fetch("0x" + Parent.FileMagic);

            // if PE format (ImpHash)
            if (Parent.FileMagic.StartsWith("4d5a"))
            {
                try
                {
                    string imphash = ImpHash.Calculate(Parent.FilePath);
                    search.Fetch(imphash);

                    string organization = (new PeOrganizationExtractor(Parent.FilePath)).GetString();
                    search.Fetch(organization);
                    Parent.AddIndicator(DateTime.Now, "This file are distributed by " + organization, 4);
                    Parent.ShowStatus("ImpHash=" + imphash + "; Organization=" + organization);
                }
                catch (Exception ex)
                {
                    Parent.ShowStatus(ex.Message);
                }
            }

            // fetch data by file extension
            if (Parent.FileExtension.Length > 0)
            {
                search.Fetch(Parent.FileExtension);

                // if Office365 format
                if (Parent.FileExtension.StartsWith("xls") || Parent.FileExtension.StartsWith("ppt") || Parent.FileExtension.StartsWith("doc"))
                {
                    if (Parent.FileExtension == "xlsx" || Parent.FileExtension == "pptx" || Parent.FileExtension == "docx")
                    {
                        FromOpenXML();
                    }

                    search.Fetch("msoffice");
                    search.Fetch("office365");
                }
            }

            // if it contains ransomware keywords
            if (Parent.FileName.ToLower().Contains("readme") || Parent.FileName.ToLower().Contains("decrypt"))
            {
                search.Fetch("ransomware");
            }

            // if IoC (Indicators of Compomise) mode
            if (Parent.FileMagic == "58354f")    // EICAR test file header
            {
                search.Fetch("malware");
            }

            // Show the timeline
            foreach (TimelineMessage ind in search.Messages)
            {
                Parent.AddIndicator(ind.CreatedAt, ind.Content, 1);
            }
        }

        public void FromAndroidManifest()
        {
            if (Parent.FileExtension == "apk")
            {
                var extractor = new ApkManifestExtractor(Parent.FilePath);
                extractor.Open();
                foreach (AndroidPermission perm in extractor.GetPermissions())
                {
                    Parent.AddIndicator(perm.CreatedAt, perm.Name + ' ' + perm.Description, 2);
                }
                extractor.Close();
            }
        }

        public void FromOpenXML()
        {
            var extractor = new OpenXMLExtractor(Parent.FilePath);
            extractor.Open();

            var metadata = extractor.GetMetadata();
            Parent.AddIndicator(DateTime.Now, "Author: " + metadata.Author, 3);
            Parent.AddIndicator(DateTime.Now, "Title: " + metadata.Title, 3);
            Parent.AddIndicator(DateTime.Now, "Subject: " + metadata.Subject, 3);
            Parent.AddIndicator(DateTime.Now, "Category: " + metadata.Category, 3);
            Parent.AddIndicator(DateTime.Now, "Description: " + metadata.Description, 3);
            Parent.AddIndicator(DateTime.Now, "Created: " + metadata.CreatedAt.ToString(), 3);
            Parent.AddIndicator(DateTime.Now, "Last updated: " + metadata.UpdatedAt.ToString(), 3);
            Parent.AddIndicator(DateTime.Now, "Last updated by: " + metadata.LastUpdatedBy, 3);
            Parent.AddIndicator(DateTime.Now, "Last printed: " + metadata.LastPrintedAt, 3);
            extractor.Close();
        }

        public void FromLinks()
        {
            var extractor = new LinkExtractor(Parent.FilePath);
            var strings = extractor.GetStrings();
            foreach (string str in strings)
            {
                Parent.AddIndicator(DateTime.Now, str, 4);
            }
        }

        public void FromExif()
        {
            var extractor = new ExifTagExtractor(Parent.FilePath);
            var tags = extractor.GetTags();
            foreach (ExifTag tag in tags)
            {
                Parent.AddIndicator(DateTime.Now, $"{tag.Name} ({tag.Section}): {tag.Description}", 5);
            }
        }

        public void FormCfbf()
        {
            var extractor = new CfbfExtractor(Parent.FilePath);
            var parts = extractor.GetParts();
            foreach (CfbfPartInfo part in parts)
            {
                Parent.AddIndicator(DateTime.Now, $"CFBF: {part.Content} ({part.ContentType}, {part.URI})", 5);
            }
        }

        public void Run()
        {
            new Task(() =>
            {
                FromFileExtension();    // Get data from file extension database
                FromAndroidManifest();    // Get data from Android manifest
                FromTimeline();   // Get data from timeline
                FromLinks();  // Get links from file
                FromExif();  // Get EXIF tags from file
                FormCfbf();  // Get CFBF (aka. OLE) parts from file
            }).Start();
        }
    }
}
