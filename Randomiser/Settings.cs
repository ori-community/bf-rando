using BepInEx.Configuration;
using OriModding.BF.Core;

namespace Randomiser;

public static class Settings
{
    public static ConfigEntry<float> HoldBackToTPTime; // = new FloatSetting("randomiserTPHoldTime", 0.4f);
    public static ConfigEntry<bool> EnableArchipelago; // = new BoolSetting("randomiser.feature.archipelago", false);

    public static void Bind(RandomiserMod plugin)
    {
        HoldBackToTPTime = plugin.Config.Bind("Randomiser", "Teleport Hold Time", 0.4f, "How long you need to hold [Map] in order to teleport (min 0.1s, max 2s).");
        EnableArchipelago = plugin.Config.Bind("Randomiser", "Enable Archipelago", false, "(Experimental) Enable Archipelago Multiworld support. Requires restart.");

        if (plugin.TryGetPlugin(OriModding.BF.ConfigMenu.PluginInfo.PLUGIN_GUID, out OriModding.BF.ConfigMenu.Plugin configMenu))
        {
            configMenu.ConfigureSlider(HoldBackToTPTime, 0.1f, 2f, 0.1f);
        }
    }
}

//public class RandomiserSettingsScreen : CustomOptionsScreen
//{
//    public override void InitScreen()
//    {
//        this.AddSlider(Settings.HoldBackToTPTime, "Teleport hold time", 0.1f, 2f, 0.1f, "How long you need to hold [Map] in order to teleport (min 0.1s, max 2s).");
//        this.AddToggle(Settings.EnableArchipelago, "Enable Archipelago", "(Experimental) Enable Archipelago Multiworld support. Requires restart.");
//    }
//}
