
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment10.Models
{
    public class LevelNodeModel
    {
        public string LevelName { get; set; }
        public ElementId LevelId { get; set; } = ElementId.InvalidElementId;

        public ObservableCollection<WallInfoModel> Walls { get; } =
            new ObservableCollection<WallInfoModel>();
    }
}
