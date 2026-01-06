
using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment11.Models
{
    public class OpeningInfoModel
    {
        public int Id { get; set; }
        public string Kind { get; set; }
        public string FamilyName { get; set; }
        public string TypeName { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Details { get; set; }
        public string Level { get; set; }
        public ElementId ElementId { get; set; } = ElementId.InvalidElementId;

        
        public int HostWallId { get; set; }
        public string HostWallName { get; set; }
    }
}
