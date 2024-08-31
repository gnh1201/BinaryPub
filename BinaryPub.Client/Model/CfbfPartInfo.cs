namespace BinaryPub.Client.Model
{
    public class CfbfPartInfo: Timestamp
    {
        public string URI { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}
