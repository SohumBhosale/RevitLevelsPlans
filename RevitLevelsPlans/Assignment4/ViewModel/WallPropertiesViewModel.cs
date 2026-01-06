
using System.Collections.ObjectModel;
using System.Linq;

using Autodesk.Revit.DB;

using RevitLevelsPlans.Assignment4.Models;

namespace RevitLevelsPlans.Assignment4.ViewModels
{
    public class WallPropertiesViewModel
    {
        public ObservableCollection<WallPropertyModel> Properties { get; }

        public WallPropertiesViewModel(Element wall, Document doc)
        {
            Properties = new ObservableCollection<WallPropertyModel>();
            LoadWallProperties(wall, doc);
        }

        private void LoadWallProperties(Element wall, Document doc)
        {
            Properties.Add(new WallPropertyModel("ElementId", wall.Id.IntegerValue.ToString()));
            Properties.Add(new WallPropertyModel("UniqueId", wall.UniqueId));
            Properties.Add(new WallPropertyModel("Category", wall.Category?.Name ?? "(none)"));
            Properties.Add(new WallPropertyModel("Name", wall.Name));

            if (wall is Wall w)
            {
                var wallType = doc.GetElement(w.GetTypeId()) as WallType;
                Properties.Add(new WallPropertyModel("Wall Type", wallType?.Name ?? "(none)"));

                // Base Level
                var baseConstraintId = w.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT)?.AsElementId();
                var baseLevel = (baseConstraintId != null) ? doc.GetElement(baseConstraintId) as Level : null;
                Properties.Add(new WallPropertyModel("Base Level", baseLevel?.Name ?? "(none)"));

                Level topLevel = null;
                var topConstraintP = w.LookupParameter("Top Constraint");
                if (topConstraintP != null && topConstraintP.StorageType == StorageType.ElementId)
                {
                    var topId = topConstraintP.AsElementId();
                    if (topId != ElementId.InvalidElementId)
                        topLevel = doc.GetElement(topId) as Level;
                }

                string topInfo;
                if (topLevel != null)
                {
                    topInfo = topLevel.Name;
                }
                else
                {
                    var unconnectedHeightP = w.LookupParameter("Unconnected Height");
                    var heightStr = unconnectedHeightP?.AsValueString()
                                    ?? unconnectedHeightP?.AsDouble().ToString("0.######");
                    topInfo = heightStr != null ? $"Unconnected ({heightStr})" : "(none)";
                }
                Properties.Add(new WallPropertyModel("Top", topInfo));

                // Top Offset
                var topOffsetStr = w.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET)?.AsValueString();
                if (string.IsNullOrEmpty(topOffsetStr))
                    topOffsetStr = w.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET)?.AsDouble().ToString("0.######");
                Properties.Add(new WallPropertyModel("Top Offset", topOffsetStr ?? "(none)"));
            }

            Properties.Add(new WallPropertyModel("=== Parameters ===", ""));
            var paramsList = wall.Parameters.Cast<Parameter>()
                                .OrderBy(p => p.Definition?.Name)
                                .ToList();

            foreach (Parameter param in paramsList)
            {
                var pname = param.Definition?.Name ?? "(unnamed)";
                var pval = GetParameterValueString(param, doc);
                Properties.Add(new WallPropertyModel(pname, pval));
            }
        }

        private string GetParameterValueString(Parameter param, Document doc)
        {
            if (param == null) return "";

            switch (param.StorageType)
            {
                case StorageType.String:
                    return param.AsString() ?? "";

                case StorageType.Double:
                    var vs = param.AsValueString();
                    return !string.IsNullOrEmpty(vs) ? vs : param.AsDouble().ToString("0.######");

                case StorageType.Integer:
                    return param.AsInteger().ToString();

                case StorageType.ElementId:
                    var id = param.AsElementId();
                    if (id == ElementId.InvalidElementId) return "(none)";
                    var e = doc.GetElement(id);
                    return e?.Name ?? id.IntegerValue.ToString();

                default:
                    return "";
            }
        }
    }
}
