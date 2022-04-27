using HarmonyLib;
using OriDeModLoader;
using Sein.World;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.Awake))]
    internal class InventoryManagerKeyHintsAwake
    {
        public static MessageBox waterVeinClueText;
        public static MessageBox gumonSealClueText;
        public static MessageBox sunstoneClueText;

        private static void Postfix(InventoryManager __instance)
        {
            waterVeinClueText = Object.Instantiate(__instance.EnergyUpgradesText);
            waterVeinClueText.transform.position = __instance.GinsoTreeKey.transform.position + Vector3.down * 0.55f;
            waterVeinClueText.transform.SetParent(__instance.GinsoTreeKey.transform);
            gumonSealClueText = Object.Instantiate(__instance.EnergyUpgradesText);
            gumonSealClueText.transform.position = __instance.ForlornRuinsKey.transform.position + Vector3.down * 0.55f;
            gumonSealClueText.transform.SetParent(__instance.ForlornRuinsKey.transform);
            sunstoneClueText = Object.Instantiate(__instance.EnergyUpgradesText);
            sunstoneClueText.transform.position = __instance.MountHoruKey.transform.position + Vector3.down * 0.55f;
            sunstoneClueText.transform.SetParent(__instance.MountHoruKey.transform);
        }
    }

    [HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.UpdateItems))]
    internal class InventoryManagerKeyHintsUpdateItems
    {
        private static void Postfix()
        {
            InventoryManagerKeyHintsAwake.waterVeinClueText.SetMessage(new MessageDescriptor(GetKeyLabel(Keys.GinsoTree, Randomiser.Seed.Clues.WaterVein, Randomiser.Inventory.waterVeinShards)));
            InventoryManagerKeyHintsAwake.gumonSealClueText.SetMessage(new MessageDescriptor(GetKeyLabel(Keys.ForlornRuins, Randomiser.Seed.Clues.GumonSeal, Randomiser.Inventory.gumonSealShards)));
            InventoryManagerKeyHintsAwake.sunstoneClueText.SetMessage(new MessageDescriptor(GetKeyLabel(Keys.MountHoru, Randomiser.Seed.Clues.Sunstone, Randomiser.Inventory.sunstoneShards)));
        }

        private static string GetKeyLabel(bool ownsKey, Clues.Clue clue, int shardCount)
        {
            if (ownsKey)
                return "";

            switch (Randomiser.Seed.KeyMode)
            {
                case KeyMode.Clues:
                    if (!clue.owned && clue.revealed)
                        return Strings.Get("AREA_" + clue.area);

                    return "";

                case KeyMode.Shards:
                    return $"{shardCount}/{Randomiser.Seed.ShardsRequiredForKey}";

                default:
                    return "";
            }
        }
    }
}
