using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MembankCore.Domain.Entities.Economic {
  public class Wallet {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public int OwnerId { get; set; }

    [Required]
    public string CurrencyId { get; set; }

    [ForeignKey("CurrencyId")]
    public Currency Currency { get; set; }

    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<TransferNote> RecivedTransferNotes { get; set; } = new List<TransferNote>();
    public ICollection<TransferNote> SentTransferNotes { get; set; } = new List<TransferNote>();

    /// <summary>
    /// Создание кошелька.
    /// </summary>
    /// <param name="currency">Объект Currency, который является валютой кошелька.</param>
    /// <param name="balance">Начальный баланс.</param>
    /// <param name="name">Имя кошелька.</param>
    /// <param name="currency">Объект Currency, который является валютой кошелька.</param>
    public Wallet(int ownerId, Currency currency, decimal balance, string name) {
      OwnerId = ownerId;
      Currency = currency;
      CurrencyId = currency.Id;
      Name = name;
      Balance = balance;
    }

    /// <summary>
    /// Пополнение баланса.
    /// </summary>
    /// <param name="amount">Количество средств для перевода (в валюте кошелька)</param>
    public void Deposit(decimal amount) {
      if (amount < 0)
        throw new ArgumentException("Сумма пополнения не может быть отрицательной.", nameof(amount));

      Balance += amount;
    }

    /// <summary>
    /// Снятие средств с проверкой остатка.
    /// </summary>
    /// <param name="amount">Количество средств для перевода (в валюте кошелька)</param>
    /// <exception cref="InvalidOperationException">Если недостаточно средств.</exception>
    public void Withdraw(decimal amount) {
      if (amount < 0)
        throw new ArgumentException("Сумма снятия не может быть отрицательной.", nameof(amount));

      if (Balance < amount)
        throw new InvalidOperationException($"Недостаточно средств на кошельке '{Name}'. Текущий баланс: {Balance}, требуется: {amount}");

      Balance -= amount;
    }
  }
}
