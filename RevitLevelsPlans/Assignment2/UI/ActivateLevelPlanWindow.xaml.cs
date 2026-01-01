
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace RevitLevelsPlans.Assignment2.UI
{
    public partial class ActivateLevelPlanWindow : Window
    {
        private readonly UIDocument _uidoc;
        private readonly Document _doc;

        public ActivateLevelPlanWindow(UIDocument uidoc, Document doc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            _doc = doc;

            LoadLevels();
        }

        private void LoadLevels()
        {
            //var levels = new FilteredElementCollector(_doc)
            //             .OfClass(typeof(Level))
            //             .Cast<Level>()
            //             .OrderBy(l => l.Elevation)  // internal feet
            //             .ToList();

            var levels = new FilteredElementCollector(_doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(f => !f.IsTemplate && f.ViewType == ViewType.FloorPlan)
                .ToList();


            cbLevels.SelectedIndex = -1;

            cbLevels.Text = "Select floor";
            cbLevels.ItemsSource = levels;
        }
        private void OnShow(object sender, RoutedEventArgs e)
        {
            var plan = cbLevels.SelectedItem as View;
            if (plan == null)
            {
                MessageBox.Show("Please select a Level.", "Activate Floor Plan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _uidoc.RequestViewChange(plan);

        }
        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
