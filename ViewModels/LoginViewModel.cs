using MemoryGame.Commands;
using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MemoryGame.Views;
using System.IO;
using MemoryGame.Services;
using System.Windows;


namespace MemoryGame.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private User? selectedUser;
        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public User? SelectedUser
        {
            get => selectedUser;
            set => SetProperty(ref selectedUser, value);
        }
        public ICommand CreateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }

        public LoginViewModel()
        {
            CreateUserCommand = new RelayCommand(_ => CreateUser());
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            PlayCommand = new RelayCommand(_ => Play(), _ => SelectedUser != null);
            var loaded = UserService.LoadUsers();
            foreach (var user in loaded)
            {
                Users.Add(user);
            }

        }
        private void CreateUser()
        {
            var dialog = new CreateUserDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string name = dialog.UserName?.Trim() ?? string.Empty;
                string? sourcePath = dialog.ImagePath;

                if (string.IsNullOrWhiteSpace(name))
                {
                    System.Windows.MessageBox.Show("Username can not be empty.");
                    return;
                }

                if (Users.Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    System.Windows.MessageBox.Show("This username already exists.");
                    return;
                }

                string localFolder = Path.Combine("Images", "Users");
                Directory.CreateDirectory(localFolder); 

                string fileName = $"{name}{Path.GetExtension(sourcePath)}";
                string destinationPath = Path.Combine(localFolder, fileName);

                try
                {
                    File.Copy(sourcePath!, destinationPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to copy image: {ex.Message}");
                    return;
                }

                string relativePath = Path.Combine("Images", "Users", fileName);

                Users.Add(new User
                {
                    Name = name,
                    ImagePath = relativePath,
                    GamesPlayed = 0,
                    GamesWon = 0
                });

                UserService.SaveUsers(Users);
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                var user = SelectedUser; 

                Users.Remove(user); 

                string imageFullPath = Path.Combine(Directory.GetCurrentDirectory(), user.ImagePath);
                if (File.Exists(imageFullPath))
                {
                    try
                    {
                        File.Delete(imageFullPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                string gamePath = Path.Combine("SavedGames", $"saved_game_{user.Name}.json");
                if (File.Exists(gamePath))
                {
                    try
                    {
                        File.Delete(gamePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete saved game: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                UserService.SaveUsers(Users);
            }
        }


        private void Play()
        {
            var gameView = new GameView();
            if (gameView.DataContext is GameViewModel gvm)
            {
                gvm.SelectedUser = SelectedUser;
            }

            gameView.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.LoginView)
                {
                    w.Close();
                    break;
                }
            }
        }

    }
}
