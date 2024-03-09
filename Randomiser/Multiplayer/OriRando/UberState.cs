using System;
using System.Collections.Generic;
using Game;
using Randomiser.Extensions;
using UnityEngine;
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
    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object obj) => (obj is UberId) && (obj as UberId).GroupID == GroupID && (obj as UberId).ID == ID;
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
        RandomiserMod.Logger.LogDebug($"{UberId} value is now {Get()}");
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
    protected Action onChange;
    public IntUberState(UberId uberId, Action<int> setter, Func<int> getter, Action onChange = null) : base()
    {
        this.UberId = uberId;
        this.setter = setter;
        this.getter = getter;
        this.onChange = onChange ?? delegate () { };
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
        this.onChange = onChange ?? delegate () { };
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
            Add(new BoolUberState(ability.UberId(), 
                delegate (bool value) { Characters.Sein.PlayerAbilities.SetAbility(ability, value); },
                delegate () { return Characters.Sein.PlayerAbilities.HasAbility(ability); })
                );
        }
    }
    public static void AddLoc(Location loc)
    {
        var multiStateId = new UberId(12, (loc.uberId.GroupID + 1) * 100 + loc.uberId.ID);
        var grantLoc = delegate (bool value) { if (value) Randomiser.Grant(loc.guid); };

        void onChange() {
            if (loc.HasBeenObtained()) {
                WebsocketClient.SendQueue.TryAdd(new Network.Packet
                {
                    Id = Network.Packet.PacketID.UberStateUpdateMessage,
                    packet = new Network.UberStateUpdateMessage {State = new Network.UberId() { Group = multiStateId.GroupID, State = multiStateId.ID }, Value = 1}.ToByteArray()
                });
            }
            else
                RandomiserMod.Logger.LogError("Unpickuping is not supported (yet!)");
        }
        Add(new BoolUberState(loc.uberId, grantLoc, loc.HasBeenObtained, onChange));
        // crime zone
        Add(new BoolUberState(multiStateId, delegate (bool value) {
            if (value && !loc.HasBeenObtained()) {
                try {
                    RandomiserMod.Logger.LogInfo($"got {Randomiser.Seed.GetActionFromGuid(loc.guid).ToSeedFormat()} from {loc.name}");
                } catch(Exception e) {
                    RandomiserMod.Logger.LogError($"{multiStateId} hasBeenObtained (for {loc.name}): {e}");
                }
                Randomiser.Grant(loc.guid, true);
            }
        }, loc.HasBeenObtained));
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
    public static Network.UberStateUpdateMessage ToUpdateMessage(this UberState state) => new Network.UberStateUpdateMessage
    {
        State = new Network.UberId()
        {
            Group = state.UberId.GroupID,
            State = state.UberId.ID
        },
        Value = state.Get()
    };
    public static UberState State(this AbilityType abilityType) => abilityType.UberId().State();
    public static UberId UberId(this AbilityType abilityType) => new UberId(13, (int)abilityType);
    public static float ValueAsFloat(this UberId uberId) => uberId.State().Get();
}

public class UberStateController : MonoBehaviour {
    public void Awake()
    {
        UberStates.init();
    }
}

