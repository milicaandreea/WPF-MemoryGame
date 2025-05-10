using Microsoft.Win32;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class CreateUserDialog : Window
    {
        public string? UserName { get; private set; }
        public string? ImagePath { get; private set; }

        public CreateUserDialog()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.png, *.gif)|*.jpg;*.png;*.gif"
            };

            if (dlg.ShowDialog() == true)
            {
                ImagePathBox.Text = dlg.FileName;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(ImagePathBox.Text))
            {
                MessageBox.Show("Please enter a name and select an image.");
                return;
            }

            UserName = NameBox.Text.Trim();
            ImagePath = ImagePathBox.Text;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
