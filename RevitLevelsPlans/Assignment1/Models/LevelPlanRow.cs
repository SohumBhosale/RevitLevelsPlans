
using System.Collections.Generic;
using System.Linq;

namespace RevitLevelsPlans
{
    public class LevelPlanRow
    {
        public string LevelName { get; set; }
        public double ElevationFeet { get; set; }              // Revit internal units (feet)
        public List<string> FloorPlanNames { get; set; } = new List<string>();

        // For binding in the grid (computed via LINQ)
        public string PlansJoined => (FloorPlanNames != null && FloorPlanNames.Any())
            ? string.Join(", ", FloorPlanNames)
            : "(none)";
    }
}
