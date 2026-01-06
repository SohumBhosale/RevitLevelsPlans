
using Autodesk.Revit.DB;
using RevitLevelsPlans.Assignment10.Models;
using RevitLevelsPlans.Assignment12.Models;

using System.Collections.ObjectModel;
using System.Linq;

namespace RevitLevelsPlans.Assignment12.ViewModel
{
    public class LevelwiseCountsViewModel
    {
        public ObservableCollection<LevelNodeModel12> Roots { get; } =
            new ObservableCollection<LevelNodeModel12>();

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

        public LevelwiseCountsViewModel(Document doc)
        {
            // Root node (document title)
            string docTitle = string.IsNullOrWhiteSpace(doc.Title) ? doc.PathName : doc.Title;
            var root = new LevelNodeModel12 { LevelName = docTitle, IsDocumentHeader = true };

            // Collect Levels
            var levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(l => l.Elevation)
                .ThenBy(l => l.Name)
                .ToList();

            foreach (var level in levels)
            {
                var levelNode = new LevelNodeModel12
                {
                    LevelName = level.Name,
                    LevelId = level.Id
                };

                // Per Category, count elements at this level
                foreach (var bic in TargetCategories)
                {
                    int count = CountElementsOnLevel(doc, level, bic);
                    var catName = CategoryName(doc, bic);

                    levelNode.Categories.Add(new LevelCategoryCountModel
                    {
                        CategoryName = catName,
                        Count = count
                    });
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
        private string CategoryName(Document doc, BuiltInCategory bic)
        {
            var cat = doc.Settings.Categories.get_Item(bic);
            return cat?.Name ?? bic.ToString();
        }
    }
}
