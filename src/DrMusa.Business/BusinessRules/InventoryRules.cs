namespace DrMusa.Business.BusinessRules;

/// <summary>Inventory-related business rules for stock threshold checks.</summary>
public static class InventoryRules
{
    public static bool IsLowStock(int currentStock, int minimumStock)
        => currentStock <= minimumStock;

    public static bool IsOutOfStock(int currentStock)
        => currentStock <= 0;

    public static bool CanFulfillOrder(int currentStock, int requestedQuantity)
        => currentStock >= requestedQuantity;
}
