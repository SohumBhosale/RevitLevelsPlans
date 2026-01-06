
using System;
using System.Windows;
using System.Windows.Interop;

using RevitLevelsPlans.Assignment14.ViewModel;

namespace RevitLevelsPlans.Assignment14.UI
{
    public partial class DoorSizeWindow : Window   // ✅ partial, inherits Window
    {
        private readonly DoorSizeViewModel _vm;

        public DoorSizeWindow()
        {
            InitializeComponent();                 // ✅ must be here
        }

        public DoorSizeWindow(DoorSizeViewModel vm, IntPtr revitMainHwnd) : this()
        {
            _vm = vm;
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _vm.ApplyChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Set Door Size",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
