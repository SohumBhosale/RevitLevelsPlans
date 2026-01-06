
using RevitLevelsPlans.Assignment10.Models;
using RevitLevelsPlans.Assignment10.ViewModel;
using System;
using System.Windows;
using System.Windows.Interop;

namespace RevitLevelsPlans.Assignment10.UI
{
    public partial class LevelwiseWallsWindow : Window
    {
        public LevelwiseWallsWindow()
        {
            InitializeComponent(); // Requires XAML Build Action = Page
        }
        public LevelwiseWallsWindow(LevelWallsViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;

            // Center/Modal over Revit
            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }

        private void OnLevelSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var vm = DataContext as LevelWallsViewModel;
                if (vm == null) return;

                // The TreeView's SelectedItem is a LevelNodeModel
                var levelNode = LevelsTree.SelectedItem as LevelNodeModel;
                if (levelNode != null)
                    vm.SelectedLevel = levelNode;
            }
            catch
            {
                // swallow event errors silently; binding will remain valid
            }
        }
    }
}
