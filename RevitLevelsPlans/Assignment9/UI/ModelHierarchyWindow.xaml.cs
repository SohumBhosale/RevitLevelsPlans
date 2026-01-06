
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitLevelsPlans.Assignment9.Models;
using RevitLevelsPlans.Assignment9.ViewModel;

namespace RevitLevelsPlans.Assignment9.UI
{
    public partial class ModelHierarchyActivateWindow : Window
    {
        private readonly UIDocument _uidoc;
        private readonly Document _doc;

        public ModelHierarchyActivateWindow()
        {
            InitializeComponent();
        }

        public ModelHierarchyActivateWindow(ModelHierarchyViewModel vm, IntPtr revitMainHwnd, UIDocument uidoc) : this()
        {
            DataContext = vm;
            _uidoc = uidoc;
            _doc = uidoc.Document;

            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }

        private void OnTreeItemDoubleClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ActivateSelectedPlan();
        }

        private void OnSingleClickChecked(object sender, RoutedEventArgs e)
        {
            HierarchyTree.SelectedItemChanged += OnSelectedItemChangedActivate;
        }

        private void OnSingleClickUnchecked(object sender, RoutedEventArgs e)
        {
            HierarchyTree.SelectedItemChanged -= OnSelectedItemChangedActivate;
        }

        private void OnSelectedItemChangedActivate(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ActivateSelectedPlan();
        }

        private void ActivateSelectedPlan()
        {
            var node = HierarchyTree.SelectedItem as TreeNodeModel;
            if (node == null) return;

            if (node.ViewId == null || node.ViewId == ElementId.InvalidElementId)
                return;

            var view = _doc.GetElement(node.ViewId) as View;
            if (!(view is ViewPlan vp) || vp.ViewType != ViewType.FloorPlan)
            {
                MessageBox.Show(this, "Selected node is not a Floor Plan.", "Activate Plan",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _uidoc.ActiveView = view;
        }
    }
}
