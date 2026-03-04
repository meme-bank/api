using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Stocks;

public class Stock {
  public string Ticker { get; private set; } // Например, "NBM" или "LVK"
  public int IssuerId { get; private set; } // Компания, которая выпустила акции

  public decimal TotalSupply { get; private set; } // Общее кол-во выпущенных акций
  public Money CurrentPrice { get; private set; }

  public virtual Currency Currency { get; private set; }
  public string CurrencyId => CurrentPrice.CurrencyId;

  protected Stock() { }

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
