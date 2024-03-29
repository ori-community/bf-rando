﻿using System.Collections.Generic;
using HarmonyLib;
using OriModding.BF.l10n;
using Sein.World;
using UnityEngine;

namespace Randomiser;

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

        __instance.CompletionText.transform.parent.GetComponentInChildren<InventoryItemHelpText>().HelpMessage = ScriptableObject.CreateInstance<CompletionDescriptionMessageProvider>();
        __instance.LockedMessageProvider = ScriptableObject.CreateInstance<LockedSkillMessageProvider>();
    }
}

[HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.UpdateItems))]
internal class InventoryManagerKeyHintsUpdateItems
{
    private static void Postfix()
    {
        InventoryManagerKeyHintsAwake.waterVeinClueText.SetMessage(new MessageDescriptor(GetKeyLabel(Keys.GinsoTree, 0, Randomiser.Inventory.waterVeinShards)));
        InventoryManagerKeyHintsAwake.gumonSealClueText.SetMessage(new MessageDescriptor(GetKeyLabel(Keys.ForlornRuins, 1, Randomiser.Inventory.gumonSealShards)));
        InventoryManagerKeyHintsAwake.sunstoneClueText.SetMessage(new MessageDescriptor(GetKeyLabel(Keys.MountHoru, 2, Randomiser.Inventory.sunstoneShards)));

        InventoryManager.Instance.CompletionText.SetMessage(new MessageDescriptor(GetCompletionText()));
    }

    private static string GetKeyLabel(bool ownsKey, int key, int shardCount)
    {
        if (ownsKey)
            return "";

        switch (Randomiser.Seed.KeyMode)
        {
            case KeyMode.Clues:
                var clue = ClueForKey(key);
                if (!clue.owned && clue.revealed)
                    return Strings.Get("AREA_SHORT_" + clue.area);

                return "";

            case KeyMode.Shards:
                return $"{shardCount}/{Randomiser.Seed.ShardsRequiredForKey}";

            default:
                return "";
        }
    }

    // TODO fix the whole clues thing, something is very wrong with this code.
    // Shouldn't be inventing some new id for the keys just for this screen
    private static Clues.Clue ClueForKey(int key)
    {
        switch (key)
        {
            case 0: return Randomiser.Seed.Clues.WaterVein;
            case 1: return Randomiser.Seed.Clues.GumonSeal;
            case 2: return Randomiser.Seed.Clues.Sunstone;
            default: return default;
        }
    }

    private static string GetCompletionText()
    {
        switch (Randomiser.Seed.GoalMode)
        {
            case GoalMode.ForceTrees:
                return $"{Randomiser.TreesFoundExceptSein}/10";
            case GoalMode.WorldTour:
                return $"{Randomiser.RelicsFound}/{Randomiser.Seed.RelicsRequired}";
            case GoalMode.Frags:
                return $"{Randomiser.Inventory.warmthFragments}/{Randomiser.Seed.WarmthFragmentsRequired}";
        }

        return "";
    }
}

public class CompletionDescriptionMessageProvider : MessageProvider
{
    public override IEnumerable<MessageDescriptor> GetMessages()
    {
        return new MessageDescriptor[] { new MessageDescriptor(DynamicText.BuildDetailedGoalString()) };
    }
}

public class LockedSkillMessageProvider : MessageProvider
{
    public override IEnumerable<MessageDescriptor> GetMessages()
    {
        if (!Randomiser.Inventory.skillClueFound)
        {
            yield return new MessageDescriptor(Strings.Get("UI_ABILITY_LOCKED"));
            yield break;
        }

        AbilityType? selectedSkill = InventoryManager.Instance.NavigationManager.CurrentMenuItem?.GetComponent<InventoryAbilityItem>()?.Ability;
        if (selectedSkill != null && (selectedSkill == AbilityType.Stomp || selectedSkill == AbilityType.Grenade))
        {
            var location = Randomiser.Seed.GetSkillLocation(selectedSkill.Value);
            yield return new MessageDescriptor(Strings.Get("UI_ABILITY_WITH_CLUE", Strings.Get("AREA_LONG_" + location.area.ToString())));
            yield break;
        }

        yield return new MessageDescriptor(Strings.Get("UI_ABILITY_LOCKED"));
    }
}
