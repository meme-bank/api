namespace MembankCore.Domain.Entities.Stocks {
  public class StockPortfolio {
    public int OwnerId { get; set; } // Владелец акций
    public required string StockTicker { get; set; } // Тикер акции

    public decimal Quantity { get; set; } // Сколько акций на руках

    public Stock? Stock { get; set; }
  }
}
