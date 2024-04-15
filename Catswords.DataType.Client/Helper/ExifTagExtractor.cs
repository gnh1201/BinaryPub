using Catswords.DataType.Client.Model;
using MetadataExtractor;
using System;
using System.Collections.Generic;

namespace Catswords.DataType.Client.Helper
{
    class ExifTagExtractor
    {
        private string FilePath;

        public ExifTagExtractor(string filePath)
        {
            FilePath = filePath;
        }

        public List<ExifTag> GetTags()
        {
            List<ExifTag> tags = new List<ExifTag>();

            try {
                IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(FilePath);
                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        tags.Add(new ExifTag
                        {
                            Section = directory.Name,
                            Name = tag.Name,
                            Description = tag.Description.ToString()
                        });
                    }
                }
            }
            catch
            {
                // nothing
            }

            return tags;
        }
    }
}
