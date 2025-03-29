namespace OctopusAPI.Database.Entities.Core
{
    public class Photo
    {
        public string Id { get; set; }
        public int OwnerId { get; set; }
        public byte[]? Image { get; set; }
        public PhotoType Type { get; set; } = PhotoType.Unknown;
        public DateTime RequestedAt { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public enum PhotoType
    {
        Item, // 512x512 (1/1)
        Avatar, // 128x128 (1/1)
        Cover, // 1280x512 (5/2)
        Icon, // 64x64 (1/1)
        Unknown // Any other type (maximum 1280x1280)
    }
}