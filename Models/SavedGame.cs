using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class SavedGame
    {
        public string Category { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int TimeLeft { get; set; }

        public List<TileState> Tiles { get; set; } = new();
    }
}

