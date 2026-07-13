namespace DrMusa.Business.BusinessRules;

/// <summary>Encapsulates pricing business rules used across modules.</summary>
public static class PricingRules
{
    public static decimal CalculateDiscount(decimal amount, decimal discountPercent)
        => amount * discountPercent / 100;

    public static decimal CalculateTax(decimal amount, decimal taxPercent)
        => amount * taxPercent / 100;

    public static decimal CalculateTotal(decimal subTotal, decimal discountPercent, decimal taxPercent)
    {
        var discount = CalculateDiscount(subTotal, discountPercent);
        var taxable = subTotal - discount;
        var tax = CalculateTax(taxable, taxPercent);
        return taxable + tax;
    }

    public static decimal CalculateChange(decimal totalAmount, decimal paidAmount)
        => paidAmount - totalAmount;

    public static decimal CalculateProfit(decimal sellingPrice, decimal purchasePrice, int quantity)
        => (sellingPrice - purchasePrice) * quantity;
}
