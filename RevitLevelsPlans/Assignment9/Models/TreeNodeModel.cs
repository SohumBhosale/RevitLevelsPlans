
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment9.Models
{

    public class TreeNodeModel
    {
        public string Name { get; set; }
        public ElementId ViewId { get; set; } = ElementId.InvalidElementId;

        public ObservableCollection<TreeNodeModel> Children { get; } =
            new ObservableCollection<TreeNodeModel>();

        public TreeNodeModel() { }
        public TreeNodeModel(string name) { Name = name; }
    }
}
