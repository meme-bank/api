using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Photo? Icon { get; set; }
        public List<Service> Services { get; set; }
        public List<ItemBlueprint> ItemBlueprints { get; set; }
    }
}