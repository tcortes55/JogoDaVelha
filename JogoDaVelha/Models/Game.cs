using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JogoDaVelha.Models
{
    public class Game
    {
        public Guid id = Guid.NewGuid();
        public string firstPlayer;
        public string nextPlayer;
        public int numRounds = 0;
        public string winner = string.Empty;
        public string[][] map = new string[3][];
    }
}