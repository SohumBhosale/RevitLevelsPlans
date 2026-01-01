
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using RevitLevelsPlans.Assignment6.UI;
//using RevitLevelsPlans.Assignment6.ViewModel;
using RevitLevelsPlans.Assignment6.ViewModel;

using System;

namespace RevitLevelsPlans.Assignment6.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ShowRoomListCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            UIDocument uidoc = data.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                var activePlan = uidoc.ActiveView as ViewPlan;
                if (activePlan == null || activePlan.ViewType != ViewType.FloorPlan)
                {
                    TaskDialog.Show("Show Room List", "Please activate a Floor Plan first (use 'Activate Plan').");
                    return Result.Succeeded;
                }

                if (activePlan.GenLevel == null)
                {
                    TaskDialog.Show("Show Room List", "Active plan does not have an associated Level (GenLevel).");
                    return Result.Succeeded;
                }

                var vm = new RoomListViewModel(doc, activePlan);

                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var win = new RoomListWindow(vm, hwnd);
                win.ShowDialog();

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
