
using System;
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment6.ViewModel;

namespace RevitLevelsPlans.Assignment6.UI
{
    public partial class RoomListWindow : Window
    {
        public RoomListWindow()
        {
            InitializeComponent();
        }

        public RoomListWindow(RoomListViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }
    }
}
