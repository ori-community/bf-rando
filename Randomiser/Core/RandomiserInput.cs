using BepInEx.Configuration;
using OriModding.BF.InputLib;
using UnityEngine;

namespace Randomiser;

public class RandomiserInput
{
    public static ConfigEntry<CustomInput> OpenTeleport { get; private set; }
    public static ConfigEntry<CustomInput> ShowProgress { get; private set; }
    public static ConfigEntry<CustomInput> ShowLastPickup { get; private set; }
    public static ConfigEntry<CustomInput> ShowGoal { get; private set; }
    public static ConfigEntry<CustomInput> ShowSkillClue { get; private set; }

    internal static void Initialise(RandomiserMod randomiserMod, Plugin plugin)
    {
        OpenTeleport = plugin.BindAndRegister(randomiserMod, "Randomiser", "Teleport",
            new CustomInput().AddChord(KeyCode.LeftAlt, KeyCode.R));

        ShowProgress = plugin.BindAndRegister(randomiserMod, "Randomiser", "Show Progress",
            new CustomInput().AddChord(KeyCode.LeftAlt, KeyCode.P));

        ShowLastPickup = plugin.BindAndRegister(randomiserMod, "Randomiser", "Show Last Pickup",
            new CustomInput().AddChord(KeyCode.LeftAlt, KeyCode.T));

        ShowGoal = plugin.BindAndRegister(randomiserMod, "Randomiser", "Show Goal",
            new CustomInput().AddChord(KeyCode.LeftAlt, KeyCode.Alpha5));

        ShowSkillClue = plugin.BindAndRegister(randomiserMod, "Randomiser", "Show Skill Clue",
            new CustomInput().AddChord(KeyCode.LeftAlt, KeyCode.Alpha6));
    }
}
