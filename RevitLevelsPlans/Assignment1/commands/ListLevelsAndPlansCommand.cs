
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using RevitLevelsPlans.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;

namespace RevitLevelsPlans
{
    [Transaction(TransactionMode.Manual)]
    public class ListLevelsAndPlansCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = (uidoc == null) ? null : uidoc.Document;

            if (doc == null)
            {
                message = "No active document.";
                return Result.Failed;
            }

            try
            {
                var levels = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .WhereElementIsNotElementType()
                    .Cast<Level>()
                    .OrderBy(l => l.Elevation)
                    .ToList();

                var floorPlans = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewPlan))
                    .WhereElementIsNotElementType()
                    .Cast<ViewPlan>()
                    .Where(vp => vp.ViewType == ViewType.FloorPlan && !vp.IsTemplate && vp.GenLevel != null)
                    .Select(vp => new { vp.Name, LevelId = vp.GenLevel.Id })
                    .ToList();

                var plansLookup = floorPlans.ToLookup(x => x.LevelId, x => x.Name);

                var rows = levels
                    .Select(lvl => new LevelPlanRow
                    {
                        LevelName = lvl.Name,
                        ElevationFeet = lvl.Elevation,                                   // NO conversion
                        FloorPlanNames = plansLookup[lvl.Id].OrderBy(n => n).ToList()     // sorted names
                    })
                    .ToList();

                var window = new LevelsPlansWindow(rows);

                var revitHwnd = uiapp.MainWindowHandle;

                var helper = new WindowInteropHelper(window) { Owner = revitHwnd };

                window.ShowDialog();


                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
