using Catswords.DataType.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace Catswords.DataType.Client.Helper
{
    class ApkManifestExtractor
    {
        private string ApkPath;
        private string TempDirectory;
        private string ManifestPath;

        public ApkManifestExtractor(string ApkPath)
        {
            this.ApkPath = ApkPath;
        }

        public void Open()
        {
            // Create a temporary directory for extraction
            TempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDirectory);

            using (ZipArchive apkArchive = ZipFile.OpenRead(ApkPath))
            {
                foreach (ZipArchiveEntry entry in apkArchive.Entries)
                {
                    if (entry.FullName == "AndroidManifest.xml")
                    {
                        ManifestPath = Path.Combine(TempDirectory, entry.FullName);
                        entry.ExtractToFile(ManifestPath);
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

            // Read the AndroidManifest.xml file
            XmlDocument doc = new XmlDocument();
            doc.Load(ManifestPath);

            // Find all <uses-permission> elements
            XmlNodeList permissionNodes = doc.GetElementsByTagName("uses-permission");
            foreach (XmlNode node in permissionNodes)
            {
                // Extract permissions
                permissions.Add(new AndroidPermission
                {
                    Name = node.Attributes["android:name"].Value,
                    Description = "",
                    Severity = 0
                });
            }

            return permissions;
        }
    }
}
