
namespace RevitLevelsPlans.Assignment4.Models
{
    public class WallPropertyModel
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public WallPropertyModel(string name, string value)
        {
            Name = name;
            Value = value ?? "(none)";
        }
    }
}
