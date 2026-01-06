
using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment10.Models
{
    public class WallInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Length { get; set; } 
        public string Thickness { get; set; }
        public ElementId ElementId { get; set; } = ElementId.InvalidElementId;
    }
}
