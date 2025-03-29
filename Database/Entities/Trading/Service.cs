using OctopusAPI.Database.Entities.Economic;
using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Service
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Photo Photo { get; set; }
        public ServiceType Type { get; set; } = ServiceType.Once;
        public int ProviderId { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public List<Category> Categories { get; set; }
        public TimeSpan? Duration { get; set; } // Nullable for once services or subscription services with no duration or end date
        public DateTime PublishedAt { get; set; }

        public ICollection<ProvideService> ProvideServices { get; set; } = new List<ProvideService>();
    }

    public enum ServiceType
    {
        Once,
        Subscription,
    }
}