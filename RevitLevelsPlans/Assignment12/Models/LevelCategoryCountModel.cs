
namespace RevitLevelsPlans.Assignment12.Models
{
    public class LevelCategoryCountModel
    {
        public string CategoryName { get; set; }
        public int Count { get; set; }

        // Convenience for display
        public string Display => $"{CategoryName} ({Count})";
    }
}
