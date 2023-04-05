namespace Randomiser
{
    public class StatsController : SaveSerialize
    {
        public struct AllStats
        {
            public int soulLinks;
            public int deaths;
        }

        public AllStats current;

        public void Reset()
        {
            current = new AllStats();
        }

        public override void Awake()
        {
            Reset();
            Randomiser.Stats = this;
        }

        public override void OnDestroy()
        {
            if (Randomiser.Stats == this)
                Randomiser.Stats = null;
        }

        public void SaveNow()
        {
            // Updates the SaveGameData for just this object
            SaveSceneManager.Master.Save(Game.Checkpoint.SaveGameData.Master, this);
            // Writes all SaveGameData to file
            GameController.Instance.SaveGameController.PerformSave();
        }

        public override void Serialize(Archive ar)
        {
            ar.Serialize(ref current.soulLinks);
            ar.Serialize(ref current.deaths);
        }

        // TODO when copying over another file that has the same identifier, keep the stats of the more played one
        //      (this will keep stats consistent when using backups)
    }
}
