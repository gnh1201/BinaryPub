using System;

namespace Catswords.DataType.Client.Model
{
    class AndroidPermission
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Severity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
