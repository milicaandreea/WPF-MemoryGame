using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class User
    {
        public required string Name { get; set; }
        public required string ImagePath { get; set; }

        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
    }
}

