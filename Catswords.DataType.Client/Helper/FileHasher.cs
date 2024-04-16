using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Force.Crc32;
using SsdeepNET;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Catswords.DataType.Client.Helper
{
    public class FileHasher
    {
        private string FilePath;

        public FileHasher(string filePath)
        {
            FilePath = filePath;
        }

        public string GetExtension()
        {
            try
            {
                if (Path.GetExtension(FilePath).Length > 0)
                {
                    return Path.GetExtension(FilePath).Substring(1).ToUpper();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetMD5()
        {
            string checksum = "";

            using (MD5 hasher = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(FilePath))
                {
                    byte[] hash = hasher.ComputeHash(stream);
                    checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return checksum;
        }

        public string GetSHA1()
        {
            string checksum = "";

            using (SHA1 hasher = SHA1.Create())
            {
                using (FileStream stream = File.OpenRead(FilePath))
                {
                    byte[] hash = hasher.ComputeHash(stream);
                    checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return checksum;
        }

        public string GetCRC32()
        {
            string checksum = "";

            using (FileStream stream = File.OpenRead(FilePath))
            {
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                checksum = string.Format("{0:x}", Crc32Algorithm.Compute(ms.ToArray()));
            }

            return checksum;
        }

        public string GetSHA256()
        {
            string checksum = "";

            using (SHA256 hasher = SHA256.Create())
            {
                using (FileStream stream = File.OpenRead(FilePath))
                {
                    var hash = hasher.ComputeHash(stream);
                    checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return checksum;
        }

        public byte[] GetFileBytes(int count = 32)
        {
            byte[] buffer = new byte[count];

            using (var stream = File.OpenRead(FilePath))
            {
                int offset = 0;
                while (offset < count)
                {
                    try
                    {
                        int read = stream.Read(buffer, offset, count - offset);
                        if (read == 0)
                            throw new EndOfStreamException();
                        offset += read;
                    }
                    catch (EndOfStreamException)
                    {
                        break;
                    }
                }

                System.Diagnostics.Debug.Assert(offset == count);
            }

            return buffer;
        }

        public string GetMagic()
        {
            return new FileMagicExtractor(FilePath).GetString();
        }

        public string GetInfoHash()
        {
            string checksum = "";
            string extension = GetExtension().ToLower();

            if (extension == "torrent")
            {
                BencodeParser parser = new BencodeParser();
                Torrent torrent = parser.Parse<Torrent>(FilePath);
                checksum = BitConverter.ToString(torrent.GetInfoHashBytes()).Replace("-", "").ToLowerInvariant();
            }

            return checksum;
        }

        public string GetSSDEEP()
        {
            string checksum = "";

            using (FileStream stream = File.OpenRead(FilePath))
            {
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);

                FuzzyHash fuzzyHash = new FuzzyHash();
                checksum = fuzzyHash.ComputeHash(ms.ToArray());
            }

            return checksum;
        }

        public string GetHexView(byte[] Data)
        {
            string output = "";

            StringBuilder strb = new StringBuilder();
            StringBuilder text = new StringBuilder();
            char[] ch = new char[1];
            for (int x = 0; x < Data.Length; x += 16)
            {
                text.Length = 0;
                strb.Length = 0;
                for (int y = 0; y < 16; ++y)
                {
                    if ((x + y) > (Data.Length - 1))
                        break;
                    ch[0] = (char)Data[x + y];
                    strb.AppendFormat("{0,0:X2} ", (int)ch[0]);
                    if (((int)ch[0] < 32) || ((int)ch[0] > 127))
                        ch[0] = '.';
                    text.Append(ch);
                }
                text.Append("\r\n");
                while (strb.Length < 52)
                    strb.Append(" ");
                strb.Append(text.ToString());
                output += strb.ToString();
            }

            return output;
        }
    }
}
