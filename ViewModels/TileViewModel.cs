using MemoryGame.Models;
using MemoryGame.Commands;
using System;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class TileViewModel : BaseViewModel
    {
        private readonly Tile tile;
        private readonly Action<TileViewModel> onTileClicked;
        public Tile Tile => tile;

        public TileViewModel(Tile tile, Action<TileViewModel> onTileClicked, string backImagePath)
        {
            this.tile = tile;
            this.onTileClicked = onTileClicked;
            tile.BackImagePath = backImagePath;

            FlipCommand = new RelayCommand(_ => Flip(), _ => !tile.IsMatched);
        }

        public string ImagePath
        {
            get
            {
                string path = tile.IsFlipped || tile.IsMatched ? tile.ImagePath : tile.BackImagePath;
                return $"pack://siteoforigin:,,,/{path.Replace("\\", "/")}";
            }
        }

        public bool IsFlipped => tile.IsFlipped;
        public bool IsMatched => tile.IsMatched;

        public ICommand FlipCommand { get; }

        public void Flip()
        {
            if (tile.IsMatched || tile.IsFlipped)
                return;

            tile.IsFlipped = true;
            OnPropertyChanged(nameof(ImagePath));
            onTileClicked?.Invoke(this);
        }

        public void Hide()
        {
            tile.IsFlipped = false;
            OnPropertyChanged(nameof(ImagePath));
        }

        public void Match()
        {
            tile.IsMatched = true;
            OnPropertyChanged(nameof(ImagePath));
        }
    }
}
