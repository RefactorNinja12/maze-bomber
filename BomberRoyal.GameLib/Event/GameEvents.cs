using BomberRoyal.Core.Features.Board;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using BomberRoyal.Core.GameState;


namespace BomberRoyal.Core.Event
{
    public readonly record struct StartGameEvent();
    public readonly record struct GameOverEvent();
    public readonly record struct StateChangedEvent(IGameState NewState);
    public readonly record struct TilePlacedEvent(Tile Tile); 
    public readonly record struct MazeGeneratedEvent(MazeData Maze);
    public readonly record struct TileDestroyedEvent(Tile Tile);
    public readonly record struct PlayerBlockedByBombEvent(Player Player, Bomb bomb);
    public readonly record struct PlayerDiedEvent(Player Player);
    public readonly record struct EnemyDiedEvent(EnemyBase Enemy);
    public readonly record struct BombPlacedEvent(Bomb Bomb);
    public readonly record struct BombExplodedEvent(Bomb Bomb);
    public readonly record struct ReturnToMenuEvent();
    public readonly record struct ExitGameEvent();
}   
