using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class TileState
    {
        public string ImagePath { get; set; } = string.Empty;
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
    }
}

