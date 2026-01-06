
using System.Collections.ObjectModel;
using System.Linq;

using Autodesk.Revit.DB;

using RevitLevelsPlans.Assignment13.Models;

namespace RevitLevelsPlans.Assignment13.ViewModel
{
    public class LevelCategoryHierarchyViewModel
    {
        public ObservableCollection<HierarchyNode> Roots { get; } =
            new ObservableCollection<HierarchyNode>();

        // Categories to display under each level
        private static readonly BuiltInCategory[] TargetCategories =
        {
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Columns
        };

        public LevelCategoryHierarchyViewModel(Document doc)
        {
            // Document root
            string docTitle = string.IsNullOrWhiteSpace(doc.Title) ? doc.PathName : doc.Title;
            var root = new HierarchyNode
            {
                Name = docTitle,
                NodeType = HierarchyNodeType.Document
            };

            // Levels
            var levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(l => l.Elevation)
                .ThenBy(l => l.Name)
                .ToList();

            // For each level, add categories (with counts in display name)
            foreach (var level in levels)
            {
                var levelNode = new HierarchyNode
                {
                    Name = level.Name,
                    NodeType = HierarchyNodeType.Level,
                    LevelId = level.Id
                };

                foreach (var bic in TargetCategories)
                {
                    int count = CountElementsOnLevel(doc, level, bic);
                    var catName = doc.Settings.Categories.get_Item(bic)?.Name ?? bic.ToString();

                    var catNode = new HierarchyNode
                    {
                        Name = $"{catName} ({count})",
                        NodeType = HierarchyNodeType.Category,
                        LevelId = level.Id,
                        Category = bic
                    };

                    levelNode.Children.Add(catNode);
                }

                root.Children.Add(levelNode);
            }

            Roots.Add(root);
        }

        private int CountElementsOnLevel(Document doc, Level level, BuiltInCategory bic)
        {
            var levelFilter = new ElementLevelFilter(level.Id);
            return new FilteredElementCollector(doc)
                .OfCategory(bic)
                .WhereElementIsNotElementType()
                .WherePasses(levelFilter)
                .GetElementCount();
        }
    }
}
