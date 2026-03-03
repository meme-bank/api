using System.ComponentModel.DataAnnotations;

namespace MembankCore.Domain.Entities.Core;

public enum SettingValueType
{
  Integer,
  String,
  Bool,
  Decimal
};

public class Setting
{
  public string Key { get; private set; }
  public string? DisplayKey { get; private set; }

  public string Value { get; private set; }
  public SettingValueType ValueType { get; private set; }

  public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
  public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;
  public int? ChangedById { get; private set; }

  protected Setting() { }

  public Setting(string key, SettingValueType type, string value, string? displayKey, int? changerId)
  {
    if (!ValidateValueAsync(value, type))
      throw new ArgumentException($"'{value}' не может быть записан в {key}, так как он не соответствует типу {type}");

    Key = key;
    ValueType = type;
    DisplayKey = displayKey ?? key;
    Value = value;
    ChangedById = changerId;
  }

  public void SetValue(string newValue, int? changerId)
  {
    if (Value == newValue)
      return;
    if (!ValidateValueAsync(newValue, ValueType))
      throw new ArgumentException($"'{newValue}' не может быть записан в {Key}, так как он не соответствует типу {ValueType}");
    Value = newValue;
    ChangedById = changerId;
    ChangedAt = DateTime.UtcNow;
  }

  private static bool ValidateValueAsync(string newValue, SettingValueType valueType) => valueType switch
  {
    SettingValueType.Integer => int.TryParse(newValue, out int _),
    SettingValueType.Decimal => decimal.TryParse(newValue, out decimal _),
    SettingValueType.Bool => bool.TryParse(newValue, out bool _),
    SettingValueType.String => true,
    _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
  };
}
