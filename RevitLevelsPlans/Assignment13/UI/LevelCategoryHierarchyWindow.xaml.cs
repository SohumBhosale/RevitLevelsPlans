
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using RevitLevelsPlans.Assignment13.Models;
using RevitLevelsPlans.Assignment13.ViewModel;

namespace RevitLevelsPlans.Assignment13.UI
{
    public partial class LevelCategoryHierarchyWindow : Window
    {
        private readonly UIDocument _uidoc;
        private readonly Document _doc;

        public LevelCategoryHierarchyWindow()
        {
            InitializeComponent();
        }

        public LevelCategoryHierarchyWindow(LevelCategoryHierarchyViewModel vm, IntPtr revitMainHwnd, UIDocument uidoc) : this()
        {
            DataContext = vm;
            _uidoc = uidoc;
            _doc = uidoc.Document;

            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Optional: if you want single-click behavior, call HandleSelection here.
            // HandleSelection(HierarchyTree.SelectedItem as HierarchyNode);
        }

        private void OnNodeDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HandleSelection(HierarchyTree.SelectedItem as HierarchyNode);
        }

        private void HandleSelection(HierarchyNode node)
        {
            if (node == null) return;

            switch (node.NodeType)
            {
                case HierarchyNodeType.Level:
                    ActivateLevelPlan(node.LevelId);
                    // No selection for level-only click (as per steps). You can select all categories if desired.
                    break;

                case HierarchyNodeType.Category:
                    ActivateLevelPlan(node.LevelId);
                    HighlightCategoryElementsOnLevel(node.LevelId, node.Category);
                    break;

                case HierarchyNodeType.Document:
                default:
                    // Do nothing on document node
                    break;
            }
        }

        private void ActivateLevelPlan(ElementId levelId)
        {
            if (levelId == ElementId.InvalidElementId) return;

            // Find a floor plan whose GenLevel == this level
            var viewPlan = new FilteredElementCollector(_doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .FirstOrDefault(v => v.ViewType == ViewType.FloorPlan && !v.IsTemplate && v.GenLevel?.Id == levelId);

            if (viewPlan == null)
            {
                TaskDialog.Show("Activate Level", "No floor plan found for this level.");
                return;
            }

            // Activate synchronously in an external command context
            _uidoc.ActiveView = viewPlan;

            // Optional UX: Zoom to fit (requires UIView)
            try
            {
                var uiViews = _uidoc.GetOpenUIViews();
                var thisView = uiViews.FirstOrDefault(v => v.ViewId == viewPlan.Id);
                thisView?.ZoomToFit();
            }
            catch { /* ignore if not available */ }
        }

        private void HighlightCategoryElementsOnLevel(ElementId levelId, BuiltInCategory? bicMaybe)
        {
            if (levelId == ElementId.InvalidElementId || bicMaybe == null) return;
            var bic = bicMaybe.Value;

            var levelFilter = new ElementLevelFilter(levelId);
            var ids = new FilteredElementCollector(_doc)
                .OfCategory(bic)
                .WhereElementIsNotElementType()
                .WherePasses(levelFilter)
                .ToElementIds()
                .ToList();

            if (ids.Count == 0)
            {
                TaskDialog.Show("Highlight", "No elements found for this category on the selected level.");
                return;
            }

            // Select (highlight) them in the active view
            _uidoc.Selection.SetElementIds(ids);

            // Optional: temporarily isolate them to visually emphasize
            // NOTE: Requires a transaction.
            using (var t = new Transaction(_doc, "Temporary Isolate"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    _uidoc.ActiveView.IsolateElementsTemporary(new HashSet<ElementId>(ids));
                    t.Commit();
                }
            }
        }
    }
}
