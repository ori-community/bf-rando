﻿using System;
using OriModding.BF.Core;
using OriModding.BF.l10n;
using Randomiser.Utils;
using UnityEngine;

namespace Randomiser;

public class GenerateRandomiserSeedWizardController : MonoBehaviour
{
    public enum WizardState
    {
        Start,
        LogicMode,
        KeyMode,
        Goal,
        Seed,
        Difficulty,
        Finish
    }

    public static GenerateRandomiserSeedWizardController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        newBackAction = gameObject.AddComponent<DeprogressWizardAction>();
        newBackAction.Controller = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private ActionMethod closeMenuSequence;
    private DeprogressWizardAction newBackAction;

    public SeedGen.SeedGenOptions seedGenOptions;
    private DifficultyMode difficulty;
    private WizardState state;
    private CleverMenuItemSelectionManager selectionManager;
    private CleverMenuItemLayout layout;
    private CleverMenuItem[] options;
    private MessageBox[] messageBoxes;
    private BasicMessageProvider[] tooltips;

    /// <summary>When pressing Back how far back should we go. Usually 1 but quickstart skips some steps.</summary>
    private int backStepSize;

    private void SetOptionCount(int count)
    {
        selectionManager.MenuItems.Clear();
        layout.MenuItems.Clear();
        int i;
        for (i = 0; i < count; i++)
        {
            selectionManager.MenuItems.Add(options[i]);
            layout.MenuItems.Add(options[i]);

            options[i].gameObject.SetActive(true);
        }

        for (; i < options.Length; i++)
        {
            options[i].gameObject.SetActive(false);
        }

        layout.Sort();
    }

    private void SetOption(int index, string label, Action<GenerateRandomiserSeedWizardController> action = null)
    {
        (options[index].Pressed as ProgressWizardAction).OptionalAction = action;

        messageBoxes[index].SetMessage(new MessageDescriptor(Strings.Get(label)));
        tooltips[index].SetMessage(Strings.Get(label + "_DESC"));
    }

    private void SelectOption(int index)
    {
        selectionManager.SetCurrentItem(index);

        for (int i = 0; i < options.Length; i++)
        {
            if (i == index)
                options[index].HighlightAnimator.AnimatorDriver.GoToEnd();
            else
                options[index].HighlightAnimator.AnimatorDriver.GoToStart();
        }
    }

    public void BeginWizard(GameObject screen)
    {
        seedGenOptions = new SeedGen.SeedGenOptions();
        difficulty = DifficultyMode.Normal;

        selectionManager = screen.GetComponent<CleverMenuItemSelectionManager>();
        layout = screen.transform.Find("items").GetComponent<CleverMenuItemLayout>();

        closeMenuSequence = selectionManager.BackAction;
        selectionManager.BackAction = newBackAction;

        options = new CleverMenuItem[4];
        messageBoxes = new MessageBox[4];
        tooltips = new BasicMessageProvider[4];
        for (int i = 0; i < layout.transform.childCount; i++)
        {
            options[i] = layout.transform.GetChild(i).GetComponent<CleverMenuItem>();
            messageBoxes[i] = options[i].GetComponentInChildren<MessageBox>();

            var provider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            options[i].GetComponent<CleverMenuItemTooltip>().Tooltip = provider;
            tooltips[i] = provider;

            Destroy(options[i].Pressed);
            var action = options[i].gameObject.AddComponent<ProgressWizardAction>();
            action.Controller = this;
            options[i].Pressed = action;
        }

        GoToState(WizardState.Start);
    }

    private void GoToState(WizardState state)
    {
        this.state = state;

        if (state == WizardState.Finish)
        {
            SaveSlotsUI.Instance.SetDifficulty(difficulty); // would you believe this method also starts the game? well it does!
                                                            // It will run the seed generator because we added it to the start game sequence on the emptySlotPressed object
            return;
        }


        ModifyOptions(state);
    }

    private const int QuickStart = 0, NewSeed = 1, ImportSeed = 2, ReadSeed = 3;
    private const int Casual = 0, Standard = 1, Expert = 2, Master = 3;
    private const int Clues = 0, Shards = 1, NoKeyMode = 2;
    private const int ForceTrees = 0, WorldTour = 1, ForceMaps = 2, WarmthFrags = 3;
    private const int Easy = 0, Normal = 1, Hard = 2, OneLife = 3;
    private const int RandomSeed = 0, InputSeed = 1;

    private void ModifyOptions(WizardState state)
    {
        switch (state)
        {
            case WizardState.Start:
                SetOptionCount(4);
                SetOption(QuickStart, "UI_NEW_RANDO_QUICK_START");
                SetOption(NewSeed, "UI_NEW_RANDO_NEW_SEED");
                SetOption(ImportSeed, "UI_NEW_RANDO_IMPORT_SHARED_SEED");
                SetOption(ReadSeed, "UI_NEW_RANDO_LOAD_SEED_FROM_FILE");
                SelectOption(0);
                break;
            case WizardState.LogicMode:
                SetOptionCount(4);
                SetOption(Casual, "UI_NEW_RANDO_DIFFICULTY_Casual", w => w.seedGenOptions.LogicPreset = LogicPath.Casual);
                SetOption(Standard, "UI_NEW_RANDO_DIFFICULTY_Standard", w => w.seedGenOptions.LogicPreset = LogicPath.Standard);
                SetOption(Expert, "UI_NEW_RANDO_DIFFICULTY_Expert", w => w.seedGenOptions.LogicPreset = LogicPath.Expert);
                SetOption(Master, "UI_NEW_RANDO_DIFFICULTY_Master", w => w.seedGenOptions.LogicPreset = LogicPath.Master);
                SelectOption(1);
                break;
            case WizardState.KeyMode:
                SetOptionCount(3);
                SetOption(Clues, "UI_NEW_RANDO_KEYMODE_Clues", w => w.seedGenOptions.KeyMode = KeyMode.Clues);
                SetOption(Shards, "UI_NEW_RANDO_KEYMODE_Shards", w => w.seedGenOptions.KeyMode = KeyMode.Shards);
                SetOption(NoKeyMode, "UI_NEW_RANDO_KEYMODE_None", w => w.seedGenOptions.KeyMode = KeyMode.None);
                SelectOption(0);
                break;
            case WizardState.Goal:
                SetOptionCount(4);
                SetOption(ForceTrees, "UI_NEW_RANDO_GOAL_ForceTrees", w => w.seedGenOptions.GoalMode = GoalMode.ForceTrees);
                SetOption(WorldTour, "UI_NEW_RANDO_GOAL_WorldTour", w => w.seedGenOptions.GoalMode = GoalMode.WorldTour);
                SetOption(ForceMaps, "UI_NEW_RANDO_GOAL_ForceMaps", w => w.seedGenOptions.GoalMode = GoalMode.ForceMaps);
                SetOption(WarmthFrags, "UI_NEW_RANDO_GOAL_Frags", w => w.seedGenOptions.GoalMode = GoalMode.Frags);
                SelectOption(0);
                break;
            case WizardState.Seed:
                SetOptionCount(2);
                SetOption(RandomSeed, "UI_NEW_RANDO_RANDOM_SEED");
                SetOption(InputSeed, "UI_NEW_RANDO_ENTER_SEED");
                SelectOption(0);
                break;
            case WizardState.Difficulty:
                SetOptionCount(4);
                SetOption(Easy, "UI_NEW_RANDO_DIFFICULTY_Easy", w => w.difficulty = DifficultyMode.Easy);
                SetOption(Normal, "UI_NEW_RANDO_DIFFICULTY_Normal", w => w.difficulty = DifficultyMode.Normal);
                SetOption(Hard, "UI_NEW_RANDO_DIFFICULTY_Hard", w => w.difficulty = DifficultyMode.Hard);
                SetOption(OneLife, "UI_NEW_RANDO_DIFFICULTY_OneLife", w => w.difficulty = DifficultyMode.OneLife);
                SelectOption(1);
                break;
            default:
                break;
        }
    }

    public void OnSelectMenuItem(int index)
    {
        switch (state)
        {
            case WizardState.Start:
                if (index != ReadSeed)
                {
                    seedGenOptions.FilePath = null;
                }

                if (index == QuickStart)
                {
                    int rngSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                    uint seed = unchecked((uint)rngSeed); // positive numbers are nicer

                    seedGenOptions.Flags = RandomiserFlags.None;
                    seedGenOptions.KeyMode = KeyMode.Clues;
                    seedGenOptions.GoalMode = GoalMode.ForceTrees;
                    seedGenOptions.LogicPreset = LogicPath.Standard;
                    seedGenOptions.Seed = seed.ToString();

                    backStepSize = WizardState.Difficulty - WizardState.Start;
                    GoToState(WizardState.Difficulty);
                    return;
                }

                if (index == ImportSeed)
                {
                    Randomiser.Message("Not yet implemented");
                    return;
                }

                if (index == ReadSeed)
                {
                    string filename = WindowsUtils.OpenFile("Randomiser seed file", "Blind Forest Rando (*.obfr, *.dat)\0*.obfr;*.dat\0All Files (*.*)\0*.*\0");

                    if (filename != null)
                    {
                        seedGenOptions.FilePath = filename;
                        backStepSize = WizardState.Difficulty - WizardState.Start;
                        GoToState(WizardState.Difficulty);
                    }
                    return;
                }

                break;

            case WizardState.Goal:
                if (index == ForceMaps)
                {
                    Randomiser.Message("Not yet implemented");
                    return;
                }
                break;

            case WizardState.Seed:
                if (index == RandomSeed)
                {
                    int rngSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                    uint seed = unchecked((uint)rngSeed); // positive numbers are nicer
                    seedGenOptions.Seed = seed.ToString();
                }
                else if (index == InputSeed)
                {
                    Randomiser.Message("Not yet implemented");
                    return;
                }
                break;
        }

        backStepSize = 1;
        options[index].HighlightAnimator.AnimatorDriver.GoToStart();
        GoToState(state + 1);
    }

    public void GoBackwards()
    {
        if (state == WizardState.Start)
        {
            closeMenuSequence.Perform(null);
            return;
        }

        GoToState(state - backStepSize);
    }
}
