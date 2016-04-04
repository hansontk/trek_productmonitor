using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trek.ProductMonitor.ViewModels
{
    public class ProductUpdateViewModel
    {
        public ProductUpdateViewModel()
        {
            Updates = new List<ProductUpdate>();
        }

        public List<ProductUpdate> Updates { get; set; }

        public static ProductUpdateViewModel Deserialize(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new ProductUpdateViewModel();
            }

            var model = JsonConvert.DeserializeObject<ProductUpdateViewModel>(value);
            return model;
        }
    }

    public class ProductUpdate
    {
        public String VendorCode { get; set; }
        public String ProductId { get; set; }
    }
}
