using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Economic {
  public class Wallet {
    private readonly List<TransferNote> _recivedTransferNotes = [];
    private readonly List<TransferNote> _sentTransferNotes = [];

    public Guid Id { get; private set; }
    public int OwnerId { get; private set; }

    public string CurrencyId => Balance.CurrencyId;
    public Currency Currency { get; private set; }

    public Money Balance { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public string Name { get; private set; }
    public string? Description { get; private set; }

    public IReadOnlyCollection<TransferNote> RecivedTransferNotes => _recivedTransferNotes.AsReadOnly();
    public IReadOnlyCollection<TransferNote> SentTransferNotes => _sentTransferNotes.AsReadOnly();

    /// <summary>
    /// Создание кошелька.
    /// </summary>
    /// <param name="ownerId">ID аккаунта, владеющего кошельком.</param>
    /// <param name="balance">Начальный баланс.</param>
    /// <param name="name">Имя кошелька.</param>
    /// <param name="currency">Объект <see cref="Currency"/>, который является валютой кошелька.</param>
    public Wallet(int ownerId, Currency currency, Money balance, string name) {
      if (balance.CurrencyId != currency.Id)
        throw new ArgumentException("Валюта начального баланса не совпадает с валютой кошелька.");

      OwnerId = ownerId;
      Currency = currency;
      Name = name;
      Balance = balance;
    }

    /// <summary>
    /// Пополнение баланса.
    /// </summary>
    /// <param name="amount">Количество средств для перевода (в валюте кошелька)</param>
    public void Deposit(Money amount) {
      if (amount <= 0)
        throw new ArgumentException("Сумма пополнения не может быть отрицательной.", nameof(amount));

      if (amount.CurrencyId != this.CurrencyId)
        throw new ArgumentException("Несоответствие валют при пополнении.");

      Balance += amount;
    }

    /// <summary>
    /// Снятие средств с проверкой остатка.
    /// </summary>
    /// <param name="amount">Количество средств для перевода (в валюте кошелька)</param>
    /// <exception cref="InvalidOperationException">Если недостаточно средств.</exception>
    public void Withdraw(Money amount) {
      if (amount <= 0)
        throw new ArgumentException("Сумма снятия не может быть отрицательной.", nameof(amount));

      if (amount.CurrencyId != this.CurrencyId)
        throw new ArgumentException("Несоответствие валют при снятии.");

      if (Balance < amount)
        throw new InvalidOperationException($"Недостаточно средств на кошельке '{Name}'. Текущий баланс: {Balance.Amount}, требуется: {amount.Amount}");

      Balance -= amount;
    }
  }
}
