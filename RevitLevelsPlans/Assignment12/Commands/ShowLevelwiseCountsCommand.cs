
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using RevitLevelsPlans.Assignment12.ViewModel;
using RevitLevelsPlans.Assignment12.UI;
namespace RevitLevelsPlans.Assignment12.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ShowLevelwiseCountsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            try
            {
                var vm = new LevelwiseCountsViewModel(doc);

                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var win = new LevelwiseCountsWindow(vm, hwnd);
                win.ShowDialog();

                return Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Error", ex.ToString());
                return Result.Failed;
            }
        }
    }
}
