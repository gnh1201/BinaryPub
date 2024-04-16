namespace Catswords.DataType.Client.Model
{
    public class AndroidPermission: Timestamp
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Severity { get; set; }
    }
}
