using System.ComponentModel.DataAnnotations;
using MembankCore.Domain.Entities.Economic;

namespace MembankCore.Domain.Entities.Stocks {
  public class Stock {
    [Required]
    [Key]
    public required string Ticker { get; set; } // Например, "NBM" или "LVK"
    public int IssuerId { get; set; } // Компания, которая выпустила акции

    public decimal TotalSupply { get; set; } // Общее кол-во выпущенных акций
    public decimal CurrentPrice { get; set; }


    public Currency? Currency { get; set; }
    public required string CurrencyId { get; set; }
  }
}
