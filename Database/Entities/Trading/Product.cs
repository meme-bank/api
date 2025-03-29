using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Product
    {
        public ItemBlueprint ItemBlueprint { get; set; }
        public int SellerId { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
    }
}