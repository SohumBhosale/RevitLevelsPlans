
using System;
using System.Linq;
using System.Reflection;

using Autodesk.Revit.UI;

namespace RevitLevelsPlans
{
    public class App : IExternalApplication
    {
        private const string TabName = "Assignment Tools";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                try { application.CreateRibbonTab(TabName); } catch {}

                string asmPath = Assembly.GetExecutingAssembly().Location;

                CreatePanelWithButton(
                    application, TabName, "Assignment 1",
                    "Assignment 1:\nList Levels & Plans",
                    asmPath,
                    "RevitLevelsPlans.ListLevelsAndPlansCommand",
                    "List Levels and their Floor Plan views"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 2",
                    "Assignment 2:\nActivate Floor Plan",
                    asmPath,
                    "RevitLevelsPlans.Assignment2.Commands.ActivateLevelPlanCommand",
                    "Open a window, pick a Level, and activate its Floor Plan view"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 3",
                    "Assignment 3:\nHighlits walls",
                    asmPath,
                    "RevitLevelsPlans.Assignment3.Commands.ActivateLevelPlanCommand",
                    "Highlight or operate on walls (your Assignment 3 command)"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 4",
                    "Assignment 4:\nwalls properties",
                    asmPath,
                    "WallInspector.Command",
                    "Open external Wall Inspector (Assignment 4)"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 5",
                    "Assignment 5:\nwalls properties1",
                    asmPath,
                    "RevitLevelsPlans.Assignment4.Commands.ShowWallPropertiesCommand",
                    "Show wall properties (your Assignment 5)"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 6 (Activate Plan)",
                    "Assignment 6:\nActivate Plan",
                    asmPath,
                    "RevitLevelsPlans.Assignment6.Commands.ActivatePlanCommand",
                    "Pick a floor plan (by level) and activate it"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 6 (Room List)",
                    "Assignment 6:\nShow Room List",
                    asmPath,
                    "RevitLevelsPlans.Assignment6.Commands.ShowRoomListCommand",
                    "Show the list of rooms in the currently active floor plan"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 7",
                    "Assignment 7:\nRoom Walls",
                    asmPath,
                    "RevitLevelsPlans.Assignment7.Commands.RoomWallsBySelectionCommand",
                    "Select a room and list its bounding walls"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 8",
                    "Assignment 8:\nModel Hierarchy",
                    asmPath,
                    "RevitLevelsPlans.Assignment8.Commands.ShowModelHierarchyCommand",
                    "Open a WPF TreeView showing the document name and its Floor Plan views"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 9",
                    "Assignment 9:\nActivate From Hierarchy",
                    asmPath,
                    "RevitLevelsPlans.Assignment9.Commands.ActivateFromHierarchyCommand",
                    "Open the model hierarchy and activate a floor plan by double‑clicking it"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 10",
                    "Assignment 10:\nLevelwise Walls",
                    asmPath,
                    "RevitLevelsPlans.Assignment10.Commands.ShowLevelwiseWallsCommand",
                    "Open a WPF form that shows walls grouped by Level"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 11",
                    "Assignment 11:\nWall Openings (Multi)",
                    asmPath,
                    "RevitLevelsPlans.Assignment11.Commands.ShowWallOpeningsCommand",
                    "Select multiple walls and list their openings (doors/windows/custom openings)"
                );

                CreatePanelWithButton(
                    application, TabName, "Assignment 12",
                    "Assignment 12:\nElement Count",
                    asmPath,
                    "RevitLevelsPlans.Assignment12.Commands.ShowLevelwiseCountsCommand",
                    "Level Wise Element Count"
                );
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Startup Error", ex.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

        private static void CreatePanelWithButton(
            UIControlledApplication app,
            string tabName,
            string panelName,
            string buttonText,
            string assemblyPath,
            string commandClassFullName,
            string tooltip)
        {
            // Ensure panel exists (reuse if present)
            RibbonPanel panel = EnsurePanel(app, tabName, panelName);

            var pbd = new PushButtonData(
                MakeSafeName("PB_" + panelName), // internal unique name
                buttonText,                      // visible text
                assemblyPath,
                commandClassFullName
            );

            var push = panel.AddItem(pbd) as PushButton;
            if (push != null)
            {
                push.ToolTip = tooltip;
            }
        }

        private static RibbonPanel EnsurePanel(UIControlledApplication app, string tabName, string panelName)
        {
            var panels = app.GetRibbonPanels(tabName);
            var existing = panels.FirstOrDefault(p => p.Name.Equals(panelName, StringComparison.OrdinalIgnoreCase));
            if (existing != null) return existing;

            return app.CreateRibbonPanel(tabName, panelName);
        }
        private static string MakeSafeName(string name)
        {
            var safe = new string(name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.').ToArray());
            return string.IsNullOrEmpty(safe) ? Guid.NewGuid().ToString("N") : safe;
        }
    }
}
