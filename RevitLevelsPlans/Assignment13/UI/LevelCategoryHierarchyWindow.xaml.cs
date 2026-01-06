
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
                    break;

                case HierarchyNodeType.Category:
                    ActivateLevelPlan(node.LevelId);
                    HighlightCategoryElementsOnLevel(node.LevelId, node.Category);
                    break;

                case HierarchyNodeType.Document:
                default:
                    break;
            }
        }

        private void ActivateLevelPlan(ElementId levelId)
        {
            if (levelId == ElementId.InvalidElementId) return;

            var viewPlan = new FilteredElementCollector(_doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .FirstOrDefault(v => v.ViewType == ViewType.FloorPlan && !v.IsTemplate && v.GenLevel?.Id == levelId);

            if (viewPlan == null)
            {
                TaskDialog.Show("Activate Level", "No floor plan found for this level.");
                return;
            }

            _uidoc.ActiveView = viewPlan;
            try
            {
                var uiViews = _uidoc.GetOpenUIViews();
                var thisView = uiViews.FirstOrDefault(v => v.ViewId == viewPlan.Id);
                thisView?.ZoomToFit();
            }
            catch { }
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

            _uidoc.Selection.SetElementIds(ids);

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
