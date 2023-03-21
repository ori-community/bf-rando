using System;
using BaseModLib;
using HarmonyLib;
using OriDeModLoader;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch]
    class ShowRandomiserWizard
    {
        [HarmonyPostfix, HarmonyPatch(typeof(SaveSlotsUI), nameof(SaveSlotsUI.EmptySaveSlotSelected))]
        static void ShowRandomiserWizardOnPress(GameObject ___m_difficultyScreen)
        {
            GenerateRandomiserSeedWizardController.Instance.BeginWizard(___m_difficultyScreen);
        }
    }

    public class ProgressWizardAction : ActionMethod
    {
        public GenerateRandomiserSeedWizardController Controller;
        public Action<GenerateRandomiserSeedWizardController> OptionalAction;
        public override void Perform(IContext context)
        {
            OptionalAction?.Invoke(Controller);

            Controller.OnSelectMenuItem(transform.GetSiblingIndex());
        }
    }

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
        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public SeedGen.SeedGenOptions seedGenOptions;
        private DifficultyMode difficulty;
        private WizardState state;


        CleverMenuItemSelectionManager selectionManager;
        CleverMenuItemLayout layout;

        CleverMenuItem[] options;
        MessageBox[] messageBoxes;
        BasicMessageProvider[] tooltips;

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

        public void BeginWizard(GameObject screen)
        {
            seedGenOptions = new SeedGen.SeedGenOptions();
            difficulty = DifficultyMode.Normal;

            selectionManager = screen.GetComponent<CleverMenuItemSelectionManager>();
            layout = screen.transform.Find("items").GetComponent<CleverMenuItemLayout>();

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
            selectionManager.SetIndexToFirst();
        }

        const int QuickStart = 0, NewSeed = 1, ImportSeed = 2;
        const int Casual = 0, Standard = 1, Expert = 2, Master = 3;
        const int Clues = 0, Shards = 1, NoKeyMode = 2;
        const int ForceTrees = 0, ForceMaps = 1, WarmthFrags = 2;
        const int Easy = 0, Normal = 1, Hard = 2, OneLife = 3;
        const int RandomSeed = 0, InputSeed = 1;

        private void ModifyOptions(WizardState state)
        {
            switch (state)
            {
                case WizardState.Start:
                    SetOptionCount(3);
                    SetOption(QuickStart, "UI_NEW_RANDO_QUICK_START");
                    SetOption(NewSeed, "UI_NEW_RANDO_NEW_SEED");
                    SetOption(ImportSeed, "UI_NEW_RANDO_IMPORT_SHARED_SEED");
                    break;
                case WizardState.LogicMode:
                    SetOptionCount(4);
                    SetOption(Casual, "UI_NEW_RANDO_DIFFICULTY_Casual", w => w.seedGenOptions.LogicPreset = LogicPath.Casual);
                    SetOption(Standard, "UI_NEW_RANDO_DIFFICULTY_Standard", w => w.seedGenOptions.LogicPreset = LogicPath.Standard);
                    SetOption(Expert, "UI_NEW_RANDO_DIFFICULTY_Expert", w => w.seedGenOptions.LogicPreset = LogicPath.Expert);
                    SetOption(Master, "UI_NEW_RANDO_DIFFICULTY_Master", w => w.seedGenOptions.LogicPreset = LogicPath.Master);
                    break;
                case WizardState.KeyMode:
                    SetOptionCount(3);
                    SetOption(Clues, "UI_NEW_RANDO_KEYMODE_Clues", w => w.seedGenOptions.KeyMode = KeyMode.Clues);
                    SetOption(Shards, "UI_NEW_RANDO_KEYMODE_Shards", w => w.seedGenOptions.KeyMode = KeyMode.Shards);
                    SetOption(NoKeyMode, "UI_NEW_RANDO_KEYMODE_None", w => w.seedGenOptions.KeyMode = KeyMode.None);
                    break;
                case WizardState.Goal:
                    SetOptionCount(3);
                    SetOption(ForceTrees, "UI_NEW_RANDO_GOAL_ForceTrees", w => w.seedGenOptions.GoalMode = GoalMode.ForceTrees);
                    SetOption(ForceMaps, "UI_NEW_RANDO_GOAL_ForceMaps", w => w.seedGenOptions.GoalMode = GoalMode.ForceMaps);
                    SetOption(WarmthFrags, "UI_NEW_RANDO_GOAL_WarmthFrags", w => w.seedGenOptions.GoalMode = GoalMode.WarmthFrags);
                    break;
                case WizardState.Seed:
                    SetOptionCount(2);
                    SetOption(RandomSeed, "UI_NEW_RANDO_RANDOM_SEED");
                    SetOption(InputSeed, "UI_NEW_RANDO_ENTER_SEED");
                    break;
                case WizardState.Difficulty:
                    SetOptionCount(4);
                    SetOption(Easy, "UI_NEW_RANDO_DIFFICULTY_Easy", w => w.difficulty = DifficultyMode.Easy);
                    SetOption(Normal, "UI_NEW_RANDO_DIFFICULTY_Normal", w => w.difficulty = DifficultyMode.Normal);
                    SetOption(Hard, "UI_NEW_RANDO_DIFFICULTY_Hard", w => w.difficulty = DifficultyMode.Hard);
                    SetOption(OneLife, "UI_NEW_RANDO_DIFFICULTY_OneLife", w => w.difficulty = DifficultyMode.OneLife);
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
                    if (index == QuickStart)
                    {
                        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

                        seedGenOptions.Flags = RandomiserFlags.None;
                        seedGenOptions.KeyMode = KeyMode.Clues;
                        seedGenOptions.GoalMode = GoalMode.ForceTrees;
                        seedGenOptions.LogicPreset = LogicPath.Standard;
                        seedGenOptions.Seed = seed.ToString();

                        GoToState(WizardState.Difficulty);
                        return;
                    }

                    break;

                case WizardState.Seed:
                    if (index == RandomSeed)
                    {
                        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                        seedGenOptions.Seed = seed.ToString();
                    }
                    break;
            }

            GoToState(state + 1);
        }
    }
}
