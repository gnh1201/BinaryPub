using AndroidXml;
using Catswords.DataType.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Catswords.DataType.Client.Helper
{
    class ApkManifestExtractor
    {
        private string FilePath;
        private string TempDirectory;
        private string TargetPath;

        public ApkManifestExtractor(string filePath)
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
                    if (entry.FullName == "AndroidManifest.xml")
                    {
                        TargetPath = Path.Combine(TempDirectory, entry.FullName);
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

        public List<AndroidPermission> GetPermissions()
        {
            List<AndroidPermission> permissions = new List<AndroidPermission>();

            using (FileStream stream = File.OpenRead(TargetPath))
            {
                // Read the AndroidManifest.xml file
                var reader = new AndroidXmlReader(stream);
                XDocument doc = XDocument.Load(reader);

                // Find all <uses-permission> elements
                var permissionNodes = doc.Descendants().Where(e => e.Name.LocalName == "uses-permission");
                foreach (var node in permissionNodes)
                {
                    foreach (var attr in node.Attributes())
                    {
                        if (attr.Name.LocalName == "name")
                        {
                            permissions.Add(new AndroidPermission
                            {
                                Name = attr.Value,
                                Description = "",
                                Severity = 0,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }
            }

            return permissions;
        }
    }
}
