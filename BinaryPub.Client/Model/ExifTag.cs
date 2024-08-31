namespace BinaryPub.Client.Model
{
    public class ExifTag: Timestamp
    {
        public string Section { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
