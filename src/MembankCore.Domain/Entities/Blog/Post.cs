namespace MembankCore.Domain.Entities.Blog;

public class Post {
  public Guid Id { get; private set; }
  public string Title { get; private set; }
  public string Content { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime UpdatedAt { get; private set; }

  public Guid BlogId { get; private set; }
  public virtual Blog Blog { get; private set; }

  protected Post() { }

  public Post(string title, string content, Guid blogId) {
    if (string.IsNullOrWhiteSpace(title))
      throw new Exception("Заголовок поста не может быть пустым.");

    if (string.IsNullOrWhiteSpace(content))
      throw new Exception("Содержание поста не может быть пустым.");

    Id = Guid.NewGuid();
    Title = title;
    Content = content;
    BlogId = blogId;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = CreatedAt;
  }

  public void Edit(string newTitle, string newContent) {
    if (string.IsNullOrWhiteSpace(newTitle) || string.IsNullOrWhiteSpace(newContent))
      throw new Exception("Заголовок и содержание не могут быть пустыми при редактировании.");

    if (Title != newTitle)
      Title = newTitle;
    if (Content != newContent)
      Content = newContent;
    if (Title == newTitle && Content == newContent) return;
    UpdatedAt = DateTime.UtcNow;
  }
}
