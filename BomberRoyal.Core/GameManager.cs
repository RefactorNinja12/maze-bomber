using BomberRoyal.Core.GameState;
using BomberRoyal.Core.Features.Board;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberRoyal.Core.Event;

namespace BomberRoyal.Core
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        public MazeData Maze { get; private set; }
        private MazeGenerator _mazeGenerator;
        public IGameState CurrentState { get; private set; }

        private const int MazeWidth = 40;
        private const int MazeHeight = 30;
        private const int TileSize = 32; 

        private GameManager()
        {
            _mazeGenerator = new MazeGenerator();
            
        }
        public void ChangeState(IGameState newState)
        {
            if (CurrentState != null)
                CurrentState.Exit();

            CurrentState = newState;
            CurrentState.Enter();

            EventBus.Publish(GameEvents.StateChanged, newState);
        }
        public void Initialize()
        {
            _mazeGenerator.BorderWalls(MazeHeight, MazeWidth);
            _mazeGenerator.Generate(1, 1, MazeWidth - 2, MazeHeight - 2);
            _mazeGenerator.SmoothMaze(_mazeGenerator.GameBoard);

            Maze = new MazeData(MazeWidth, MazeHeight);

            for(int y = 0; y < MazeHeight; y++)
            {
                for(int x = 0; x < MazeWidth; x++)
                {
                    bool isDestructible = _mazeGenerator.GameBoard[y, x] != "@";
                    var tile = new Tile(x, y, TileSize, isDestructible);
                    Maze.SetTile(x, y, tile);
                }
            }

            EventBus.Publish(GameEvents.MazeGenerated, Maze);
        }
        public void Update(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }
    }
}
