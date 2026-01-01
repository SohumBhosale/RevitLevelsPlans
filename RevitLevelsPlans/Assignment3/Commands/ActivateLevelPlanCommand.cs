
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace RevitLevelsPlans.Assignment3.Commands
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ActivateLevelPlanCommand:IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uiapp = data.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var win = new UI.ActivateLevelPlanWindow(uidoc, doc);
            win.ShowDialog();

            return Result.Succeeded;
        }
    }
}
