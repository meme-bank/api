using MembankCore.Domain.Entities.Core;

namespace MembankCore.Domain.Entities.Blog;

public class Blog {
  private readonly List<Post> _posts = new();

  public Guid Id { get; set; }

  public required string Name { get; set; }
  public string? Description { get; set; }

  public Guid? PhotoId { get; private set; }
  public virtual Photo? Photo { get; private set; }

  public int OwnerId { get; set; }
  public DateTime CreatedAt { get; set; }

  public virtual IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();

  protected Blog() { }

  public Blog(string name, int ownerId, string? description = null) {
    if (string.IsNullOrWhiteSpace(name))
      throw new Exception("Имя блога не может быть пустым.");

    Id = Guid.NewGuid();
    Name = name;
    OwnerId = ownerId;
    Description = description;
    CreatedAt = DateTime.UtcNow;
  }

  public Post CreatePost(string title, string content) {
    var post = new Post(title, content, this.Id);
    _posts.Add(post);
    return post;
  }

  public void UpdateMetadata(string name, string? description) {
    if (string.IsNullOrWhiteSpace(name))
      throw new Exception("Имя блога не может быть пустым.");

    Name = name;
    Description = description;
  }

  public void SetPhoto(Photo photo) {
    if (photo.Type != PhotoType.Avatar && photo.Type != PhotoType.Cover)
      throw new Exception("Для блога можно использовать только аватар или обложку.");

    Photo = photo;
    PhotoId = photo.Id;
  }
}
