using Microsoft.Extensions.Caching.Distributed;
using ShipmentService.Models;
using ShipmentService.Utils;
using System.Text.Json;

namespace ShipmentService.Services
{
    public class ShipmentCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public ShipmentCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

        }

        private static string GetCacheKey(int id) => CacheKeys.Shipment(id);

        // Get a single shipment from cache
        public async Task<Shipment?> GetShipmentAsync(int id)
        {
            var cached = await _cache.GetStringAsync(GetCacheKey(id));
            if (cached == null) return null;

            return JsonSerializer.Deserialize<Shipment>(cached);
        }

        // Store a single shipment
        public async Task SetShipmentAsync(Shipment shipment)
        {
            var serialized = JsonSerializer.Serialize(shipment);
            await _cache.SetStringAsync(GetCacheKey(shipment.Id), serialized, _cacheOptions);
        }

        // Remove a single shipment
        public async Task RemoveShipmentAsync(int id)
        {
            await _cache.RemoveAsync(GetCacheKey(id));
        }

        // Get all shipments (cached list)
        public async Task<List<Shipment>?> GetAllShipmentsAsync()
        {
            var cached = await _cache.GetStringAsync(CacheKeys.AllShipments);
            return cached == null ? null : JsonSerializer.Deserialize<List<Shipment>>(cached);
        }

        // Set all shipments
        public async Task SetAllShipmentsAsync(List<Shipment> shipments)
        {
            var serialized = JsonSerializer.Serialize(shipments);
            await _cache.SetStringAsync(CacheKeys.AllShipments, serialized, _cacheOptions);
        }

        // Remove cached list
        public async Task RemoveAllShipmentsAsync()
        {
            await _cache.RemoveAsync(CacheKeys.AllShipments);
        }
    }
}
