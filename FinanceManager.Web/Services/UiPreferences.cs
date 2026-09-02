using Microsoft.JSInterop;

namespace FinanceManager.Web.Services;

/// <summary>Preferências de interface persistidas em <c>localStorage</c> (tema e menu lateral).</summary>
public sealed class UiPreferences(IJSRuntime js)
{
    private const string DarkKey = "fm.darkMode";
    private const string DrawerKey = "fm.drawerOpen";

    /// <summary><c>null</c> = seguir o sistema operacional.</summary>
    public bool? DarkMode { get; private set; }
    public bool DrawerOpen { get; private set; } = true;

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        DarkMode = await GetBoolAsync(DarkKey);
        DrawerOpen = await GetBoolAsync(DrawerKey) ?? true;
    }

    public async Task SetDarkModeAsync(bool value)
    {
        DarkMode = value;
        await SetAsync(DarkKey, value);
        Changed?.Invoke();
    }

    public async Task SetDrawerOpenAsync(bool value)
    {
        DrawerOpen = value;
        await SetAsync(DrawerKey, value);
    }

    private async Task<bool?> GetBoolAsync(string key)
    {
        try
        {
            var raw = await js.InvokeAsync<string?>("localStorage.getItem", key);
            return raw switch { "true" => true, "false" => false, _ => null };
        }
        catch
        {
            return null;
        }
    }

    private async Task SetAsync(string key, bool value)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", key, value ? "true" : "false");
        }
        catch
        {
            // localStorage indisponível (aba privada, bloqueio) — segue sem persistir.
        }
    }
}
