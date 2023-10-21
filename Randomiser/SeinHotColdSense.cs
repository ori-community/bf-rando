using System.Collections.Generic;
using System.Linq;
using Game;
using OriModding.BF.Core.SeinAbilities;
using UnityEngine;

namespace Randomiser;

public class SeinHotColdSense : CustomSeinAbility
{
    public override bool AllowAbility(SeinLogicCycle logicCycle) => logicCycle.Sein.PlayerAbilities.Sense.HasAbility;

    private static readonly Color coldColour = new Color(0f, 0.5f, 0.5f, 0.5f);
    private static readonly Color warmColour = new Color(0.5f, 0.1666667f, 0f, 0.5f);
    private const float MaxDistance = 64f;

    public override void UpdateCharacterState()
    {
        // TODO optimise? use inventory if caching
        var realDist = Characters.Ori && Characters.Ori.InsideMapstone ? GetMapstoneItem() : GetNormalItem();

        Characters.Sein.PlatformBehaviour.Visuals.SpriteRenderer.material.color = Color.Lerp(warmColour, coldColour, realDist / MaxDistance);
    }

    private static float GetNormalItem()
    {
        IEnumerable<Vector2> positions = Randomiser.Seed.SenseItems.Where(l => l.type != Location.LocationType.ProgressiveMapstone && !l.HasBeenObtained()).Select(p => p.position);
        Vector2 seinPosition = Characters.Sein.Position;

        float closestDistance = float.PositiveInfinity;
        Vector2 closestPos = Vector2.zero;
        foreach (var pos in positions)
        {
            float dist = (pos - seinPosition).sqrMagnitude;
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestPos = pos;
            }
        }

        if (closestDistance == float.PositiveInfinity)
            return MaxDistance;

        return (seinPosition - closestPos).magnitude;
    }

    private static float GetMapstoneItem()
    {
        // Location data has progressive maps at (0, 24 + i * 4) so the y position can be used to judge how deep into the progression it is
        IEnumerable<float> positions = Randomiser.Seed.SenseItems.Where(l => l.type == Location.LocationType.ProgressiveMapstone && !l.HasBeenObtained()).Select(p => p.position.y);

        float closestDistance = float.PositiveInfinity;
        foreach (var pos in positions)
        {
            if (pos < closestDistance)
                closestDistance = pos;
        }

        if (closestDistance == float.PositiveInfinity)
            return MaxDistance;

        return closestDistance - (24 + Randomiser.MapstonesRepaired * 4);
    }
}
