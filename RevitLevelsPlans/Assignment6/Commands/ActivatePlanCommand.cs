
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

using RevitLevelsPlans.Assignment6.ViewModel;
using RevitLevelsPlans.Assignment6.UI;

namespace RevitLevelsPlans.Assignment6.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ActivatePlanCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            try
            {
                // Collect non-template floor plan views
                var plans = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewPlan))
                    .Cast<ViewPlan>()
                    .Where(v => v.ViewType == ViewType.FloorPlan && !v.IsTemplate)
                    .ToList();

                if (plans.Count == 0)
                {
                    TaskDialog.Show("Activate Plan", "No floor plan views found in this project.");
                    return Result.Succeeded;
                }

                // ViewModel + picker window
                var vm = new LevelPlanPickerViewModel(plans);

                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var picker = new LevelPlanPickerWindow(vm, hwnd);
                var dialogResult = picker.ShowDialog();
                if (dialogResult != true || vm.Selected == null)
                    return Result.Cancelled;

                // Resolve selected view
                var selectedView = doc.GetElement(vm.Selected.ViewId) as View;
                if (selectedView == null)
                {
                    TaskDialog.Show("Activate Plan", "Selected view could not be resolved.");
                    return Result.Failed;
                }

                // ✅ Preferred: set ActiveView directly (immediate change in ExternalCommand)
                uidoc.ActiveView = selectedView;
























































































































































































































                if (uidoc.ActiveView?.Id != selectedView.Id)
                {
                    message = "Failed to activate the selected view.";
                    return Result.Failed;
                }
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Error", ex.ToString());
                return Result.Failed;
            }
        }
    }
}
