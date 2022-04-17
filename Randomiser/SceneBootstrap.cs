using BaseModLib;
using Game;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Randomiser
{
    public class SceneBootstrap : MonoBehaviour
    {
        public static void RegisterHandler(Action<SceneBootstrap> callback, string group = "Bootstrap")
        {
            Controllers.Add<SceneBootstrap>(group: group, callback: mb => callback(mb as SceneBootstrap));
        }


        void Awake()
        {
            Events.Scheduler.OnSceneRootPreEnabled.Add(OnSceneRootPreEnabled);
        }

        void OnSceneRootPreEnabled(SceneRoot sceneRoot)
        {
            if (BootstrapActions.ContainsKey(sceneRoot.name))
                BootstrapActions[sceneRoot.name].Invoke(sceneRoot);
        }

        public Dictionary<string, Action<SceneRoot>> BootstrapActions = new Dictionary<string, Action<SceneRoot>>();
    }
}
