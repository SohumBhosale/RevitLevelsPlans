
using System;
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment12.ViewModel;

namespace RevitLevelsPlans.Assignment12.UI
{
    public partial class LevelwiseCountsWindow : Window
    {
        public LevelwiseCountsWindow()
        {
            InitializeComponent();
        }
        public LevelwiseCountsWindow(LevelwiseCountsViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }
    }
}
