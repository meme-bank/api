using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Blog
{
    public class Blog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Photo? Photo { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}