
using Autodesk.Revit.DB;

using RevitLevelsPlans.Assignment8.Models;

using System.Collections.ObjectModel;
using System.Linq;

namespace RevitLevelsPlans.Assignment8.ViewModel
{
    public class ModelHierarchyViewModel
    {
        public ObservableCollection<TreeNodeModel> Roots { get; } = new ObservableCollection<TreeNodeModel>();

        public ModelHierarchyViewModel(Document doc)
        {
            string docTitle = string.IsNullOrWhiteSpace(doc.Title) ? doc.PathName : doc.Title;
            var root = new TreeNodeModel(docTitle);

            var plans = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(v => v.ViewType == ViewType.FloorPlan && !v.IsTemplate)
                .OrderBy(v => v.GenLevel?.Name)
                .ThenBy(v => v.Name)
                .ToList();

            foreach (var plan in plans)
            {
                string levelName = plan.GenLevel?.Name ?? "(no level)";
                root.Children.Add(new TreeNodeModel($"{plan.Name}  (Level: {levelName})"));
            }
            Roots.Add(root);
        }
    }
}
