#  MemoryGame-WPF

A fully interactive memory card game developed in **WPF and C#**, following the **MVVM architectural pattern**. The application allows users to register with profile pictures, play standard or custom board games, save and resume game state, and track gameplay statistics. Gameplay is time-limited and image-based, with a dynamic board and real-time tile matching logic.

---

## ⚙️ Features

###  User Management
- Create new user accounts with profile pictures (JPG, PNG, GIF)
- Select from existing users to start playing
- Delete user accounts (removes profile image, saved games, and statistics)
- User data is stored in `Data/users.json`

###  Game Modes
- **Standard board**: 4x4 layout
- **Custom board**: User-defined MxN layout (even number of tiles only, 2–6 rows/columns)
- **Category selection**: Load tile images from image folders
- Each game has a randomized board configuration

###  Save & Resume
- Save your game at any point (category, time left, board state)
- Resume from where you left off, even after closing the app
- Game state stored per user in `SavedGames/saved_game_<username>.json`

###  Timer & Game Flow
- User sets a custom countdown timer before each game
- Timer is shown in real time; losing the game if time runs out
- Winning triggers automatic stat update and feedback
- Matched tiles remain visible; mismatches flip back after delay

###  Statistics
- Tracks each user's number of games played and won
- Displays statistics in a dedicated window
- Stats are persistent and updated automatically


---

## ▶️ How to Run

1. Clone the repository or download the source
2. Open the solution in **Visual Studio 2022+**
3. Build and run the project
4. Create a new user with a profile image or choose an existing one
5. Click **Play**, select a category and board size for a new game or open a saved game, and start playing!

---

## 📦 Dependencies & Technologies

- **WPF** (.NET 6 or higher)
- **C#** with MVVM pattern
- `System.Text.Json` for data persistence
- `DispatcherTimer` for real-time countdown
- `ObservableCollection`, `ICommand`, `INotifyPropertyChanged`
- Relative file paths for portability

---

> ⚠️ **Important:**  
> This repository includes only **one sample image category** (`BT21`) for demonstration purposes.  
> To test additional content, you can manually add your own categories and images to the `Images/Categories/` folder.  
> Each category must contain 18 distinct images and a `back.png` image representing the hidden tile face.

