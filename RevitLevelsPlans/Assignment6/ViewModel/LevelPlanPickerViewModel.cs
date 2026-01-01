
using System.Collections.ObjectModel;
using System.Linq;

using Autodesk.Revit.DB;

using RevitLevelsPlans.Assignment6.Models;

namespace RevitLevelsPlans.Assignment6.ViewModel
{
    public class LevelPlanPickerViewModel
    {
        public ObservableCollection<LevelPlanItem> Items { get; } = new ObservableCollection<LevelPlanItem>();
        public LevelPlanItem Selected { get; set; }

        public LevelPlanPickerViewModel(System.Collections.Generic.IEnumerable<ViewPlan> plans)
        {
            var ordered = plans
                .Select(v => new LevelPlanItem
                {
                    LevelName = v.GenLevel?.Name ?? "(no level)",
                    PlanName = v.Name,
                    ViewId = v.Id
                })
                .OrderBy(x => x.LevelName)
                .ThenBy(x => x.PlanName)
                .ToList();

            foreach (var item in ordered)
                Items.Add(item);
        }
    }
}
