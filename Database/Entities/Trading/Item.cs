namespace OctopusAPI.Database.Entities.Trading
{
    public class Item
    {
        public ItemBlueprint ItemBlueprint { get; set; }
        public string OwnerId { get; set; }
        public DateTime OwnedAt { get; set; }
        public decimal Amount { get; set; }
    }
}