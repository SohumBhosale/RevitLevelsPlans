
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment12.Models
{
    public class LevelNodeModel12
    {
        public string LevelName { get; set; }
        public ElementId LevelId { get; set; } = ElementId.InvalidElementId;

        public bool IsDocumentHeader { get; set; }

        public ObservableCollection<LevelNodeModel12> Children { get; } =
            new ObservableCollection<LevelNodeModel12>();

        public ObservableCollection<LevelCategoryCountModel> Categories { get; } =
            new ObservableCollection<LevelCategoryCountModel>();
    }
}
