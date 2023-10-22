using BepInEx.Configuration;
using OriModding.BF.Core;

namespace Randomiser;

public static class Settings
{
    public static ConfigEntry<float> HoldBackToTPTime;
    public static ConfigEntry<bool> EnableArchipelago;

    public static void Bind(RandomiserMod plugin)
    {
        HoldBackToTPTime = plugin.Config.Bind("Randomiser", "Teleport Hold Time", 0.4f, "How long you need to hold [Map] in order to teleport (min 0.1s, max 2s).");
        EnableArchipelago = plugin.Config.Bind("Randomiser", "Enable Archipelago", false, "(Experimental) Enable Archipelago Multiworld support. Requires restart.");

        if (plugin.TryGetPlugin(OriModding.BF.ConfigMenu.PluginInfo.PLUGIN_GUID, out var configPlugin))
            ConfigureOptionsScreen(configPlugin);
    }

    private static void ConfigureOptionsScreen(BepInEx.BaseUnityPlugin configPlugin)
    {
        var configMenu = configPlugin as OriModding.BF.ConfigMenu.Plugin;
        configMenu.ConfigureSlider(HoldBackToTPTime, 0.1f, 2f, 0.1f);
    }
}
