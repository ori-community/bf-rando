namespace Randomiser;

public class RandomiserInventory : SaveSerialize
{
    public bool goalComplete;
    public bool finishedGinsoEscape;

    public BitCollection pickupsCollected = new BitCollection(320);

    public int attackUpgrades;
    public int spiritLightEfficiency;
    public int extraDashes;
    public int extraJumps;
    public bool chargeDashEfficiency;
    public int healthRegen;
    public int energyRegen;

    public int waterVeinShards;
    public int gumonSealShards;
    public int sunstoneShards;

    public int warmthFragments;

    public bool skillClueFound;

    public string lastPickup;

    public int keysSpent;
    public int apSpent;
    public int mapsSpent;
    public int xpGainedFromEnemies;

    public override void Awake()
    {
        base.Awake();
        Reset();
    }

    public void Reset()
    {
        // TODO make this automatic somehow
        finishedGinsoEscape = false;
        goalComplete = false;
        pickupsCollected.Clear();
        attackUpgrades = 0;
        spiritLightEfficiency = 0;
        extraDashes = 0;
        extraJumps = 0;
        chargeDashEfficiency = false;
        healthRegen = 0;
        energyRegen = 0;
        waterVeinShards = 0;
        gumonSealShards = 0;
        sunstoneShards = 0;
        lastPickup = "Good luck on your rando!";
        skillClueFound = false;
        warmthFragments = 0;
        keysSpent = 0;
        mapsSpent = 0;
        apSpent = 0;
        xpGainedFromEnemies = 0;
    }

    public override void Serialize(Archive ar)
    {
        int version = ar.Serialize(0); // Can use version to determine if the file is unplayable due to changing save data, or try to repair it

        // The order of fields in the class don't matter, but the order they are serialized does.
        // Don't change it.
        ar.Serialize(ref goalComplete);
        ar.Serialize(ref finishedGinsoEscape);
        pickupsCollected.Serialize(ar);

        ar.Serialize(ref attackUpgrades);
        ar.Serialize(ref spiritLightEfficiency);
        ar.Serialize(ref extraDashes);
        ar.Serialize(ref extraJumps);
        ar.Serialize(ref chargeDashEfficiency);
        ar.Serialize(ref healthRegen);
        ar.Serialize(ref energyRegen);

        ar.Serialize(ref waterVeinShards);
        ar.Serialize(ref gumonSealShards);
        ar.Serialize(ref sunstoneShards);

        ar.Serialize(ref lastPickup);
        ar.Serialize(ref skillClueFound);
        ar.Serialize(ref warmthFragments);

        ar.Serialize(ref keysSpent);
        ar.Serialize(ref mapsSpent);
        ar.Serialize(ref apSpent);
        ar.Serialize(ref xpGainedFromEnemies);
    }
}
