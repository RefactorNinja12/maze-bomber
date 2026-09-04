using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Event
{
    public class GameEvents
    {
        public const string StartGame = "StartGame";
        public const string GameOver = "GameOver";
        public const string StateChanged = "StateChanged";
        public const string MazeGenerated = "MazeGenerated";
        public const string PlayerDied = "Player Died";
        public const string ReturnToMenu = "Return to menu";
    }
}
