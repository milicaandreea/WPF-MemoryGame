using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;
using MemoryGame.ViewModels;

namespace MemoryGame.Services
{
    public static class GameSaveService
    {
        public static void SaveGame(User user, ObservableCollection<TileViewModel> tiles, string category, int timeLeft, int rows, int columns)
        {
            var save = new SavedGame
            {
                Category = category,
                Rows = rows,
                Columns = columns,
                TimeLeft = timeLeft,
                Tiles = tiles.Select(vm => new TileState
                {
                    ImagePath = vm.Tile.ImagePath,
                    IsFlipped = vm.IsFlipped,
                    IsMatched = vm.IsMatched
                }).ToList()
            };

            string folder = "SavedGames";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string userPath = Path.Combine(folder, $"saved_game_{user.Name}.json");
            string json = JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(userPath, json);
        }
        public static SavedGame? LoadGame(User user)
        {
            string filePath = Path.Combine("SavedGames", $"saved_game_{user.Name}.json");

            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SavedGame>(json);
        }

    }
}
