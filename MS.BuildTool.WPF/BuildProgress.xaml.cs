using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MS.BuildTool.WPF
{
    /// <summary>
    /// Interaction logic for BuildProgress.xaml
    /// </summary>
    public partial class BuildProgress 
    {
        private GridViewColumnHeader _listViewSortCol = null;
        private SortAdorner _listViewSortAdorner = null;

        public BuildProgress()
        {
            InitializeComponent();

            this.DataContext = new BuildProgressViewModel();

            LogTextBox.TextChanged += LogTextBox_TextChanged;
        }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogTextBox.ScrollToEnd();
        }

        private void lvUsersColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();

            if (_listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(_listViewSortCol).Remove(_listViewSortAdorner);
                ProjectsListView.Items.SortDescriptions.Clear();
            }

            var newDir = ListSortDirection.Ascending;
            if (_listViewSortCol == column && _listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            _listViewSortCol = column;
            _listViewSortAdorner = new SortAdorner(_listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(_listViewSortCol).Add(_listViewSortAdorner);
            ProjectsListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void SoultionClicked(object sender, RoutedEventArgs e)
        {
            var hyperLink = sender as Hyperlink;
            if (hyperLink != null)
            {
                Process.Start(hyperLink.NavigateUri.LocalPath);
            }
        }
    }
}
