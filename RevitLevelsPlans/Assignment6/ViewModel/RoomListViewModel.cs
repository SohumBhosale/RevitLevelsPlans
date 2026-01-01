
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

using RevitLevelsPlans.Assignment6.Models;

using System.Collections.ObjectModel;
using System.Linq;

namespace RevitLevelsPlans.Assignment6.ViewModel
{
    public class RoomListViewModel
    {
        public ObservableCollection<RoomInfoModel> Rooms { get; } = new ObservableCollection<RoomInfoModel>();
        public string ViewName { get; }

        public RoomListViewModel(Document doc, ViewPlan plan)
        {
            ViewName = plan.Name;

            var genLevel = plan.GenLevel;
            ElementId levelId = genLevel?.Id ?? ElementId.InvalidElementId;

            var rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<SpatialElement>()
                .OfType<Room>()
                .Where(r => levelId != ElementId.InvalidElementId && r.LevelId == levelId)
                .OrderBy(r => r.Number)
                .ThenBy(r => r.Name)
                .ToList();

            foreach (var r in rooms)
            {
                var areaParam = r.get_Parameter(BuiltInParameter.ROOM_AREA);
                var areaStr = areaParam?.AsValueString() ?? r.Area.ToString("0.##");

                Rooms.Add(new RoomInfoModel
                {
                    Number = r.Number,
                    Name = r.Name,
                    Level = genLevel?.Name ?? "(none)",
                    Area = areaStr
                });
            }

            if (Rooms.Count == 0)
            {
                Rooms.Add(new RoomInfoModel
                {
                    Number = "",
                    Name = "(No rooms found on this level)",
                    Level = genLevel?.Name ?? "(none)",
                    Area = ""
                });
            }
        }
    }
}
