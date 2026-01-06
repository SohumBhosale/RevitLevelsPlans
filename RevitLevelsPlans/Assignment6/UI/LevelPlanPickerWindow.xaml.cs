
using RevitLevelsPlans.Assignment6.ViewModel;

using System;
using System.Windows;
using System.Windows.Interop;

namespace RevitLevelsPlans.Assignment6.UI
{
    public partial class LevelPlanPickerWindow : Window
    {
        public LevelPlanPickerWindow()
        {
            InitializeComponent();
        }

        public LevelPlanPickerWindow(LevelPlanPickerViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }

        private void OnActivateClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LevelPlanPickerViewModel;
            if (vm?.Selected == null)
            {
                MessageBox.Show(this, "Please select a plan to activate.", "Activate Floor Plan",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
