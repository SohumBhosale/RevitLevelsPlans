
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitLevelsPlans.Assignment4.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ShowWallPropertiesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            UIDocument uidoc = data.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Wall wall = null;

                // Use preselection if exactly one element is selected; otherwise prompt
                var selectedIds = uidoc.Selection.GetElementIds();
                if (selectedIds.Count == 1)
                {
                    wall = doc.GetElement(selectedIds.First()) as Wall;
                    if (wall == null)
                    {
                        TaskDialog.Show("Wall Properties", "Selected element is not a wall.");
                        return Result.Failed;
                    }
                }
                else
                {
                    var pickedRef = uidoc.Selection.PickObject(ObjectType.Element, new WallSelectionFilter(), "Select a wall");
                    if (pickedRef == null) return Result.Cancelled;
                    wall = doc.GetElement(pickedRef.ElementId) as Wall;
                    if (wall == null)
                    {
                        TaskDialog.Show("Wall Properties", "Selected element is not a wall.");
                        return Result.Failed;
                    }
                }

                var sb = new StringBuilder();

                // Header info
                sb.AppendLine($"ElementId: {wall.Id.IntegerValue}");
                sb.AppendLine($"UniqueId: {wall.UniqueId}");
                sb.AppendLine($"Category: {wall.Category?.Name}");
                sb.AppendLine($"Name: {wall.Name}");
                var wallType = doc.GetElement(wall.GetTypeId()) as WallType;
                sb.AppendLine($"Wall Type: {wallType?.Name}");

                // Base level (built-in exists across versions)
                var baseConstraintId = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT)?.AsElementId();
                var baseLevel = (baseConstraintId != null) ? doc.GetElement(baseConstraintId) as Level : null;
                sb.AppendLine($"Base Level: {baseLevel?.Name ?? "(none)"}");

                // Top constraint robust handling
                Level topLevel = null;
                var topConstraintP = wall.LookupParameter("Top Constraint");
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
                    var unconnectedHeightP = wall.LookupParameter("Unconnected Height");
                    var heightStr = unconnectedHeightP?.AsValueString() ?? unconnectedHeightP?.AsDouble().ToString("0.######");
                    topInfo = heightStr != null ? $"Unconnected ({heightStr})" : "(none)";
                }
                sb.AppendLine($"Top: {topInfo}");

                // Top offset (null-safe)
                var topOffsetStr = wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET)?.AsValueString();
                if (string.IsNullOrEmpty(topOffsetStr))
                    topOffsetStr = wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET)?.AsDouble().ToString("0.######");
                sb.AppendLine($"Top Offset: {topOffsetStr ?? "(none)"}");

                // Parameters - copy to list to sort
                sb.AppendLine();
                sb.AppendLine("=== Parameters ===");
                var parameters = new List<Parameter>();
                foreach (Parameter p in wall.Parameters) parameters.Add(p);

                foreach (Parameter p in parameters.OrderBy(x => x.Definition?.Name))
                {
                    string pname = p.Definition?.Name ?? "(unnamed)";
                    string pval = GetParamValueString(p, doc);
                    sb.AppendLine($"{pname}: {pval}");
                }

                TaskDialog.Show("Wall Properties", sb.ToString());
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Error", ex.ToString());
                return Result.Failed;
            }
        }

        private string GetParamValueString(Parameter p, Document doc)
        {
            if (p == null) return "";
            switch (p.StorageType)
            {
                case StorageType.String:
                    return p.AsString() ?? "";
                case StorageType.Double:
                    var vs = p.AsValueString();
                    return !string.IsNullOrEmpty(vs) ? vs : p.AsDouble().ToString("0.######");
                case StorageType.Integer:
                    return p.AsInteger().ToString();
                case StorageType.ElementId:
                    var id = p.AsElementId();
                    if (id == ElementId.InvalidElementId) return "(none)";
                    var e = doc.GetElement(id);
                    return e?.Name ?? id.IntegerValue.ToString();
                default:
                    return "";
            }
        }
    }

    // Correct filter signature (XYZ for second arg)
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is Wall;
        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
