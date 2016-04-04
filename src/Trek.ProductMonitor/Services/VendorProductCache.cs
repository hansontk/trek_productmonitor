using System.Collections.Specialized;
using System.Runtime.Caching;
using Trek.ProductMonitor.Common;
using Trek.ProductMonitor.Models;

namespace Trek.ProductMonitor.Services
{
    public interface IVendorProductCache
    {
        VendorProduct Get(string vendorCode, string productId);
        void Add(VendorProduct product);
    }

    public class VendorProductMemoryCache: IVendorProductCache
    {
        private readonly ObjectCache _cache;
        private readonly CacheItemPolicy _policy;

        public VendorProductMemoryCache()
        {
            _cache = new MemoryCache(AppConstants.VendorProductCacheName, GetMemoryCacheConfig());
            _policy = new CacheItemPolicy();
        }

        public NameValueCollection GetMemoryCacheConfig()
        {
            var config = new NameValueCollection //Would set this in config for larger app
            {
                {"pollingInterval", "00:01:00"},
                {"physicalMemoryLimitPercentage", "0"},
                {"cacheMemoryLimitMegabytes", "10"}
            };

            return config;
        }

        public VendorProduct Get(string vendorCode, string productId)
        {
            var key = GetCacheKey(vendorCode, productId);

            if (_cache.Contains(key))
            {
                return _cache.Get(key) as VendorProduct;
            }

            return null;
        }

        public void Add(VendorProduct product)
        {
            var key = GetCacheKey(product);

            if (!_cache.Contains(key))
            {
                _cache.Add(key, product, _policy);
            }
            else
            {
                _cache.Set(key, product, _policy);
            }
        }

        protected string GetCacheKey(string vendorCode, string productId)
        {
            return $"{vendorCode}_{productId}";
        }

        protected string GetCacheKey(VendorProduct product)
        {
            return GetCacheKey(product.VendorCode, product.ProductId.ToString());
        }
    }
}
