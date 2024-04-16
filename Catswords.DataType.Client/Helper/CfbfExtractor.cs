using Catswords.DataType.Client.Model;
using System.Collections.Generic;
using System.IO.Packaging;
using System.IO;

namespace Catswords.DataType.Client.Helper
{
    public class CfbfExtractor
    {
        private string FilePath;

        public CfbfExtractor(string filePath)
        {
            FilePath = filePath;
        }

        public List<CfbfPartInfo> GetParts()
        {
            List<CfbfPartInfo> partInfoList = new List<CfbfPartInfo>();

            if (!IsValidFormat())
            {
                return partInfoList;
            }

            using (Package package = Package.Open(FilePath, FileMode.Open, FileAccess.Read))
            {
                foreach (PackagePart part in package.GetParts())
                {
                    CfbfPartInfo partInfo = new CfbfPartInfo();
                    partInfo.URI = part.Uri.ToString();
                    partInfo.ContentType = part.ContentType;

                    using (Stream stream = part.GetStream(FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            partInfo.Content = reader.ReadToEnd();
                        }
                    }

                    partInfoList.Add(partInfo);
                }
            }

            return partInfoList;
        }

        public bool IsValidFormat()
        {
            // CFBF 파일 시그니처 확인
            byte[] signatureBytes = { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
            byte[] fileBytes = new byte[signatureBytes.Length];

            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(fileBytes, 0, signatureBytes.Length);
            }

            for (int i = 0; i < signatureBytes.Length; i++)
            {
                if (fileBytes[i] != signatureBytes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
