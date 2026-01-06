
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

using RevitLevelsPlans.Assignment13.ViewModel;
using RevitLevelsPlans.Assignment13.UI;

namespace RevitLevelsPlans.Assignment13.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class HighlightFromHierarchyCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            try
            {
                // Build the Level → Category hierarchy
                var vm = new LevelCategoryHierarchyViewModel(doc);

                // Owner = Revit main window (no Autodesk.Windows dependency)
                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var win = new LevelCategoryHierarchyWindow(vm, hwnd, uidoc);
                win.ShowDialog();

                return Result.Succeeded;
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
