namespace OctopusAPI.Database.Entities.Blog
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}