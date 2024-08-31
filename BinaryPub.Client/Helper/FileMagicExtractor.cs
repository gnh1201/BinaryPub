using System;
using System.IO;

namespace BinaryPub.Client.Helper
{
    public class FileMagicExtractor
    {
        private string FilePath;
        private string Error = null;

        public FileMagicExtractor(string filePath)
        {
            FilePath = filePath;
        }

        public string GetString()
        {
            string hexString = "";

            try
            {
                // 파일 열기
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    // 첫 3 바이트 읽기
                    byte[] buffer = new byte[3] { 0x00, 0x00, 0x00 };
                    int bytesRead = fs.Read(buffer, 0, 3);

                    // 16진수로 변환하여 출력
                    hexString = BitConverter.ToString(buffer).Replace("-", string.Empty).ToLower();
                }
            }
            catch (Exception ex)
            {
                hexString = "000000";
                Error = ex.Message;
            }

            return hexString;
        }

        public string GetError()
        {
            return Error;
        }
    }
}
