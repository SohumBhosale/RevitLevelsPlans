
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

using RevitLevelsPlans.Assignment10.ViewModel;
using RevitLevelsPlans.Assignment10.UI;

namespace RevitLevelsPlans.Assignment10.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ShowLevelwiseWallsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            UIDocument uidoc = data.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Build the ViewModel (group walls by level)
                var vm = new LevelWallsViewModel(doc);

                // Owner = Revit main window handle (no Autodesk.Windows dependency)
                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                // Open the WPF window
                var win = new LevelwiseWallsWindow(vm, hwnd);
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
