
using System.Text;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace WallInspector
{
    [Transaction(TransactionMode.ReadOnly)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            try
            {
                // Pick a wall
                var pick = uiDoc.Selection.PickObject(ObjectType.Element, new WallSelectionFilter(), "Select a wall");
                if (pick == null)
                {
                    TaskDialog.Show("Failed", "");

                    return Result.Cancelled;

                }
                var wall = doc.GetElement(pick) as Wall;
                if (wall == null)
                {
                    message = "Selected element is not a wall.";
                    TaskDialog.Show("Failed","");
                    return Result.Failed;
                }

                var sb = new StringBuilder();

                // Type name
                var wallType = doc.GetElement(wall.GetTypeId()) as WallType;
                sb.AppendLine("Type: " + (wallType?.Name ?? "(no type)"));

                // Length (Instance parameter, formatted to project units)
                var lenParam = wall.LookupParameter("Length");
                sb.AppendLine("Length: " + (lenParam?.AsValueString() ?? "(n/a)"));

                // Thickness (Type parameter "Width", formatted to project units)
                var widthParam = wallType?.LookupParameter("Width");
                sb.AppendLine("Thickness: " + (widthParam?.AsValueString() ?? "(n/a)"));

                TaskDialog.Show("Wall Properties", sb.ToString());
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled; // user pressed Esc
            }
        }
    }
}
