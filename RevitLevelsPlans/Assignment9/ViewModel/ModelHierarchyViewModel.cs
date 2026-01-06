
using System.Collections.ObjectModel;
using System.Linq;

using Autodesk.Revit.DB;

using RevitLevelsPlans.Assignment9.Models;

namespace RevitLevelsPlans.Assignment9.ViewModel
{
    public class ModelHierarchyViewModel
    {
        public ObservableCollection<TreeNodeModel> Roots { get; } =
            new ObservableCollection<TreeNodeModel>();

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
                var child = new TreeNodeModel($"{plan.Name}  (Level: {levelName})")
                {
                    ViewId = plan.Id 
                };
                root.Children.Add(child);
            }

            Roots.Add(root);
        }
    }
}
