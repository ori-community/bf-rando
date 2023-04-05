namespace Randomiser.Stats
{
    public struct AreaStats : ISerializable
    {
        public int deaths;
        public float time;

        public void Serialize(Archive ar)
        {
            ar.Serialize(ref deaths);
            ar.Serialize(ref time);
        }
    }
}
