namespace ShipmentService.Utils
{
    public static class CacheKeys
    {
        private const string Prefix = "logitrack";

        public static string Shipment(int id) => $"{Prefix}:shipment:{id}";
        // Can be added in future for driver caching
        // public static string Driver(int id) => $"{Prefix}:driver:{id}";
    }
}