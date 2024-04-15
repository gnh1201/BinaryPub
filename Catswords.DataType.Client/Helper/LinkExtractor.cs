using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Catswords.DataType.Client.Helper
{
    class LinkExtractor
    {
        private string FilePath;

        public LinkExtractor(string filePath)
        {
            FilePath = filePath;
        }

        private void ProcessFile(string srcfile, List<string> results)
        {
            // Read the binary file
            byte[] data = File.ReadAllBytes(srcfile);

            // Convert binary data to string
            string binaryString = Encoding.ASCII.GetString(data);

            // Regular expression to match IP address pattern
            string ipPattern = @"(?<!\d)(?:\d{1,3}\.){3}\d{1,3}(?!\d)";

            // Regular expression to match URL pattern
            string urlPattern = @"(?:http|https):\/\/[\w\-_]+(?:\.[\w\-_]+)+[\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#]";

            // Match IP addresses
            MatchCollection ipMatches = Regex.Matches(binaryString, ipPattern);
            foreach (Match match in ipMatches)
            {
                results.Add(match.Value);
            }

            // Match URLs
            MatchCollection urlMatches = Regex.Matches(binaryString, urlPattern);
            foreach (Match match in urlMatches)
            {
                results.Add(match.Value);
            }
        }

        private bool IsZipFile()
        {
            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[4];
                    int bytesRead = fs.Read(buffer, 0, 4);
                    if (bytesRead >= 4)
                    {
                        string fileSignature = BitConverter.ToString(buffer);
                        return fileSignature == "50-4B-03-04"; // "PK" signature for zip files
                    }
                    else
                    {
                        // File is too small to read signature
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public string[] GetStrings()
        {
            List<string> results = new List<string>();

            try
            {
                // Check if the file is a zip file
                bool isZip = IsZipFile();

                if (isZip)
                {
                    // Extract files from zip archive
                    string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempDir);
                    using (ZipArchive apkArchive = ZipFile.OpenRead(FilePath))
                    {
                        foreach (ZipArchiveEntry entry in apkArchive.Entries)
                        {
                            string extractedFile = Path.Combine(tempDir, Guid.NewGuid().ToString());
                            entry.ExtractToFile(extractedFile);
                            ProcessFile(extractedFile, results);
                        }
                    }

                    // Clean up temp directory
                    Directory.Delete(tempDir, true);
                }
                else
                {
                    // Process the file directly
                    ProcessFile(FilePath, results);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            return results.ToArray();
        }
    }
}
