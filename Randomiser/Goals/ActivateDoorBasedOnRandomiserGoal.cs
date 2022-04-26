using BaseModLib;
using OriDeModLoader;
using UnityEngine;

namespace Randomiser
{
    // Doesn't actually disable the door but does add a message telling you that you can't enter
    // See ForbidDoorPatch
    public class ActivateDoorBasedOnRandomiserGoal : MonoBehaviour
    {
        public Door door;

        public BasicMessageProvider messageProvider;

        private void Awake()
        {
            messageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            messageProvider.SetMessage(Strings.Get("RANDO_EXIT_BLOCKED"));
        }

        private void FixedUpdate()
        {
            if (Randomiser.Seed.GoalMode != GoalMode.None && !Randomiser.Inventory.goalComplete)
                door.OverrideEnterDoorMessage = messageProvider;
            else
                door.OverrideEnterDoorMessage = null;
        }
    }
}
