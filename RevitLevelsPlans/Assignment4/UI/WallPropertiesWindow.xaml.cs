
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment4.ViewModels;

namespace RevitLevelsPlans.Assignment4.UI
{
    public partial class WallPropertiesWindow : Window
    {
        public WallPropertiesWindow()
        {
            InitializeComponent();
        }

        public WallPropertiesWindow(WallPropertiesViewModel vm, System.IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;

            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }
    }
}
