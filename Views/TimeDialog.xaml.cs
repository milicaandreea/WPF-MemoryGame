using System.Windows;
using System.Windows.Controls;

namespace MemoryGame.Views
{
    public partial class TimeDialog : Window
    {
        public int SelectedTime { get; private set; } = 60;

        public TimeDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (TimeComboBox.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content.ToString(), out int time))
            {
                SelectedTime = time;
            }

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
