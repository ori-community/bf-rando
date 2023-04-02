using System.ComponentModel;

namespace Randomiser
{
    public enum RandomiserActionKind
    {
        [Description("MU")] MultiPickup,
        [Description("SK")] Skill,
        [Description("EC")] EnergyCell,
        [Description("EX")] Experience,
        [Description("HC")] HealthCell,
        [Description("AC")] AbilityCell,
        [Description("KS")] Keystone,
        [Description("MS")] Mapstone,
        [Description("EV")] WorldEvent,
        [Description("TP")] Teleporter,
        [Description("RB")] Bonus,
        [Description("WT")] WorldTourRelic,
        [Description("SC")] SkillClue
    }
}
