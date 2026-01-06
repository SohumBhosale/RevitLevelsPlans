
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment12.Models
{
    public class LevelNodeModel12
    {
        public string LevelName { get; set; }
        public ElementId LevelId { get; set; } = ElementId.InvalidElementId;

        // Top (document header) vs actual level node
        public bool IsDocumentHeader { get; set; }

        // Children: either level nodes (when IsDocumentHeader) or category count rows
        public ObservableCollection<LevelNodeModel12> Children { get; } =
            new ObservableCollection<LevelNodeModel12>();

        // For a level node: we show categories as second tier
        public ObservableCollection<LevelCategoryCountModel> Categories { get; } =
            new ObservableCollection<LevelCategoryCountModel>();
    }
}
