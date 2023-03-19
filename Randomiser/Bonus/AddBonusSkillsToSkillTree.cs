using System.Collections.Generic;
using BaseModLib;
using HarmonyLib;
using OriDeModLoader;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch]
    internal static class AddBonusAbilitiesToSkillTree
    {
        [HarmonyPrefix, HarmonyPatch(typeof(SkillTreeManager), nameof(SkillTreeManager.Awake))]
        private static void AddItemsAfterAwake(SkillTreeManager __instance)
        {
            var container = new GameObject("custom").transform;
            container.SetParentMaintainingLocalTransform(__instance.transform.Find("skillTree/menuItems"));

            __instance.AddItem(0, container, "skillTree/menuItems/aqua/04. airDash", BonusAbilities.ExtraAirDash, new int[0]);
            __instance.AddItem(1, container, "skillTree/menuItems/aqua/09. doubleJumpUpgrade", BonusAbilities.ExtraDoubleJump, new int[] { 4 });
            __instance.AddItem(2, container, "skillTree/menuItems/purple/03. healthEfficiency", BonusAbilities.HealthRegen, new int[] { 3, 4 });
            __instance.AddItem(3, container, "skillTree/menuItems/purple/05. energyEfficiency", BonusAbilities.EnergyRegen, new int[] { 3 });
            __instance.AddItem(4, container, "skillTree/menuItems/orange/06. cinderFlame", BonusAbilities.AttackPowerUpgrade, new int[] { 2 });
        }

        private static void AddItem(this SkillTreeManager manager, int index, Transform parent, string iconTemplatePath, BonusAbilities ability, int[] paths)
        {
            var templateObject = GetTemplate(2 - (index % 3), manager.transform).gameObject;
            
            var newItem = Object.Instantiate(templateObject);
            newItem.transform.SetParentMaintainingLocalTransform(parent);
            newItem.transform.position = new Vector3(-6.35f + index * 0.8f, -3.35f, 0f);
            newItem.name = $"{index + 1:00}. {ability}";
            SetColours(newItem.transform.Find("backgroundGlow"));

            var customItem = newItem.AddComponent<CustomSkillItem>();
            customItem.Ability = ability;

            var skillItem = newItem.GetComponent<SkillItem>();
            skillItem.RequiredAbilities.Clear();
            skillItem.RequiredItems.Clear();
            skillItem.Ability = AbilityType.BashBuff; // It is impossible to get this ability, so this signals our SkillItem as a custom one

            var iconTemplate = manager.transform.Find(iconTemplatePath).GetComponent<SkillItem>();
            skillItem.Icon.material.mainTexture = iconTemplate.Icon.material.mainTexture;
            skillItem.Icon.GetComponent<MeshFilter>().mesh = iconTemplate.Icon.GetComponent<MeshFilter>().mesh;
            skillItem.transform.Find("backgroundGlow/abilityIconAquaflame").GetComponent<Renderer>().material.mainTexture = iconTemplate.Icon.material.mainTexture;
            skillItem.transform.Find("backgroundGlow/abilityIconAquaflame").GetComponent<MeshFilter>().mesh = iconTemplate.transform.Find("backgroundGlow/abilityIconAquaflame").GetComponent<MeshFilter>().mesh;
            skillItem.LargeIcon = iconTemplate.LargeIcon;

            var nameMessageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            nameMessageProvider.SetMessage(Strings.Get("ABILITY_" + ability.ToString()));
            skillItem.NameMessageProvider = nameMessageProvider;

            var descriptionMessageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            descriptionMessageProvider.SetMessage(Strings.Get("ABILITY_DESC_" + ability.ToString()));
            skillItem.DescriptionMessageProvider = descriptionMessageProvider;

            var orange = manager.transform.Find("skillTree/menuItems/orange");

            var menuItem = newItem.GetComponent<CleverMenuItem>();
            manager.NavigationManager.MenuItems.Add(menuItem);
            for (int i = 0; i < paths.Length; i++)
                manager.NavigationManager.Navigation.AddTwoWayPath(menuItem, orange.GetChild(paths[i]).GetComponent<CleverMenuItem>());

            if (index > 0)
                manager.NavigationManager.Navigation.AddTwoWayPath(menuItem, manager.NavigationManager.MenuItems[manager.NavigationManager.MenuItems.Count - 2]);
        }

        private static Transform GetTemplate(int index, Transform menuItems)
        {
            switch (index)
            {
                case 0: return menuItems.Find("skillTree/menuItems/orange/01. quickFlame");
                case 1: return menuItems.Find("skillTree/menuItems/orange/02. sparkFlame");
                default:
                    return menuItems.Find("skillTree/menuItems/orange/03. chargeFlameBurn");
            }
        }

        private static void SetColours(Transform backgroundGlow)
        {
            // Makes it glow green
            backgroundGlow.GetChild(1).GetComponent<Renderer>().material.color = new Color(0.112f, 0.957f, 0.100f, 0.460f);
            backgroundGlow.GetChild(2).GetComponent<Renderer>().material.color = new Color(0.167f, 1.000f, 0.175f, 0.439f);
            backgroundGlow.GetChild(3).GetComponent<Renderer>().material.color = new Color(0.155f, 0.749f, 0.100f, 0.216f);
            backgroundGlow.GetChild(4).GetComponent<Renderer>().material.color = new Color(0.142f, 0.773f, 0.100f, 0.223f);
        }

        private static void AddTwoWayPath(this List<CleverMenuItemSelectionManager.NavigationData> navigation, CleverMenuItem itemA, CleverMenuItem itemB)
        {
            navigation.Add(new CleverMenuItemSelectionManager.NavigationData { From = itemA, To = itemB });
            navigation.Add(new CleverMenuItemSelectionManager.NavigationData { From = itemB, To = itemA });
        }

        private static int GetBonusAbility(BonusAbilities ability)
        {
            switch (ability)
            {
                case BonusAbilities.ExtraAirDash: return Randomiser.Inventory.extraDashes;
                case BonusAbilities.ExtraDoubleJump: return Randomiser.Inventory.extraJumps;
                case BonusAbilities.HealthRegen: return Randomiser.Inventory.healthRegen;
                case BonusAbilities.EnergyRegen: return Randomiser.Inventory.energyRegen;
                case BonusAbilities.AttackPowerUpgrade: return Randomiser.Inventory.attackUpgrades;
                default: return 0;
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SkillItem), "get_CanEarnSkill")]
        private static bool PreventBuyingCustomAbilities(SkillItem __instance, ref bool __result)
        {
            if (__instance.Ability == AbilityType.BashBuff)
            {
                __result = false;
                return HarmonyHelper.StopExecution;
            }

            return HarmonyHelper.ContinueExecution;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SkillItem), nameof(SkillItem.OnEnable))]
        private static bool HighlightBonusAbilityWhenOwned(SkillItem __instance, TransparencyAnimator ___m_animator)
        {
            if (__instance.Ability == AbilityType.BashBuff)
            {
                __instance.HasSkillItem = GetBonusAbility(__instance.GetComponent<CustomSkillItem>().Ability) > 0;
                __instance.UpdateItem();
                ___m_animator.Initialize();
                if (__instance.HasSkillItem)
                    ___m_animator.AnimatorDriver.GoToEnd();
                else
                    ___m_animator.AnimatorDriver.GoToStart();

                return HarmonyHelper.StopExecution;
            }

            return HarmonyHelper.ContinueExecution;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SkillTreeManager), nameof(SkillTreeManager.UpdateRequirementsText))]
        private static void UpdateRequirementsTextForCustomAbilities(SkillTreeManager __instance)
        {
            if (__instance.CurrentSkillItem && __instance.CurrentSkillItem.Ability == AbilityType.BashBuff)
            {
                var customAbility = __instance.CurrentSkillItem.GetComponent<CustomSkillItem>().Ability;
                int owned = GetBonusAbility(customAbility);
                if (owned > 0)
                {
                    __instance.RequirementsLineA.SetMessage(new MessageDescriptor(Strings.Get("ABILITY_OWNED_COUNT", owned)));
                }
                else
                {
                    __instance.RequirementsLineA.SetMessage(new MessageDescriptor(Strings.Get("ABILITY_FIND_TO_UNLOCK")));
                }
            }
        }
    }

    public class CustomSkillItem : MonoBehaviour
    {
        public BonusAbilities Ability;
    }
}
