using System.Linq;
using System.Text;
using OriDeModLoader;
using Randomiser.Extensions;

namespace Randomiser
{
    public static class DynamicText
    {
        public static string BuildProgressString()
        {
            StringBuilder sb = new StringBuilder();

            int trees = Randomiser.TreesFoundExceptSein;
            int maps = Randomiser.MapstonesRepaired;

            if (Randomiser.Seed.GoalMode == GoalMode.WorldTour)
            {
                int relics = Randomiser.RelicsFound;
                sb.Append(Strings.Get("RANDO_PROGRESS_WORLDTOUR",
                    trees >= 10 ? "$" : "",
                    trees,
                    maps >= 9 ? "$" : "",
                    maps,
                    Randomiser.TotalPickupsFound,
                    relics >= Randomiser.Seed.RelicsRequired ? "$" : "",
                    relics,
                    Randomiser.Seed.RelicsRequired
                ));
            }
            else
            {
                sb.Append(Strings.Get("RANDO_PROGRESS_0",
                    trees == 10 ? "$" : "",
                    trees,
                    maps == 9 ? "$" : "",
                    maps,
                    Randomiser.TotalPickupsFound
                ));
            }

            if (Randomiser.Seed.KeyMode == KeyMode.Clues)
            {
                Clues.Clue wv = Randomiser.Seed.Clues.WaterVein;
                Clues.Clue gs = Randomiser.Seed.Clues.GumonSeal;
                Clues.Clue ss = Randomiser.Seed.Clues.Sunstone;

                sb.AppendLine();
                sb.Append(Strings.Get("RANDO_PROGRESS_CLUES",
                    wv.owned ? "*" : "",
                    wv.revealed ? Strings.Get("AREA_SHORT_" + wv.area) : "????",
                    gs.owned ? "#" : "",
                    gs.revealed ? Strings.Get("AREA_SHORT_" + gs.area) : "????",
                    ss.owned ? "@" : "",
                    ss.revealed ? Strings.Get("AREA_SHORT_" + ss.area) : "????"
                ));
            }

            if (Randomiser.Seed.KeyMode == KeyMode.Shards)
            {
                int max = Randomiser.Seed.ShardsRequiredForKey;

                sb.AppendLine();
                sb.Append(Strings.Get("RANDO_PROGRESS_SHARDS",
                    Randomiser.Inventory.waterVeinShards == max ? "*" : "",
                    Randomiser.Inventory.waterVeinShards,
                    Randomiser.Inventory.gumonSealShards == max ? "#" : "",
                    Randomiser.Inventory.gumonSealShards,
                    Randomiser.Inventory.sunstoneShards == max ? "@" : "",
                    Randomiser.Inventory.sunstoneShards,
                    max
                ));
            }

            return sb.ToString();
        }

        public static string BuildDetailedGoalString()
        {
            StringBuilder sb = new StringBuilder();

            if (Randomiser.Inventory.goalComplete)
            {
                sb.Append(Strings.Get("OBJECTIVE_COMPLETE_TEXT"));
            }
            else if (Randomiser.Seed.GoalMode == GoalMode.WorldTour)
            {
                sb.AppendLine(Strings.Get("OBJECTIVE_RELICS_FOUND_TEXT"));
                foreach (var relicLocation in Randomiser.Seed.RelicLocations.OrderBy(l => (int)l.worldArea))
                {
                    bool found = relicLocation.HasBeenObtained();
                    if (found) sb.Append("$");
                    sb.Append(Strings.Get("AREA_SHORT_" + relicLocation.worldArea.ToString()));
                    if (found) sb.Append("$");
                    sb.Append("  ");
                }
            }
            else if (Randomiser.Seed.GoalMode == GoalMode.ForceTrees)
            {
                sb.AppendLine(Strings.Get("OBJECTIVE_TREES_FOUND_TEXT"));
                foreach (var treeLocation in Randomiser.Locations.Cache.skillsExceptSein.OrderBy(l => l.saveIndex))
                {
                    bool found = treeLocation.HasBeenObtained();
                    if (found) sb.Append("$");
                    sb.Append(Strings.Get("LOCATION_" + treeLocation.name));
                    if (found) sb.Append("$");
                    sb.Append("  ");
                }
            }
            else if (Randomiser.Seed.GoalMode == GoalMode.Frags)
            {
                sb.Append(Strings.Get("OBJECTIVE_WARMTH_FRAGS_FOUND_TEXT", Randomiser.Inventory.warmthFragments, Randomiser.Seed.WarmthFragmentsRequired));
            }

            return sb.ToString();
        }

        public static string BuildSkillClueString()
        {
            StringBuilder sb = new StringBuilder();

            Location stompLoc = Randomiser.Seed.GetSkillLocation(AbilityType.Stomp);
            Location grenadeLoc = Randomiser.Seed.GetSkillLocation(AbilityType.Grenade);

            if (stompLoc != null)
            {
                bool found = stompLoc.HasBeenObtained();
                if (found) sb.Append("$");
                sb.Append(Strings.Get("SKILL_Stomp"));
                sb.Append(": ");
                sb.Append(Strings.Get("AREA_SHORT_" + stompLoc.worldArea));
                if (found) sb.Append("$");

                if (grenadeLoc != null)
                    sb.Append("  ");
            }

            if (grenadeLoc != null)
            {
                bool found = grenadeLoc.HasBeenObtained();
                if (found) sb.Append("$");
                sb.Append(Strings.Get("SKILL_Grenade"));
                sb.Append(": ");
                sb.Append(Strings.Get("AREA_SHORT_" + grenadeLoc.worldArea));
                if (found) sb.Append("$");
            }

            return sb.ToString();
        }
    }
}
