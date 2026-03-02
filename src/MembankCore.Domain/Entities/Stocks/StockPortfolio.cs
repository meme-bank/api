namespace MembankCore.Domain.Entities.Stocks;

public class StockPortfolio(int ownerId, Stock stock, decimal quantity) {
  public int OwnerId { get; set; } = ownerId; // Владелец акций
  public required string StockTicker { get; set; } = stock.Ticker; // Тикер акции

  public decimal Quantity { get; set; } = quantity; // Сколько акций на руках

  public Stock? Stock { get; set; } = stock;
}
