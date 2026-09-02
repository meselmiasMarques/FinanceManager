using MudBlazor;

namespace FinanceManager.Web.Theme;

/// <summary>Tema MudBlazor com a paleta do FinanceManager (claro e escuro).</summary>
public static class AppTheme
{
    public const string IncomeColor = "#1c9d63";
    public const string ExpenseColor = "#d8433a";

    public static readonly MudTheme Instance = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2f6df0",
            Secondary = "#5b6472",
            AppbarBackground = "#141c2b",
            AppbarText = "#ffffff",
            DrawerBackground = "#141c2b",
            DrawerText = "#aeb8c8",
            DrawerIcon = "#aeb8c8",
            Background = "#f4f6fb",
            Surface = "#ffffff",
            Success = IncomeColor,
            Error = ExpenseColor,
            TextPrimary = "#1f2733",
            TextSecondary = "#6b7686",
            LinesDefault = "#e4e8f0",
            TableLines = "#e4e8f0",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#4d86f7",
            Secondary = "#8b93a1",
            AppbarBackground = "#11161f",
            AppbarText = "#e9edf4",
            DrawerBackground = "#11161f",
            DrawerText = "#aeb8c8",
            DrawerIcon = "#aeb8c8",
            Background = "#0f141c",
            Surface = "#161c26",
            Success = "#2fb57c",
            Error = "#e0605a",
            TextPrimary = "#e9edf4",
            TextSecondary = "#9aa4b2",
            LinesDefault = "#2a3341",
            TableLines = "#2a3341",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "250px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Segoe UI", "system-ui", "-apple-system", "Roboto", "Helvetica", "Arial", "sans-serif"]
            }
        }
    };
}
