using System.IO;
using OriModding.BF.Core;
using UnityEngine;

namespace Randomiser.Utils
{
#if DEBUG
    public class CreditsHelper : MonoBehaviour
    {
        MessageBox messageBox;

        FileSystemWatcher watcher;

        string CreditsPath => RandomiserMod.Instance.GetAssetPath("assets", "credits.txt");

        void Awake()
        {
            messageBox = GetComponent<MessageBox>();

            watcher = new FileSystemWatcher(Path.GetDirectoryName(CreditsPath));
            watcher.Filter = "credits.txt";
            watcher.EnableRaisingEvents = true;
            watcher.Changed += (sender, e) => RefreshText();

            RefreshText();
        }

        void RefreshText()
        {
            messageBox.OverrideText = File.ReadAllText(CreditsPath);
        }

        void OnDestroy()
        {
            watcher.Dispose();
        }
    }
#endif
}
