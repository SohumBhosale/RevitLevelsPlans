
using System;
using System.Reflection;

using Autodesk.Revit.UI;

namespace RevitLevelsPlans
{
    public class App : IExternalApplication
    {
        private const string TabName = "Assignment Tools";
        private const string PanelName = "Levels &amp; Plans";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                
                try { application.CreateRibbonTab(TabName); } catch {}

                // Get or create panel
                RibbonPanel panel = null;
                foreach (RibbonPanel p in application.GetRibbonPanels(TabName))
                {
                    if (p.Name.Equals(PanelName, StringComparison.OrdinalIgnoreCase))
                    {
                        panel = p; break;
                    }
                }
                if (panel == null)
                {
                    panel = application.CreateRibbonPanel(TabName, PanelName);
                }

                // Create a dropdown ("Assignments")
                var dropdownData = new PulldownButtonData("AssignmentsDropdown", "Assignments");
                var dropdown = panel.AddItem(dropdownData) as PulldownButton;
                dropdown.ToolTip = "Select an assignment to run";

                var asmPath = Assembly.GetExecutingAssembly().Location;
                var a1 = new PushButtonData(
                    "BtnListLevelsPlans",
                    "Assignment 1:\nList Levels & Plans",
                    asmPath,
                    "RevitLevelsPlans.ListLevelsAndPlansCommand" 
                );
                a1.ToolTip = "List Levels and their Floor Plan views";
                dropdown.AddPushButton(a1);

               
                var a2 = new PushButtonData(
                    "BtnActivateLevelPlan",
                    "Assignment 2:\nActivate Floor Plan",
                    asmPath,
                    "RevitLevelsPlans.Assignment2.Commands.ActivateLevelPlanCommand"
                );
                a2.ToolTip = "Open a window, pick a Level, and activate its Floor Plan view";
                dropdown.AddPushButton(a2);

                var a3 = new PushButtonData(
                    "BtnListWalls",
                    "Assignment 3:\nHighlits walls",
                    asmPath,
                    "RevitLevelsPlans.Assignment3.Commands.ActivateLevelPlanCommand"
                );
                //a3.ToolTip = "List Levels and their Floor Plan vies";
                dropdown.AddPushButton(a3);


                var a4 = new PushButtonData(
                    "ViewWallProperties",
                    "Assignment 4:\n walls properties",
                    asmPath,
                    "WallInspector.Command"
                );
                dropdown.AddPushButton(a4);

                var a5 = new PushButtonData(
                   "ViewWallProperties1",
                   "Assignment 5:\n walls properties1",
                   asmPath,
                   "RevitLevelsPlans.Assignment4.Commands.ShowWallPropertiesCommand"
               );
                dropdown.AddPushButton(a5);

                // ... keep your existing usings and class content ...
                // Add these inside OnStartup(...) after your existing buttons (a1..a5)

                var a6a = new PushButtonData(
                    "BtnActivatePlan",
                    "Assignment 6:\nActivate Plan",
                    asmPath,
                    "RevitLevelsPlans.Assignment6.Commands.ActivatePlanCommand"
                );
                a6a.ToolTip = "Pick a floor plan (by level) and activate it";
                dropdown.AddPushButton(a6a);

                var a6b = new PushButtonData(
                    "BtnShowRoomList",
                    "Assignment 6:\nShow Room List",
                    asmPath,
                    "RevitLevelsPlans.Assignment6.Commands.ShowRoomListCommand"
                );
                a6b.ToolTip = "Show the list of rooms in the currently active floor plan";
                dropdown.AddPushButton(a6b);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Startup Error", ex.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
    }
}
