
using System.Collections.Generic;
using System.Linq;

namespace RevitLevelsPlans
{
    public class LevelPlanRow
    {
        public string LevelName { get; set; }
        public double ElevationFeet { get; set; }
        public List<string> FloorPlanNames { get; set; } = new List<string>();

        public string PlansJoined => (FloorPlanNames != null && FloorPlanNames.Any())
            ? string.Join(", ", FloorPlanNames)
            : "(none)";
    }
}
