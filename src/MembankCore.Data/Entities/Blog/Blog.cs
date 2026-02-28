using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Data.Entities.Core;

namespace MembankCore.Data.Entities.Blog {
  public class Blog {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public required string Name { get; set; }
    public string? Description { get; set; }

    public string? PhotoId { get; set; }
    [ForeignKey("PhotoId")]
    public Photo? Photo { get; set; }
    public int OwnerId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
  }
}
