using System;
using System.IO;

namespace SocialOnTheFile.Helper
{
    public static class FileMagic
    {
        public static string Error = string.Empty;

        public static string Read(string filePath)
        {
            string hexString = "";

            try
            {
                // 파일 열기
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
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
    }
}
