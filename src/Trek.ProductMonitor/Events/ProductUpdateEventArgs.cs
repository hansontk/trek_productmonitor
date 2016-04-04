using System;
using Trek.ProductMonitor.ViewModels;

namespace Trek.ProductMonitor.Events
{
    public class ProductUpdateEventArgs: EventArgs
    {
        public ProductUpdateItem Item { get; set; }

        public ProductUpdateEventArgs(ProductUpdateItem item)
        {
            Item = item;
        }
    }
}
