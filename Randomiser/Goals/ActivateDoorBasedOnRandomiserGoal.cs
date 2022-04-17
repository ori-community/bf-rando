using BaseModLib;
using UnityEngine;

namespace Randomiser
{
    // Doesn't actually disable the door but does add a message telling you that you can't enter
    // See ForbidDoorPatch
    public class ActivateDoorBasedOnRandomiserGoal : MonoBehaviour
    {
        public Door door;

        public BasicMessageProvider messageProvider;

        void Awake()
        {
            messageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            messageProvider.SetMessage("You're not allowed in!");
        }

        void FixedUpdate()
        {
            if (!Randomiser.Inventory.goalComplete)
                door.OverrideEnterDoorMessage = messageProvider;
            else
                door.OverrideEnterDoorMessage = null;
        }
    }
}
