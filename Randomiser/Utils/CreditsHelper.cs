using System.IO;
using OriModding.BF.Core;
using UnityEngine;

namespace Randomiser.Utils;

#if DEBUG
public class CreditsHelper : MonoBehaviour
{
    private MessageBox messageBox;
    private FileSystemWatcher watcher;

    private string CreditsPath => RandomiserMod.Instance.GetAssetPath("assets", "credits.txt");

    private void Awake()
    {
        messageBox = GetComponent<MessageBox>();

        watcher = new FileSystemWatcher(Path.GetDirectoryName(CreditsPath));
        watcher.Filter = "credits.txt";
        watcher.EnableRaisingEvents = true;
        watcher.Changed += (sender, e) => RefreshText();

        RefreshText();
    }

    private void RefreshText()
    {
        messageBox.OverrideText = File.ReadAllText(CreditsPath);
    }

    private void OnDestroy()
    {
        watcher.Dispose();
    }
}
#endif
