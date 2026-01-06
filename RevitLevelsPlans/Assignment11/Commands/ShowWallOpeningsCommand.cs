
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

using RevitLevelsPlans.Assignment11.ViewModel;
using RevitLevelsPlans.Assignment11.UI;

namespace RevitLevelsPlans.Assignment11.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ShowWallOpeningsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            try
            {
                // 1) Collect walls from preselection
                var walls = new List<Wall>();
                var selectedIds = uidoc.Selection.GetElementIds();

                if (selectedIds.Count > 0)
                {
                    foreach (var id in selectedIds)
                    {
                        var w = doc.GetElement(id) as Wall;
                        if (w != null) walls.Add(w);
                    }
                }

                // 2) If none, prompt for multi-pick
                if (walls.Count == 0)
                {
                    var pickedRefs = uidoc.Selection.PickObjects(ObjectType.Element, new WallSelectionFilter(),
                        "Select one or more Walls");
                    if (pickedRefs == null || pickedRefs.Count == 0)
                        return Result.Cancelled;

                    foreach (var r in pickedRefs)
                    {
                        var w = doc.GetElement(r.ElementId) as Wall;
                        if (w != null) walls.Add(w);
                    }
                }

                if (walls.Count == 0)
                {
                    TaskDialog.Show("Wall Openings", "No walls selected.");
                    return Result.Succeeded;
                }

                // 3) Build ViewModel and show WPF
                var vm = new WallOpeningsViewModel(doc, walls);

                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var win = new WallOpeningsWindow(vm, hwnd);
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

        private class WallSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e) => e is Wall;
            public bool AllowReference(Reference r, XYZ p) => false;
        }
    }
}
