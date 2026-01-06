
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

        // Level info (used for Level nodes and Category nodes under a level)
        public ElementId LevelId { get; set; } = ElementId.InvalidElementId;

        // Category info (used for Category nodes)
        public BuiltInCategory? Category { get; set; } = null;

        // Child nodes (document → levels, level → categories)
        public ObservableCollection<HierarchyNode> Children { get; } =
            new ObservableCollection<HierarchyNode>();
    }
}
