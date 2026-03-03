namespace MembankCore.Domain.Entities.Stocks;

public class StockPortfolio(int ownerId, Stock stock, decimal quantity) {
  public int OwnerId { get; set; } = ownerId; // Владелец акций

  public decimal Quantity { get; set; } = quantity; // Сколько акций на руках

  public string StockTicker { get; set; } = stock.Ticker; // Тикер акции
  public virtual Stock Stock { get; set; } = stock;
}
