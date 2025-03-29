using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Database.Entities.Trading
{
    public class ProvideService
    {
        public string Id { get; set; }
        public string ServiceId { get; set; }
        public Service Service { get; set; }
        public int BuyerId { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public ProvideStatus Status { get; set; } = ProvideStatus.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public enum ProvideStatus
    {
        Active,
        Succesess, // Only for once services
        Cancel,
        Expired
    }
}