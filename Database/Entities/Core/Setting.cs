namespace OctopusAPI.Database.Entities.Core
{
    public class Setting
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string? DisplayKey { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}