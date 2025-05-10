using MemoryGame.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MemoryGame.Services
{
    public static class UserService
    {
        private static readonly string filePath = "Data/users.json";

        public static List<User> LoadUsers()
        {
            if (!File.Exists(filePath))
                return new List<User>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static void SaveUsers(IEnumerable<User> users)
        {
            Directory.CreateDirectory("Data");
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
