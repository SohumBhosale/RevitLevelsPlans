
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
            InitializeComponent();
        }
        public LevelwiseWallsWindow(LevelWallsViewModel vm, IntPtr revitMainHwnd) : this()
        {
            DataContext = vm;

            new WindowInteropHelper(this) { Owner = revitMainHwnd };
        }

        private void OnLevelSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var vm = DataContext as LevelWallsViewModel;
                if (vm == null) return;

                var levelNode = LevelsTree.SelectedItem as LevelNodeModel;
                if (levelNode != null)
                    vm.SelectedLevel = levelNode;
            }
            catch
            {
            }
        }
    }
}
