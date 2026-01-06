using System.Collections.ObjectModel;
namespace RevitLevelsPlans.Assignment8.Models
{
    public class TreeNodeModel
    {
        public string Name { get; set; }

        public ObservableCollection<TreeNodeModel> Children { get; } =
        new ObservableCollection<TreeNodeModel>();

        public TreeNodeModel() { }
        public TreeNodeModel(string name) { Name = name; }
    }
}
