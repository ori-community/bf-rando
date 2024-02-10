using System;
using System.Collections.Generic;
namespace Randomiser.Multiplayer.OriRando;

public class UberId
{
    public UberId(int groupID, int id)
    {
        GroupID = groupID;
        ID = id;
    }
    public int GroupID;
    public int ID;
    public override string ToString() => $"({GroupID}, {ID})";
}


public abstract class UberState
{
    public UberId UberId;
    public abstract float AsFloat { get; set; }

}
public class IntUberState : UberState
{
    protected Action<int> setter;
    protected Func<int> getter;
    public IntUberState(UberId uberId, Action<int> setter, Func<int> getter) : base()
    {
        this.UberId = uberId;
        this.setter = setter;
        this.getter = getter;
    }

    public IntUberState(int groupId, int id, Action<int> setter, Func<int> getter) : base() => new IntUberState(new UberId(groupId, id), setter, getter);
    public override float AsFloat
    {
        get => this.getter();
        set => this.setter((int)Math.Round(value));
    }
}

public class BoolUberState : UberState
{
    protected Action<bool> setter;
    protected Func<bool> getter;
    public BoolUberState(UberId uberId, Action<bool> setter, Func<bool> getter) : base()
    {
        this.UberId = uberId;
        this.setter = setter;
        this.getter = getter;
    }

    public BoolUberState(int groupId, int id, Action<bool> setter, Func<bool> getter) : base() => new BoolUberState(new UberId(groupId, id), setter, getter);
    public override float AsFloat
    {
        get => this.getter() ? 1.0f : 0f;
        set => this.setter(value > 0f);
    }

}

public static class UberStates
{
    public static Dictionary<UberId, UberState> All = new();
    public static void AddLoc(Location loc)
    {
        void setter(bool newVal)
        {
            if (newVal)
                Randomiser.Grant(loc.guid);
            else
                RandomiserMod.Logger.LogError("Unpickuping is not supported (yet!)");
        }
        All.Add(loc.uberId, new BoolUberState(loc.uberId, setter, loc.HasBeenObtained));
    }

    public static UberState State(this UberId uberId) => All[uberId];
    public static float ValueAsFloat(this UberId uberId) => uberId.State().AsFloat;
}
