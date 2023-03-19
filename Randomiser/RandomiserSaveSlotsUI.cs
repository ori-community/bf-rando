using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Randomiser
{
    public class RandomiserSaveSlotsUI : MonoBehaviour, ISuspendable
    {
        public bool IsSuspended { get; set; }

        private SaveSlotsUI saveSlotsUI;

        void Awake()
        {
            SuspensionManager.Register(this);
            saveSlotsUI = GetComponent<SaveSlotsUI>();
        }

        void FixedUpdate()
        {
            if (IsSuspended || !GameController.IsFocused || !saveSlotsUI.IsVisible || saveSlotsUI.PromptIsOpen || !saveSlotsUI.Active || saveSlotsUI.SelectingDifficulty || saveSlotsUI.IsCopying)
                return;

            if (Core.Input.Select.OnPressed && !Core.Input.Select.Used && saveSlotsUI.CurrentSaveSlot && saveSlotsUI.CurrentSaveSlot.HasSave)
            {
                Core.Input.Select.Used = true;

                var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                int saveSlotIndex = saveSlotsUI.CurrentSlotIndex;
                var outputPath = Path.GetFullPath(Path.Combine(assemblyDir, Path.Combine("seeds", (saveSlotIndex + 1).ToString())));

                if (Directory.Exists(outputPath))
                    Process.Start(outputPath);
                else
                    Randomiser.Message("Seed directory not found");
            }
        }
    }
}
