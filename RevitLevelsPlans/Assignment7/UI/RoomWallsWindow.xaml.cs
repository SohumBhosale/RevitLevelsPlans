
using System;
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment7.ViewModel;

namespace RevitLevelsPlans.Assignment7.UI
{
    public partial class RoomWallsWindow : Window
    {
        public RoomWallsWindow()
        {
            InitializeComponent();
        }

        public RoomWallsWindow(RoomWallsViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }
    }
}
