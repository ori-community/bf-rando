using System.Collections.Generic;
using System.Linq;
using Game;
using OriDeModLoader.CustomSeinAbilities;
using UnityEngine;

namespace Randomiser
{
    public class SeinHotColdSense : CustomSeinAbility
    {
        public override bool AllowAbility(SeinLogicCycle logicCycle) => logicCycle.Sein.PlayerAbilities.Sense.HasAbility;

        private static readonly Color coldColour = new Color(0f, 0.5f, 0.5f, 0.5f);
        private static readonly Color warmColour = new Color(0.5f, 0.1666667f, 0f, 0.5f);
        private const float MaxDistance = 64f;

        public override void UpdateCharacterState()
        {
            // TODO optimise? use inventory if caching
            IEnumerable<Vector2> positions = Randomiser.Seed.SenseItems.Where(l => !l.HasBeenObtained()).Select(p => p.position);
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

            float realDist = (seinPosition - closestPos).magnitude;

            Characters.Sein.PlatformBehaviour.Visuals.SpriteRenderer.material.color = Color.Lerp(warmColour, coldColour, realDist / MaxDistance);
        }
    }
}
