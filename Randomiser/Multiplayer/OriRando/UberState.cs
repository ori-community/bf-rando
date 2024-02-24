using System;
using System.Collections.Generic;
using Game;
using Randomiser.Extensions;
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
    public abstract int Get();

    protected abstract void set(int value);
    public virtual void Set(int value, bool ignoreChange = false) {
        set(value);
        if(!ignoreChange)
            OnChange();

    }
    // probably we'll only ever use this for debugging?
    public virtual void OnChange() {
        RandomiserMod.Logger.LogInfo($"{UberId} value is now {Get()}");
    }
    public virtual int AsInt
    {
        get { return Get(); }
        set { Set(value); }
    }

    public override string ToString() => $"{UberId}: {Get()}";

}
public class IntUberState : UberState
{
    protected Action<int> setter;
    protected Func<int> getter;
    protected Action onChange = delegate () { };
    public IntUberState(UberId uberId, Action<int> setter, Func<int> getter, Action onChange = null) : base()
    {
        this.UberId = uberId;
        this.setter = setter;
        this.getter = getter;
        this.onChange ??= onChange;
    }

    public IntUberState(int groupId, int id, Action<int> setter, Func<int> getter, Action onChange = null) : base() => new IntUberState(new UberId(groupId, id), setter, getter, onChange);
    public override int Get() => getter();
    protected override void set(int value) => setter(value);
    public override void OnChange()
    {
        onChange();
        base.OnChange();
    }
}

public class BoolUberState : UberState
{
    protected Action<bool> setter;
    protected Func<bool> getter;
    protected Action onChange;
    public BoolUberState(UberId uberId, Action<bool> setter, Func<bool> getter, Action onChange = null) : base()
    {
        UberId = uberId;
        this.setter = setter;
        this.getter = getter;
        this.onChange ??= onChange;
    }
    public BoolUberState(int groupId, int id, Action<bool> setter, Func<bool> getter, Action onChange = null) : base() => new BoolUberState(new UberId(groupId, id), setter, getter, onChange);

    public override int Get() => getter() ? 1 : 0;
    protected override void set(int value) => setter(value != 0);
    public override void OnChange()
    {
        onChange();
        base.OnChange();
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

    // dumb extensions 
    public static UberState State(this UberId uberId) { 
        if(!All.ContainsKey(uberId)) {
            RandomiserMod.Logger.LogError($"Unknown UberId {uberId}!");
        }
        return All[uberId];
    }
    public static UberId UberId(this Network.UberStateUpdateMessage message) => new UberId(message.State.Group, message.State.State);
    public static UberState State(this Network.UberStateUpdateMessage message) => message.UberId().State();
    public static float ValueAsFloat(this UberId uberId) => uberId.State().AsInt;
}
