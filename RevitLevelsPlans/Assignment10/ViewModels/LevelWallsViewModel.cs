
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Autodesk.Revit.DB;
using RevitLevelsPlans.Assignment10.Models;

namespace RevitLevelsPlans.Assignment10.ViewModel
{
    public class LevelWallsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LevelNodeModel> Levels { get; } =
            new ObservableCollection<LevelNodeModel>();

        private LevelNodeModel _selectedLevel;
        public LevelNodeModel SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                if (_selectedLevel != value)
                {
                    _selectedLevel = value;
                    OnPropertyChanged(nameof(SelectedLevel));
                }
            }
        }

        public LevelWallsViewModel(Document doc)
        {
            var walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            var grouped = walls
                .GroupBy(w => w.LevelId ?? ElementId.InvalidElementId)
                .OrderBy(g => GetLevelName(doc, g.Key));

            foreach (var grp in grouped)
            {
                var levelName = GetLevelName(doc, grp.Key);
                var levelNode = new LevelNodeModel
                {
                    LevelId = grp.Key,
                    LevelName = levelName
                };

                foreach (var w in grp.OrderBy(x => x.Name))
                {
                    var wt = doc.GetElement(w.GetTypeId()) as WallType;

                    string lenStr = null;
                    var lenP = w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                    lenStr = lenP?.AsValueString();
                    if (string.IsNullOrEmpty(lenStr) && lenP != null)
                        lenStr = lenP.AsDouble().ToString("0.##");

                    string thickStr = null;
                    var widthP = wt?.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);
                    thickStr = widthP?.AsValueString();
                    if (string.IsNullOrEmpty(thickStr) && widthP != null)
                        thickStr = widthP.AsDouble().ToString("0.###");

                    levelNode.Walls.Add(new WallInfoModel
                    {
                        Id = w.Id.IntegerValue,
                        Name = w.Name,
                        Type = wt?.Name ?? "(none)",
                        Length = lenStr ?? "(none)",
                        Thickness = thickStr ?? "(none)",
                        ElementId = w.Id
                    });
                }

                Levels.Add(levelNode);
            }

            SelectedLevel = Levels.FirstOrDefault();
        }

        private string GetLevelName(Document doc, ElementId levelId)
        {
            if (levelId == ElementId.InvalidElementId) return "(none)";
            var lvl = doc.GetElement(levelId) as Level;
            return lvl?.Name ?? "(none)";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
