using System.ComponentModel.DataAnnotations;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Stocks;

public class Stock {
  public string Ticker { get; set; } // Например, "NBM" или "LVK"
  public int IssuerId { get; set; } // Компания, которая выпустила акции

  public decimal TotalSupply { get; set; } // Общее кол-во выпущенных акций
  public Money CurrentPrice { get; set; }

  public virtual Currency Currency { get; set; }
  public string CurrencyId => CurrentPrice.CurrencyId;

  public Stock(string ticker, int issuerId, decimal supply, Money price, Currency currency) {
    if (price.CurrencyId != currency.Id)
      throw new Exception("Валюта суммы не совпадает с объектом валюты.");

    Ticker = ticker;
    IssuerId = issuerId;
    TotalSupply = supply;
    CurrentPrice = price;
    Currency = currency;
  }
}
