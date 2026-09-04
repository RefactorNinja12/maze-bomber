using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.Board;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.GameState;
using System;

namespace BomberRoyal.Core
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        public IEventBus EventBus { get; }
        public MazeData Maze { get; private set; }
        private readonly MazeGenerator _mazeGenerator;
        public IGameState CurrentState { get; private set; }

        private const int MazeWidth = 40;
        private const int MazeHeight = 30;

        private GameManager()
        {
            EventBus = new EventBus();
            _mazeGenerator = new MazeGenerator();
        }

        public void ChangeState(IGameState newState)
        {
            if (CurrentState != null)
                CurrentState.Exit();

            CurrentState = newState;
            CurrentState.Enter();

            EventBus.Publish(new StateChangedEvent(newState));
        }

        public void Initialize()
        {
            
            ResetMaze();
        }
       
        public void ResetMaze()
        {
            _mazeGenerator.BorderWalls(MazeHeight, MazeWidth);
            _mazeGenerator.Generate(1, 1, MazeWidth - 2, MazeHeight - 2);
            _mazeGenerator.SmoothMaze(_mazeGenerator.GameBoard);

            Maze = new MazeData(MazeWidth, MazeHeight);

            for (int y = 0; y < MazeHeight; y++)
            {
                for (int x = 0; x < MazeWidth; x++)
                {
                    var ch = _mazeGenerator.GameBoard[y, x];

                    TileType type = ch == "@"
                        ? TileType.SolidWall
                        : (ch == "#" ? TileType.BreakableWall : TileType.Floor);

                    Maze.SetTile(x, y, new Tile(x, y, 42, type, EventBus));
                }
            }

            EventBus.Publish(new MazeGeneratedEvent(Maze));
        }

        public void Update(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }
    }
}