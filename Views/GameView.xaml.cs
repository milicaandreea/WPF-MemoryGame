using MemoryGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGame.Views
{
    public partial class GameView : Window
    {
        public GameView()
        {
            InitializeComponent();

            DataContext = new GameViewModel(); 

        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new Views.AboutDialog();
            aboutDialog.Owner = this;
            aboutDialog.ShowDialog();
        }

    }

}
