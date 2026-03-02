using System.ComponentModel.DataAnnotations;
using MembankCore.Domain.Entities.Economic;

namespace MembankCore.Domain.Entities.Stocks {
  public class Stock(string ticker, int issuerId, decimal supply, decimal price, Currency currency) {
    [Required]
    [Key]
    public required string Ticker { get; set; } = ticker; // Например, "NBM" или "LVK"
    public int IssuerId { get; set; } = issuerId; // Компания, которая выпустила акции

    public decimal TotalSupply { get; set; } = supply; // Общее кол-во выпущенных акций
    public decimal CurrentPrice { get; set; } = price;

    public Currency? Currency { get; set; } = currency;
    public required string CurrencyId { get; set; } = currency.Id;
  }
}
