namespace DrMusa.Common.Utilities;

public static class InvoiceNumberGenerator
{
    private static int _counter = 1;

    public static string Generate(string prefix = "INV")
    {
        var date = DateTime.Now.ToString("yyyyMMdd");
        return $"{prefix}-{date}-{_counter++:D4}";
    }
}
