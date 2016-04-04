using Microsoft.WindowsAzure.Storage.Table;

namespace Trek.ProductMonitor.Models
{
    public class Vendor : TableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Label => $"{Name}({Code}) - {Description}";
    }
}
