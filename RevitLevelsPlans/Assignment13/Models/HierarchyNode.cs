
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment13.Models
{
    public enum HierarchyNodeType
    {
        Document,
        Level,
        Category
    }

    public class HierarchyNode
    {
        public string Name { get; set; }
        public HierarchyNodeType NodeType { get; set; }

        public ElementId LevelId { get; set; } = ElementId.InvalidElementId;

        public BuiltInCategory? Category { get; set; } = null;

        public ObservableCollection<HierarchyNode> Children { get; } =
            new ObservableCollection<HierarchyNode>();
    }
}
