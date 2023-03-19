using BaseModLib;
using OriDeModLoader.UIExtensions;

namespace Randomiser
{
    public static class Settings
    {
        public static FloatSetting HoldBackToTPTime = new FloatSetting("randomiserTPHoldTime", 0.4f);
    }

    public class RandomiserSettingsScreen : CustomOptionsScreen
    {
        public override void InitScreen()
        {
            this.AddSlider(Settings.HoldBackToTPTime, "Teleport hold time", 0.1f, 2f, 0.1f, "How long you need to hold [Map] in order to teleport (min 0.1s, max 2s).");
        }
    }
}
