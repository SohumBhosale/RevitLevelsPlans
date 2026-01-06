
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

using RevitLevelsPlans.Assignment14.UI;
using RevitLevelsPlans.Assignment14.ViewModel;

namespace RevitLevelsPlans.Assignment14.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class SetDoorSizeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            try
            {
                var doors = new List<FamilyInstance>();
                var selectedIds = uidoc.Selection.GetElementIds();
                if (selectedIds?.Count > 0)
                {
                    foreach (var id in selectedIds)
                    {
                        var fi = doc.GetElement(id) as FamilyInstance;
                        if (fi?.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
                            doors.Add(fi);
                    }
                }

                if (doors.Count == 0)
                {
                    try
                    {
                        var refs = uidoc.Selection.PickObjects(ObjectType.Element, new DoorSelectionFilter(),
                            "Select one or more doors");
                        doors.AddRange(refs
                            .Select(r => doc.GetElement(r.ElementId) as FamilyInstance)
                            .Where(fi => fi != null));
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        var rectPicked = uidoc.Selection.PickElementsByRectangle(new DoorSelectionFilter());
                        doors.AddRange(rectPicked.OfType<FamilyInstance>());
                    }
                }

                if (doors.Count == 0)
                {
                    TaskDialog.Show("Set Door Size", "No doors selected.");
                    return Result.Succeeded;
                }

                var vm = new DoorSizeViewModel(doc, doors);

                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var win = new DoorSizeWindow(vm, hwnd);
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

        private class DoorSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                var fi = e as FamilyInstance;
                return fi?.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_Doors;
            }
            public bool AllowReference(Reference r, XYZ p) => false;
        }
    }
}
