using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Data.Entities.Core;
using MembankCore.Data.Entities.Economic;

namespace MembankCore.Data.Entities.Trading {
  public class Service {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    public required Guid PhotoId { get; set; }
    [ForeignKey("PhotoId")]
    public required Photo Photo { get; set; }
    public ServiceType Type { get; set; } = ServiceType.Once;
    public int ProviderId { get; set; }
    public decimal? Price { get; set; }
    public string? CurrencyId { get; set; }
    [ForeignKey("CurrencyId")]
    public Currency? Currency { get; set; }
    public List<Category> Categories { get; set; } = new List<Category>();
    public TimeSpan? Duration { get; set; } // Nullable for once services or subscription services with no duration or end date
    [Required]
    public DateTime PublishedAt { get; set; }
    public bool IsOtherActivate { get; set; } // Активируется вне платформы

    public ICollection<ProvideService> ProvideServices { get; set; } = new List<ProvideService>();
  }

  public enum ServiceType {
    Once,
    Subscription,
  }
}
