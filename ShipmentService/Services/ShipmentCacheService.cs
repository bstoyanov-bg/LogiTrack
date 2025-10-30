using Microsoft.Extensions.Caching.Distributed;
using ShipmentService.Models;
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

        private static string GetCacheKey(int id) => $"shipment:{id}";

        public async Task<Shipment?> GetShipmentAsync(int id)
        {
            var cached = await _cache.GetStringAsync(GetCacheKey(id));
            if (cached == null) return null;

            return JsonSerializer.Deserialize<Shipment>(cached);
        }

        public async Task SetShipmentAsync(Shipment shipment)
        {
            var serialized = JsonSerializer.Serialize(shipment);
            await _cache.SetStringAsync(GetCacheKey(shipment.Id), serialized, _cacheOptions);
        }

        public async Task RemoveShipmentAsync(int id)
        {
            await _cache.RemoveAsync(GetCacheKey(id));
        }
    }
}
