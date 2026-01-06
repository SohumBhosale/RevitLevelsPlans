
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using RevitLevelsPlans.Assignment11.Models;
using System.Collections.Generic;

namespace RevitLevelsPlans.Assignment11.ViewModel
{
    public class WallOpeningsViewModel
    {
        public string SelectionSummary { get; }

        public ObservableCollection<OpeningInfoModel> Items { get; } =
            new ObservableCollection<OpeningInfoModel>();

        public WallOpeningsViewModel(Document doc, IEnumerable<Wall> walls)
        {
            var wallList = walls?.Where(w => w != null).ToList() ?? new List<Wall>();
            SelectionSummary = $"Walls selected: {wallList.Count}";

            foreach (var wall in wallList)
            {
                var wt = doc.GetElement(wall.GetTypeId()) as WallType;
                var wl = doc.GetElement(wall.LevelId) as Level;
                var wallName = wall.Name;
                var wallId = wall.Id.IntegerValue;

                var insertIds = wall.FindInserts(
                    addRectOpenings: true,
                    includeShadows: true,
                    includeEmbeddedWalls: true,
                    includeSharedEmbeddedInserts: true
                );

                foreach (var id in insertIds.OrderBy(x => x.IntegerValue))
                {
                    var elem = doc.GetElement(id);
                    var row = new OpeningInfoModel
                    {
                        Id = id.IntegerValue,
                        ElementId = id,
                        Level = (elem.LevelId != ElementId.InvalidElementId)
                                       ? (doc.GetElement(elem.LevelId) as Level)?.Name
                                       : "(none)",
                        HostWallId = wallId,
                        HostWallName = $"{wallName} (Id: {wallId})"
                    };

                    if (elem is FamilyInstance fi)
                    {
                        var cat = fi.Category?.Name ?? "";
                        if (string.IsNullOrEmpty(cat))
                            row.Kind = "FamilyInstance";
                        else if (cat.ToLower().Contains("door"))
                            row.Kind = "Door";
                        else if (cat.ToLower().Contains("window"))
                            row.Kind = "Window";
                        else
                            row.Kind = cat;

                        var sym = fi.Symbol;
                        row.FamilyName = sym?.Family?.Name ?? fi.Name;
                        row.TypeName = sym?.Name ?? "";

                        row.Width = GetAnyWidth(fi, sym);
                        row.Height = GetAnyHeight(fi, sym);

                        row.Details = $"Host wall: {wallName}";
                    }
                    else if (elem is Opening op)
                    {
                        row.Kind = "Opening";
                        row.FamilyName = "(Opening)";
                        row.TypeName = "";
                        row.Width = "";
                        row.Height = "";

                        if (op.IsRectBoundary)
                        {
                            var rect = op.BoundaryRect;
                            var min = rect[0];
                            var max = rect[1];
                            row.Details = $"Rect: Min({min.X:0.###},{min.Y:0.###},{min.Z:0.###}) / Max({max.X:0.###},{max.Y:0.###},{max.Z:0.###})";
                        }
                        else
                        {
                            var count = op.BoundaryCurves?.Size ?? 0;
                            row.Details = $"Curves: {count}";
                        }
                    }
                    else if (elem is Wall embedded)
                    {
                        row.Kind = "Embedded Wall";
                        var ewt = doc.GetElement(embedded.GetTypeId()) as WallType;
                        row.FamilyName = embedded.Name;
                        row.TypeName = ewt?.Name ?? "";
                        row.Details = "Embedded/Joined";
                    }
                    else
                    {
                        row.Kind = elem.Category?.Name ?? elem.GetType().Name;
                        row.FamilyName = elem.Name;
                        row.TypeName = "";
                        row.Details = "(unclassified insert)";
                    }

                    Items.Add(row);
                }
            }
        }

        private string GetAnyWidth(FamilyInstance fi, ElementType sym)
        {
            var w = (fi.LookupParameter("Width")?.AsValueString()
                     ?? fi.LookupParameter("Width")?.AsDouble().ToString("0.###"));
            if (!string.IsNullOrEmpty(w)) return w;

            var sw = (sym?.LookupParameter("Width")?.AsValueString()
                      ?? sym?.LookupParameter("Width")?.AsDouble().ToString("0.###"));
            return string.IsNullOrEmpty(sw) ? "(none)" : sw;
        }

        private string GetAnyHeight(FamilyInstance fi, ElementType sym)
        {
            var h = (fi.LookupParameter("Height")?.AsValueString()
                     ?? fi.LookupParameter("Height")?.AsDouble().ToString("0.###"));
            if (!string.IsNullOrEmpty(h)) return h;

            var sh = (sym?.LookupParameter("Height")?.AsValueString()
                      ?? sym?.LookupParameter("Height")?.AsDouble().ToString("0.###"));
            return string.IsNullOrEmpty(sh) ? "(none)" : sh;
        }
    }
}
