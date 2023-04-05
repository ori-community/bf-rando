namespace Randomiser.Stats
{
    public class StatsController : SaveSerialize
    {
        public AllStats Global;

        public void Reset()
        {
            Global = new AllStats();
            Global.Init();
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
            Global.Serialize(ar);
        }

        // TODO when copying over another file that has the same identifier, keep the stats of the more played one
        //      (this will keep stats consistent when using backups)

        public void UpdateZone(Location.WorldArea area, float time)
        {
            Global.areaStats[(int)area].time += time;
        }
    }
}
