
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using RevitLevelsPlans.Assignment7.UI;
using RevitLevelsPlans.Assignment7.ViewModel;

using System;
using System.Linq;

namespace RevitLevelsPlans.Assignment7.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class RoomWallsBySelectionCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            UIDocument uidoc = data.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Room room = null;
                var selectedIds = uidoc.Selection.GetElementIds();

                if (selectedIds.Count == 1)
                {
                    room = doc.GetElement(selectedIds.First()) as Room;
                    if (room == null)
                    {
                        TaskDialog.Show("Room Walls", "Selected element is not a Room.");
                        return Result.Succeeded;
                    }
                }
                else
                {
                    var picked = uidoc.Selection.PickObject(ObjectType.Element, new RoomSelectionFilter(), "Select a Room");
                    if (picked == null) return Result.Cancelled;

                    room = doc.GetElement(picked.ElementId) as Room;
                    if (room == null)
                    {
                        TaskDialog.Show("Room Walls", "Selected element is not a Room.");
                        return Result.Succeeded;
                    }
                }

                if (room.Location == null)
                {
                    TaskDialog.Show("Room Walls", "The selected Room is not placed.");
                    return Result.Succeeded;
                }

                var vm = new RoomWallsViewModel(doc, room);

                if (!vm.Walls.Any())
                {
                    TaskDialog.Show("Room Walls",
                        $"No bounding walls found for Room: {vm.RoomNumber} - {vm.RoomName}.\n" +
                        "The room may be not enclosed or boundaries are non-wall elements.");
                    return Result.Succeeded;
                }

                IntPtr hwnd = data.Application.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                var win = new RoomWallsWindow(vm, hwnd);
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

        private class RoomSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem) => elem is Room;
            public bool AllowReference(Reference reference, XYZ position) => false;
        }
    }
}
