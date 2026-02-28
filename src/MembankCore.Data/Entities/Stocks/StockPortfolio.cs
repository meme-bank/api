namespace MembankCore.Data.Entities.Stocks {
  public class StockPortfolio {
    public int OwnerId { get; set; } // Владелец акций
    public string StockTicker { get; set; } // Тикер акции

    public decimal Quantity { get; set; } // Сколько акций на руках

    public Stock Stock { get; set; }
  }
}
