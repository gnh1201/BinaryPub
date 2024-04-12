﻿using System;

namespace Catswords.DataType.Client.Model
{
    public class FileHash
    {
        public string Path { get; set; }
        public string Extension { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string MAGIC { get; set; }
        public string CRC32 { get; set; }
        public string SHA256 { get; set; }
        public string InfoHash { get; set; }
        public string SSDEEP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}