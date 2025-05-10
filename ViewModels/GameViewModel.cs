using MemoryGame.Models;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MemoryGame.Commands;
using System.Windows.Input;
using System.IO;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        public ObservableCollection<TileViewModel> Tiles { get; }

        private TileViewModel? firstFlipped;
        private TileViewModel? secondFlipped;
        private DispatcherTimer timer = new();
        private int timeLeft;
        private string selectedCategory = string.Empty;
        public User? SelectedUser { get; set; }

        public string SelectedCategory
        {
            get => selectedCategory;
            set => SetProperty(ref selectedCategory, value);
        }
        public ICommand GenerateBoardCommand { get; }

        public ICommand SelectCategoryCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetStandardSizeCommand { get; }
        public ICommand SetCustomSizeCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand ShowStatisticsCommand { get; }


        private int boardRows = 1;
        public int BoardRows
        {
            get => boardRows;
            set => SetProperty(ref boardRows, value);
        }

        private int boardColumns = 1;
        public int BoardColumns
        {
            get => boardColumns;
            set => SetProperty(ref boardColumns, value);
        }

        public int TimeLeft
        {
            get => timeLeft;
            set => SetProperty(ref timeLeft, value);
        }

        public GameViewModel()
        {
            Tiles = new ObservableCollection<TileViewModel>();
            SelectCategoryCommand = new RelayCommand(param =>
            {
                if (param is string category)
                {
                    SelectedCategory = category;
                }
            });
            GenerateBoardCommand = new RelayCommand(_ => GenerateBoard(), _ => !string.IsNullOrEmpty(SelectedCategory));
            ExitCommand = new RelayCommand(_ => ExitToLogin());
            SetStandardSizeCommand = new RelayCommand(_ =>
            {
                BoardRows = 4;
                BoardColumns = 4;
            });

            SetCustomSizeCommand = new RelayCommand(_ =>
            {
                var dialog = new Views.CustomSizeDialog(); 
                if (dialog.ShowDialog() == true)
                {
                    BoardRows = dialog.SelectedRows;
                    BoardColumns = dialog.SelectedColumns;
                }
            });
            SaveGameCommand = new RelayCommand(_ => SaveGame(), _ => Tiles.Any());
            OpenGameCommand = new RelayCommand(_ =>
            {
                if (SelectedUser != null)
                    LoadSavedGame(SelectedUser);
            });
            ShowStatisticsCommand = new RelayCommand(_ =>
            {
                var statsWindow = new Views.StatisticsView();
                statsWindow.ShowDialog();
            });

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeLeft--;

            if (TimeLeft <= 0)
            {
                timer.Stop();
                if (SelectedUser != null)
                {
                    var users = UserService.LoadUsers();
                    var userToUpdate = users.FirstOrDefault(u => u.Name == SelectedUser.Name);
                    if (userToUpdate != null)
                    {
                        userToUpdate.GamesPlayed++;
                        UserService.SaveUsers(users);
                    }
                }

                MessageBox.Show("Time is up! You lost the game.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                Application.Current.Shutdown(); 
            }
        }
        private void GenerateBoard()
        {
            if (BoardRows < 2 || BoardColumns < 2)
            {
                MessageBox.Show("Please select de board size first (Standard or Custom).", "Missing board size", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectedCategory))
            {
                MessageBox.Show("Choose a category before starting the game.", "Missing category", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string folder = Path.Combine("Images", "Categories", SelectedCategory);
            if (!Directory.Exists(folder))
            {
                MessageBox.Show($"{SelectedCategory} category folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var imageFiles = Directory.GetFiles(folder, "*.png")
                .Concat(Directory.GetFiles(folder, "*.jpg"))
                .Concat(Directory.GetFiles(folder, "*.jpeg"))
                .Where(file => !Path.GetFileName(file).Equals("back.png", StringComparison.OrdinalIgnoreCase))
                .ToList();
            int totalTiles = BoardRows * BoardColumns;
            int totalPairs = totalTiles / 2;

            var selected = imageFiles.OrderBy(_ => Guid.NewGuid()).Take(totalPairs).ToList();
            var allImages = selected.Concat(selected).OrderBy(_ => Guid.NewGuid()).ToList();


            string backImagePath = Path.Combine(folder, "back.png");
            string backRelative = Path.GetRelativePath(Directory.GetCurrentDirectory(), backImagePath);

            Tiles.Clear();
            foreach (var imgPath in allImages)
            {
                string relative = Path.GetRelativePath(Directory.GetCurrentDirectory(), imgPath);
                var tile = new Tile
                {
                    ImagePath = relative,
                    BackImagePath = backRelative
                };

                Tiles.Add(new TileViewModel(tile, OnTileClicked, backRelative)); 
            }
            var timeDialog = new Views.TimeDialog();
            if (timeDialog.ShowDialog() != true)
            {
                return; 
            }

            TimeLeft = timeDialog.SelectedTime;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void OnTileClicked(TileViewModel clicked)
        {
            if (firstFlipped == null)
            {
                firstFlipped = clicked;
            }
            else if (secondFlipped == null && clicked != firstFlipped)
            {
                secondFlipped = clicked;

                if (firstFlipped.ImagePath == secondFlipped.ImagePath)
                {
                    firstFlipped.Match();
                    secondFlipped.Match();
                    if (Tiles.All(t => t.IsMatched))
                    {
                        timer.Stop();
                        if (SelectedUser != null)
                        {
                            var users = UserService.LoadUsers();
                            var userToUpdate = users.FirstOrDefault(u => u.Name == SelectedUser.Name);
                            if (userToUpdate != null)
                            {
                                userToUpdate.GamesPlayed++;
                                userToUpdate.GamesWon++;
                                UserService.SaveUsers(users);
                            }
                        }

                        MessageBox.Show("Congratulations! You won the game!", "Victory", MessageBoxButton.OK, MessageBoxImage.Information);
                        foreach (Window w in Application.Current.Windows)
                        {
                            if (w is Views.GameView)
                                w.Close();
                        }
                    }
                }
                else
                {
                    var f1 = firstFlipped;
                    var f2 = secondFlipped;

                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            f1.Hide();
                            f2.Hide();
                        });
                    });
                }

                firstFlipped = null;
                secondFlipped = null;
            }
        }
        private static void ExitToLogin()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var login = new Views.LoginView();
                login.Show();

                foreach (Window w in Application.Current.Windows)
                {
                    if (w is Views.GameView)
                    {
                        w.Close();
                        break;
                    }
                }
            });
        }
        private void SaveGame()
        {
            if (SelectedUser == null) return;

            GameSaveService.SaveGame(
                SelectedUser,
                Tiles,
                SelectedCategory,
                TimeLeft,
                BoardRows,
                BoardColumns
            );

            MessageBox.Show("Game saved succesfully!", "Saved Game", MessageBoxButton.OK, MessageBoxImage.Information);

            timer?.Stop();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var login = new Views.LoginView();
                login.Show();

                foreach (Window w in Application.Current.Windows)
                {
                    if (w is Views.GameView)
                    {
                        w.Close();
                        break;
                    }
                }
            });
        }

        public void LoadSavedGame(User user)
        {
            var saved = GameSaveService.LoadGame(user);
            if (saved == null)
            {
                MessageBox.Show("There is no saved game for this user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedCategory = saved.Category;
            BoardRows = saved.Rows;
            BoardColumns = saved.Columns;
            TimeLeft = saved.TimeLeft;

            string folder = Path.Combine("Images", "Categories", SelectedCategory);
            string backImagePath = Path.Combine(folder, "back.png");
            string backRelative = Path.GetRelativePath(Directory.GetCurrentDirectory(), backImagePath);

            Tiles.Clear();
            foreach (var state in saved.Tiles)
            {
                var tile = new Tile
                {
                    ImagePath = state.ImagePath,
                    IsFlipped = state.IsFlipped,
                    IsMatched = state.IsMatched,
                    BackImagePath = backRelative
                };
                Tiles.Add(new TileViewModel(tile, OnTileClicked, backRelative));
            }

            if (timer != null)
            {
                timer.Stop();
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            var flippedUnmatched = Tiles.Where(t => t.IsFlipped && !t.IsMatched).ToList();

            if (flippedUnmatched.Count >= 2)
            {
                firstFlipped = flippedUnmatched[0];
                secondFlipped = flippedUnmatched[1];

                for (int i = 2; i < flippedUnmatched.Count; i++)
                    flippedUnmatched[i].Hide();
            }
            else if (flippedUnmatched.Count == 1)
            {
                firstFlipped = flippedUnmatched[0];
            }

        }

    }
}
