using System;
using System.Collections.Generic;
using Game;
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
    public abstract int AsInt { get; set; }

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
    public override int AsInt
    {
        get => this.getter();
        set => this.setter(value);
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
    public override int AsInt
    {
        get => this.getter() ? 1 : 0;
        set => this.setter(value > 0);
    }

}

public static class UberStates
{
    public static Dictionary<UberId, UberState> All = new();

    public static void Add(UberState state) {
        All.Add(state.UberId, state);
    }

    public static void init()
    {
        foreach(AbilityType ability in Enum.GetValues(typeof(AbilityType))) {
            Add(new BoolUberState(new UberId(13, (int)ability), 
                delegate (bool value) { Characters.Sein.PlayerAbilities.SetAbility(ability, value); },
                delegate () { return Characters.Sein.PlayerAbilities.HasAbility(ability); })
                );
        }
    }
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

    public static UberId UberId(this Network.UberStateUpdateMessage message) => new UberId(message.State.Group, message.State.State);
    public static UberState State(this Network.UberStateUpdateMessage message) => message.UberId().State();
    public static float ValueAsFloat(this UberId uberId) => uberId.State().AsInt;
}
