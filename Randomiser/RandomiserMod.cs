using BaseModLib;
using HarmonyLib;
using OriDeModLoader;
using OriDeModLoader.CustomSeinAbilities;

namespace Randomiser
{
    public class RandomiserMod : IMod
    {
        public string Name => "Randomiser";

        private Harmony harmony;

        public void Init()
        {
            harmony = new Harmony("com.ori.randomiser");
            harmony.PatchAll();

            Controllers.Add<RandomiserInventory>("b9d5727e-43ff-4a6c-a9d1-d51489b3733d", "Randomiser", mb => Randomiser.Inventory = mb as RandomiserInventory);
            Controllers.Add<RandomiserSeed>("df0ebc08-9469-4f58-9e10-f836115b797b", "Randomiser", mb => Randomiser.Seed = mb as RandomiserSeed);
            Controllers.Add<RandomiserLocations>("53d559f6-f989-48ae-9a6a-1c1b72b7955c", "Randomiser", mb => Randomiser.Locations = mb as RandomiserLocations);
            Controllers.Add<RandomiserController>("f563a07e-7ac3-4fd5-b134-afe69c1f3c4e", "Randomiser");
            Controllers.Add<RandomiserMessageController>("c2d7a17a-1b2a-42cb-a9c3-d6af4704ccf1", "Randomiser", mb => Randomiser.Messages = mb as RandomiserMessageController);

            CustomSeinAbilityManager.Add<SeinRegen>("cc6427ed-31bf-4a7a-b4c4-ad559000ced9");

            Hooks.OnStartNewGame += () =>
            {
                Randomiser.Inventory.Reset();
                Randomiser.Seed.LoadSeed("randomizer.dat");
            };

            SceneBootstrap.RegisterHandler(RandomiserBootstrap.SetupBootstrap, "Randomiser");
        }

        public void Unload()
        {

        }
    }
}
