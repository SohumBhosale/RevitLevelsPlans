
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RevitLevelsPlans.UI
{
    public partial class LevelsPlansWindow : Window
    {
        private readonly List<RevitLevelsPlans.LevelPlanRow> _rows;

        public LevelsPlansWindow(List<RevitLevelsPlans.LevelPlanRow> rows)
        {
            InitializeComponent();                   // requires XAML Build Action = Page

            _rows = rows ?? new List<RevitLevelsPlans.LevelPlanRow>();
            dgLevels.ItemsSource = _rows;            // bind directly to your model
        }

        private void OnCopy(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Level Name\tElevation (ft)\tFloor Plans");
            foreach (var r in _rows)
            {
                sb.AppendLine($"{r.LevelName}\t{r.ElevationFeet:F3}\t{r.PlansJoined}");
            }
            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Copied to clipboard.", "Levels & Floor Plans",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnClose(object sender, RoutedEventArgs e) => Close();

        private void dgLevels_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {




        }
    }
}
