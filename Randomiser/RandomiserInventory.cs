namespace Randomiser
{
    public class RandomiserInventory : SaveSerialize
    {
        public bool goalComplete;
        public bool finishedEscape;

        public BitCollection pickupsCollected = new BitCollection(256);

        public void Reset()
        {
            // TODO make this automatic somehow
            finishedEscape = false;
            goalComplete = false;
            pickupsCollected.Clear();
        }

        public override void Serialize(Archive ar)
        {
            ar.Serialize(ref goalComplete);
            ar.Serialize(ref finishedEscape);
            pickupsCollected.Serialize(ar);
        }
    }
}
