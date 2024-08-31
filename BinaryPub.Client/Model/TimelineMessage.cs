namespace BinaryPub.Client.Model
{
    public class TimelineMessage: Timestamp
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }
}
