using System.ComponentModel;

namespace Randomiser
{
    public enum RandomiserActionKind
    {
        [Description("MultiPickup")] MU,
        [Description("Skill")] SK,
        [Description("EnergyCell")] EC,
        [Description("Experience")] EX,
        [Description("HealthCell")] HC,
        [Description("AbilityCell")] AC,
        [Description("Keystone")] KS,
        [Description("Mapstone")] MS,
        [Description("WorldEvent")] EV,
        [Description("Teleporter")] TP,
        [Description("Bonus")] RB,
        [Description("WorldTourRelic")] WT,
        [Description("SkillClue")] SC
    }
}
