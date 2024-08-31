using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BinaryPub.Client.Helper
{
    public static class ImpHash
    {
        public static string Calculate(string filePath)
        {
            string imphash = "";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    // DOS Header 크기만큼 스킵
                    binaryReader.BaseStream.Seek(0x3C, SeekOrigin.Begin);
                    int peHeaderOffset = binaryReader.ReadInt32();

                    // PE Header로 이동
                    binaryReader.BaseStream.Seek(peHeaderOffset, SeekOrigin.Begin);

                    // Signature 확인
                    uint signature = binaryReader.ReadUInt32();
                    if (signature != 0x00004550) // "PE\0\0"
                    {
                        throw new InvalidOperationException("Invalid PE file");
                    }

                    // Optional Header에서 Import Table Offset 찾기
                    binaryReader.BaseStream.Seek(20, SeekOrigin.Current); // COFF 파일 헤더 크기만큼 스킵
                    ushort optionalHeaderSize = binaryReader.ReadUInt16();
                    binaryReader.BaseStream.Seek(42, SeekOrigin.Current); // ImageBase 크기만큼 스킵

                    int importTableOffset = peHeaderOffset + 24 + optionalHeaderSize;
                    binaryReader.BaseStream.Seek(importTableOffset, SeekOrigin.Begin);

                    // Import Table에서 imphash 생성
                    StringBuilder imphashBuilder = new StringBuilder();
                    while (true)
                    {
                        uint lookupTableRVA = binaryReader.ReadUInt32();
                        if (lookupTableRVA == 0)
                        {
                            break;
                        }

                        binaryReader.BaseStream.Seek(12, SeekOrigin.Current); // 다른 필드들을 스킵
                        binaryReader.BaseStream.Seek(4, SeekOrigin.Current); // 항상 0인 TimeDateStamp 필드를 스킵

                        imphashBuilder.Append(lookupTableRVA.ToString("X8"));
                    }

                    // MD5 해시 생성
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(imphashBuilder.ToString()));
                        imphash =  BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                    }
                }
            }

            return imphash;
        }
    }
}
