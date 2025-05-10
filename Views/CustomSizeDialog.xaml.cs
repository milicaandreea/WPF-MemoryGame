using System.Windows;
using System.Windows.Controls;

namespace MemoryGame.Views
{
    public partial class CustomSizeDialog : Window
    {
        public int SelectedRows { get; private set; } = 4;
        public int SelectedColumns { get; private set; } = 4;

        public CustomSizeDialog()
        {
            InitializeComponent();
            RowsBox.SelectedIndex = 2; 
            ColsBox.SelectedIndex = 2; 
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (RowsBox.SelectedItem is not ComboBoxItem rowItem || rowItem.Content is not string rowContent)
            {
                MessageBox.Show("Please select a valid number of rows.", "Missing Rows", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ColsBox.SelectedItem is not ComboBoxItem colItem || colItem.Content is not string colContent)
            {
                MessageBox.Show("Please select a valid number of columns.", "Missing Columns", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int rows = int.Parse(rowContent);
            int cols = int.Parse(colContent);

            if ((rows * cols) % 2 != 0)
            {
                MessageBox.Show("The total number of tiles should be even.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedRows = rows;
            SelectedColumns = cols;

            DialogResult = true;
            Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
