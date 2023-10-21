using OriModding.BF.Core.SeinAbilities;
using UnityEngine;

namespace Randomiser.Stats;

public class StatsController : CustomSeinAbility
{
    public AllStats GlobalStats;

    public bool IsSuspended { get; set; }

    public void Reset()
    {
        GlobalStats = new AllStats();
        GlobalStats.Init();
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
        GlobalStats.Serialize(ar);
    }

    // TODO when copying over another file that has the same identifier, keep the stats of the more played one
    //      (this will keep stats consistent when using backups)

    public override void UpdateCharacterState()
    {
        // TODO check that this continues to count up during SA (and fix the base timer so it does too!)
        GlobalStats.areaStats[(int)Sein.CurrentWorldArea()].time += Time.deltaTime;
    }

    public override bool AllowAbility(SeinLogicCycle logicCycle) => true;
}
