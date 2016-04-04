using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Trek.ProductMonitor.Common;
using Trek.ProductMonitor.Events;
using Trek.ProductMonitor.Extensions;
using Trek.ProductMonitor.Models;
using Trek.ProductMonitor.ViewModels;

namespace Trek.ProductMonitor.Services
{
    /// <summary>
    /// Service use for communicating with Azure table storage and queues
    /// </summary>
    public interface IProductUpdateService
    {
        Task<IList<Vendor>> GetVendors();

        Task<VendorProduct> GetProduct(string vendorCode, string productId);

        void WatchForUpdates();

        event EventHandler<ProductUpdateEventArgs> ProductsUpdated;
    }

    public class ProductUpdateService: IProductUpdateService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly IVendorProductCache _productCache;
        private IDictionary<string, Vendor> _vendors;

        private IDictionary<string, Vendor> VendorCache
        {
            get
            {
                if (_vendors == null)
                {
                    GetVendors().Wait();
                }

                return _vendors;
            }
        }

        public event EventHandler<ProductUpdateEventArgs> ProductsUpdated;

        public ProductUpdateService(CloudStorageAccount storageAccount, IVendorProductCache productCache)
        {
            _storageAccount = storageAccount;
            _productCache = productCache;
        }

        public async Task<IList<Vendor>> GetVendors()
        {
            if (_vendors != null)
            {
                return _vendors.Values.ToList();
            }

            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(AppConstants.AzureVendorTableName);

            var query = new TableQuery<Vendor>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Vendor"));

            var results =  await table.ExecuteQueryAsync(query);
            _vendors = results.ToDictionary(k => k.Code, v => v);

            return results;
        }

        public async Task<VendorProduct> GetProduct(string vendorCode, string productId)
        {
            var product = _productCache.Get(vendorCode, productId);

            if (product != null)
            {
                return product;
            }

            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(AppConstants.AzureVendorTableName);

            var vendorFilder = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vendorCode);
            var rowFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"Product__{productId}");
            var query = new TableQuery<VendorProduct>().Where(TableQuery.CombineFilters(vendorFilder, TableOperators.And, rowFilter));
            var result = await table.ExecuteQueryAsync(query);

            product = result.FirstOrDefault();
            _productCache.Add(product);
            return product;
        }

        public void WatchForUpdates()
        {
            Task.Run(() =>
            {
                var queueClient = _storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(AppConstants.AzureProductUpdateQueue);
                CloudQueueMessage newMessage;

                while (true)
                {
                    newMessage = queue.GetMessage();
                    if (newMessage != null)
                    {
                        var model = ProductUpdateViewModel.Deserialize(newMessage.AsString);
                        Parallel.ForEach(model.Updates, ProcessUpdate);
                        queue.DeleteMessage(newMessage);
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }
            });
        }

        protected async void ProcessUpdate(ProductUpdate model)
        {
            var product = await GetProduct(model.VendorCode, model.ProductId);
            var vendor = VendorCache[model.VendorCode];

            if (product == null || vendor == null) { return; }

            var item = new ProductUpdateItem()
            {
                Vendor = vendor.Name,
                Product = product.Name,
                Description = product.Description,
                Price = product.Price
            };

            ProductsUpdated?.Invoke(this, new ProductUpdateEventArgs(item));
        }
    }
}
