using System.Globalization;

namespace FinanceManager.Web.Services;

public static class Format
{
    public static string Currency(decimal value) => value.ToString("C2", CultureInfo.CurrentCulture);

    public static string SignedCurrency(decimal value)
    {
        var prefix = value > 0 ? "+" : string.Empty;
        return prefix + value.ToString("C2", CultureInfo.CurrentCulture);
    }

    public static string Date(DateTime value) => value.ToLocalTime().ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);

    public static string DateTimeShort(DateTime value) => value.ToLocalTime().ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
}
