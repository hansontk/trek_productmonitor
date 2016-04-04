﻿using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Trek.ProductMonitor.Models
{
    public class VendorProduct: TableEntity
    {
        public Guid ProductId { get; set; }
        public string VendorCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
