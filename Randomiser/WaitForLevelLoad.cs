using Core;
using UnityEngine;

namespace Randomiser
{
    public class WaitForLevelLoad : CustomYieldInstruction
    {
        private readonly string sceneName;

        public WaitForLevelLoad(string sceneName)
        {
            this.sceneName = sceneName;
        }

        public override bool keepWaiting => Scenes.Manager.CurrentScene?.Scene != sceneName;
    }
}
