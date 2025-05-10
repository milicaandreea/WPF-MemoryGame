using MemoryGame.Models;
using System.Collections.ObjectModel;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        public ObservableCollection<User> Users { get; }

        public StatisticsViewModel()
        {
            var loaded = UserService.LoadUsers();
            Users = new ObservableCollection<User>(loaded);
        }
    }
}
