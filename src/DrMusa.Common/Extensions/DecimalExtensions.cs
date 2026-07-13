namespace DrMusa.Common.Extensions;

public static class DecimalExtensions
{
    public static string ToCurrency(this decimal value, string symbol = "PKR")
        => $"{symbol} {value:N2}";

    public static decimal ApplyDiscount(this decimal value, decimal discountPercent)
        => value - (value * discountPercent / 100);

    public static decimal ApplyTax(this decimal value, decimal taxPercent)
        => value + (value * taxPercent / 100);
}
