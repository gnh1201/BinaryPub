using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Catswords.DataType.Client.Model;
using Force.Crc32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Catswords.DataType.Client.Helper
{
    public class FileHasher
    {
        public static FileHash Compute(string filename)
        {
            string extension = GetExtension(filename);
            return new FileHash
            {
                Extension = extension,
                MD5 = GetMD5(filename),
                SHA1 = GetSHA1(filename),
                MAGIC = GetMAGIC(filename),
                CRC32 = GetCRC32(filename),
                SHA256 = GetSHA256(filename),
                InfoHash = GetInfoHash(filename, extension)
            };
        }

        private static string GetExtension(string filename)
        {
            try
            {
                if (Path.GetExtension(filename).Length > 0)
                {
                    return Path.GetExtension(filename).Substring(1).ToUpper();
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

        private static string GetMD5(string filename)
        {
            string checksum = "";

            using (MD5 hasher = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    byte[] hash = hasher.ComputeHash(stream);
                    checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return checksum;
        }

        private static string GetSHA1(string filename)
        {
            string checksum = "";

            using (SHA1 hasher = SHA1.Create())
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    byte[] hash = hasher.ComputeHash(stream);
                    checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return checksum;
        }

        private static string GetCRC32(string filename)
        {
            string checksum = "";

            using (FileStream stream = File.OpenRead(filename))
            {
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                checksum = string.Format("{0:x}", Crc32Algorithm.Compute(ms.ToArray()));
            }

            return checksum;
        }

        private static string GetSHA256(string filename)
        {
            string checksum = "";

            using (SHA256 hasher = SHA256.Create())
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return checksum;
        }

        public static byte[] GetFileBytes(string filename, int count = 32)
        {
            byte[] buffer = new byte[count];

            using (var stream = File.OpenRead(filename))
            {
                int offset = 0;
                while (offset < count)
                {
                    try
                    {
                        int read = stream.Read(buffer, offset, count - offset);
                        if (read == 0)
                            throw new System.IO.EndOfStreamException();
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

        private static string GetMAGIC(string filename)
        {
            return FileMagic.Read(filename);
        }

        private static string GetInfoHash(string filename, string extension)
        {
            string checksum = "";

            if (extension.ToUpper() == "TORRENT")
            {
                BencodeParser parser = new BencodeParser();
                Torrent torrent = parser.Parse<Torrent>(filename);
                checksum = BitConverter.ToString(torrent.GetInfoHashBytes()).Replace("-", "").ToLowerInvariant();
            }

            return checksum;
        }

        public static string GetHexView(byte[] Data)
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
