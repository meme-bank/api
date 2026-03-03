using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MembankCore.Domain.Entities.Core;

public record PhotoMetadata(int Width, int Height, double AspectRatio);

public class Photo
{
  public Guid Id { get; private set; }
  public int OwnerId { get; private set; }
  public byte[]? Image { get; private set; }
  public PhotoType Type { get; private set; } = PhotoType.Unknown;
  public DateTime RequestedAt { get; private set; }
  public DateTime UploadedAt { get; private set; }

  protected Photo() { }

  public Photo(int ownerId, PhotoType type)
  {
    Id = Guid.NewGuid();
    OwnerId = ownerId;
    Type = type;
    RequestedAt = DateTime.UtcNow;
  }

  public void UploadContent(byte[] content)
  {
    if (content == null || content.Length == 0)
      throw new Exception("Файл пуст.");

    Image = content;
    UploadedAt = DateTime.UtcNow;
  }

  public bool IsUploaded => Image != null;
}

public enum PhotoType
{
  Item, // 512x512 (1/1)
  Avatar, // 128x128 (1/1)
  Cover, // 1280x512 (5/2)
  Icon, // 64x64 (1/1)
  Unknown // Any other type (maximum 1280x1280)
}
