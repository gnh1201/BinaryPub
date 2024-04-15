using System;

namespace Catswords.DataType.Client.Model
{
    public class OpenXMLMetadata
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Category { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastPrintedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}