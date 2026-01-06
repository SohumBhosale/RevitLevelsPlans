
using System;
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment8.ViewModel;

namespace RevitLevelsPlans.Assignment8.UI
{
    public partial class ModelHierarchyWindow : Window
    {
        public ModelHierarchyWindow()
        {
            InitializeComponent();
        }

        public ModelHierarchyWindow(ModelHierarchyViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }
    }
}
