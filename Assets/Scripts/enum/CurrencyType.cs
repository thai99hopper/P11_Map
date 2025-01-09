public enum CurrencyType
{
    none,
    gold,
    gem,
    energy,
    count,
}

public static class CurrencyTypeExtensions
{
    public static string ToIndexItem(this CurrencyType currencyType)
    {
        return currencyType switch
        {
            CurrencyType.gem => "resource_gem",
            CurrencyType.gold => "resource_gold",
            CurrencyType.energy => "resource_energy",
            _ => currencyType.ToString(),
        };
    }

    public static CurrencyType ToCurrencyType(this string item_index)
    {
        return item_index switch
        {
            "resource_gem" => CurrencyType.gem,
            "resource_gold" => CurrencyType.gold,
            "resource_energy" => CurrencyType.energy,
            _ => CurrencyType.none,
        };
    }
}