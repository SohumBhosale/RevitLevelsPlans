
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
                // 1) Levels (instances), ordered by elevation (feet)
                var levels = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .WhereElementIsNotElementType()
                    .Cast<Level>()
                    .OrderBy(l => l.Elevation)
                    .ToList();

                // 2) Floor Plan views (instances), exclude templates, keep only those with GenLevel
                var floorPlans = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewPlan))
                    .WhereElementIsNotElementType()
                    .Cast<ViewPlan>()
                    .Where(vp => vp.ViewType == ViewType.FloorPlan && !vp.IsTemplate && vp.GenLevel != null)
                    .Select(vp => new { vp.Name, LevelId = vp.GenLevel.Id })      // project to anonymous type
                    .ToList();

                // 3) ToLookup: LevelId -> [plan names]  (LINQ-friendly, returns empty seq for missing keys)
                var plansLookup = floorPlans.ToLookup(x => x.LevelId, x => x.Name);

                // 4) Shape rows with LINQ
                var rows = levels
                    .Select(lvl => new LevelPlanRow
                    {
                        LevelName = lvl.Name,
                        ElevationFeet = lvl.Elevation,                                   // NO conversion
                        FloorPlanNames = plansLookup[lvl.Id].OrderBy(n => n).ToList()     // sorted names
                    })
                    .ToList();

                // 5) Show WPF window
                var window = new LevelsPlansWindow(rows);
                //var window = new LevelPlanWindow(items);

                // Set Revit main window as owner so the dialog behaves properly

                var revitHwnd = uiapp.MainWindowHandle;

                var helper = new WindowInteropHelper(window) { Owner = revitHwnd };

                window.ShowDialog(); // modal — simplest & safest


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
