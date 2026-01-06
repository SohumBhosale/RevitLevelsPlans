
using System;
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment11.ViewModel;

namespace RevitLevelsPlans.Assignment11.UI
{
    public partial class WallOpeningsWindow : Window
    {
        public WallOpeningsWindow()
        {
            InitializeComponent(); // requires XAML Build Action = Page
        }

        public WallOpeningsWindow(WallOpeningsViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }
    }
}
