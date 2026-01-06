
using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment6.Models
{
    public class LevelPlanItem
    {
        public string LevelName { get; set; }
        public string PlanName { get; set; }
        public ElementId ViewId { get; set; }
    }
}
