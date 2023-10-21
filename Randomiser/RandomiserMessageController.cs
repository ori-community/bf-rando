using System.Collections.Generic;
using Game;
using OriModding.BF.Core;
using UnityEngine;

namespace Randomiser;

public class RandomiserMessageController : MonoBehaviour
{
    public struct RandomiserMessage
    {
        public readonly string message;
        public readonly float duration;

        public RandomiserMessage(string message, float duration)
        {
            this.message = message;
            this.duration = duration;
        }
    }

    private readonly Queue<RandomiserMessage> messageQueue = new Queue<RandomiserMessage>();

    private BasicMessageProvider messageProvider;
    private float remainingDuration;

    public void AddMessage(string message, float duration = 3f)
    {
        messageQueue.Enqueue(new RandomiserMessage(message, duration));
        if (remainingDuration <= 0)
            ShowNext();
    }

    private void Update()
    {
        if (remainingDuration > 0)
        {
            remainingDuration -= Time.deltaTime;
            if (remainingDuration <= 0)
                ShowNext();
        }
    }

    private void ShowNext()
    {
        if (messageQueue.Count == 0)
            return;

        if (messageProvider == null)
            messageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();

        var nextMessage = messageQueue.Dequeue();

        messageProvider.SetMessage(nextMessage.message);
        UI.Hints.Show(messageProvider, HintLayer.Gameplay, nextMessage.duration);
        remainingDuration = nextMessage.duration;
    }
}
