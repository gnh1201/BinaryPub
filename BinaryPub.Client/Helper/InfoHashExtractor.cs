using System;
using System.IO;

namespace BinaryPub.Client.Helper
{
    public class InfoHashExtractor
    {
        private string FilePath;

        public InfoHashExtractor(string filePath)
        {
            FilePath = filePath;
        }

        public string GetString()
        {
            string infoHashString = "";

            try
            {
                // 토렌트 파일을 바이트 배열로 읽어옴
                byte[] torrentData = File.ReadAllBytes(FilePath);

                // 토렌트 파일에서 InfoHash 추출
                byte[] infoHash = ExtractInfoHash(torrentData);

                // InfoHash를 문자열로 변환하여 출력
                infoHashString = BitConverter.ToString(infoHash).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return infoHashString;
        }

        private byte[] ExtractInfoHash(byte[] torrentData)
        {
            // 토렌트 파일의 구조에 따라 InfoHash 위치 파악
            int start = Array.IndexOf(torrentData, (byte)'4', 0);
            int end = Array.IndexOf(torrentData, (byte)'e', start);

            // InfoHash 추출
            byte[] infoHash = new byte[20];
            Array.Copy(torrentData, start + 1, infoHash, 0, 20);

            return infoHash;
        }
    }
}
