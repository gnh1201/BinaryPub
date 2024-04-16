using System;
using System.IO.Compression;
using System.IO;
using System.Xml;
using Catswords.DataType.Client.Model;

namespace Catswords.DataType.Client.Helper
{
    class OpenXMLExtractor
    {
        private string FilePath;
        private string TempDirectory;
        private string TargetPath;

        public OpenXMLExtractor(string filePath)
        {
            FilePath = filePath;
        }

        public void Open()
        {
            // Create a temporary directory for extraction
            TempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDirectory);

            using (ZipArchive apkArchive = ZipFile.OpenRead(FilePath))
            {
                foreach (ZipArchiveEntry entry in apkArchive.Entries)
                {
                    if (entry.FullName == "docProps/core.xml")
                    {
                        TargetPath = Path.Combine(TempDirectory, Guid.NewGuid().ToString());
                        entry.ExtractToFile(TargetPath);
                        break;
                    }
                }
            }
        }

        public void Close()
        {
            // Delete the temporary directory and its contents
            Directory.Delete(TempDirectory, true);
        }

        public OpenXMLMetadata GetMetadata()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(TargetPath);
            return ExtractMetadata(doc);
        }

        private OpenXMLMetadata ExtractMetadata(XmlDocument doc)
        {
            XmlNode _author = doc.SelectSingleNode("//dc:creator", GetNamespaceManager(doc));
            XmlNode _title = doc.SelectSingleNode("//dc:title", GetNamespaceManager(doc));
            XmlNode _subject = doc.SelectSingleNode("//dc:subject", GetNamespaceManager(doc));
            XmlNode _category = doc.SelectSingleNode("//dc:category", GetNamespaceManager(doc));
            XmlNode _keywords = doc.SelectSingleNode("//dc:keywords", GetNamespaceManager(doc));
            XmlNode _description = doc.SelectSingleNode("//dc:description", GetNamespaceManager(doc));
            XmlNode _lastModifiedBy = doc.SelectSingleNode("//cp:lastModifiedBy", GetNamespaceManager(doc));
            XmlNode _lastPrinted = doc.SelectSingleNode("//cp:lastPrinted", GetNamespaceManager(doc));
            XmlNode _created = doc.SelectSingleNode("//dcterms:created", GetNamespaceManager(doc));
            XmlNode _modified = doc.SelectSingleNode("//dcterms:modified", GetNamespaceManager(doc));

            string author = _author != null ? _author.InnerText : "Unknown";
            string title = _title != null ? _title.InnerText : "Unknown";
            string subject = _subject != null ? _subject.InnerText : "Unknown";
            string category = _category != null ? _category.InnerText : "Unknown";
            string keywords = _keywords != null ? _keywords.InnerText : "Unknown";
            string description = _description != null ? _description.InnerText : "Unknown";
            string lastModifiedBy = _lastModifiedBy != null ? _lastModifiedBy.InnerText : "Unknown";
            DateTime lastPrinted = _lastPrinted != null ? DateTime.Parse(_lastPrinted.InnerText) : DateTime.MinValue;
            DateTime created = _created != null ? DateTime.Parse(_created.InnerText) : DateTime.MinValue;
            DateTime modified = _modified != null ? DateTime.Parse(_modified.InnerText) : DateTime.MinValue;

            return new OpenXMLMetadata
            {
                Author = author,
                Title = title,
                Subject = subject,
                Category = category,
                Keyword = keywords,
                Description = description,
                CreatedAt = created,
                UpdatedAt = modified,
                LastUpdatedBy = lastModifiedBy,
                LastPrintedAt = lastPrinted
            };
        }

        private XmlNamespaceManager GetNamespaceManager(XmlDocument doc)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            nsManager.AddNamespace("dcterms", "http://purl.org/dc/terms/");
            nsManager.AddNamespace("cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            return nsManager;
        }
    }
}
