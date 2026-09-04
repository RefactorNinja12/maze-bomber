using BomberRoyal.Core;
using BomberRoyal.Core.Enums;
using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.EnemyFeature;
using BomberRoyal.Core.Features.PathfindingFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

// Abstrakt basklass som representerar gemensam logik för alla fiender.
public abstract class EnemyBase : IEnemy
{
    protected readonly IEventBus _eventBus;
    protected readonly CollisionChecker _collisionChecker;
    protected readonly Player _player;
    protected readonly Pathfinder _pathfinder;

    protected Queue<Point> _path = new();
    protected float _repathTimer = 0f;
    protected const float REPATH_INTERVAL = 2.5f;

    public Vector2 Position { get; set; }
    public Direction Direction { get; protected set; } = Direction.Down;
    public Vector2 LastMoveDirection { get; protected set; } = Vector2.Zero;

    public bool IsDying { get; private set; }
    private float _deathAnimTimer = 0f;

    public bool IsAttacking { get; protected set; }
    protected float _attackAnimTimer = 0f;

    public bool IsAlive { get; protected set; } = true;

    public float Speed { get; protected set; } = 30f;
    public float AttackFlashTimer { get; protected set; } = 0f;
    public float Health { get; protected set; } = 100f;
    public int Damage { get; protected set; } = 25;
    public float AttackCooldown { get; protected set; } = 1.0f;
    protected float _currentCooldown = 0f;

    public int Width { get; set; } = 32;
    public int Height { get; set; } = 32;

    public Rectangle Hitbox =>
        new((int)(Position.X - Width / 2), (int)(Position.Y - Height / 2), Width, Height);

    protected EnemyBase(Vector2 startPos, CollisionChecker collision, Player player, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _collisionChecker = collision;
        _player = player;
        _pathfinder = new Pathfinder(GameManager.Instance.Maze, collision);
        Position = startPos;
        SnapToNearestWalkableTile();
    }
    // Uppdaterar fiendens tillstånd varje frame och håller koll fiendens state 
    public virtual void Update(float deltaTime)
    {
        if (!IsAlive)
            return;

        if (IsDying)
        {
            _deathAnimTimer -= deltaTime;
            if (_deathAnimTimer <= 0f)
            {
                IsAlive = false;
                IsDying = false;
                _eventBus.Publish(new EnemyDiedEvent(this));
            }
            return;
        }

        if (AttackFlashTimer > 0) AttackFlashTimer -= deltaTime;
        if (_currentCooldown > 0) _currentCooldown -= deltaTime;

        if (IsAttacking)
        {
            _attackAnimTimer -= deltaTime;
            if (_attackAnimTimer <= 0f)
                IsAttacking = false;
            else
                return;
        }

      
        _repathTimer -= deltaTime;
        if (_repathTimer <= 0f && !IsAttacking)
        {
            CalculatePathToPlayer();
            _repathTimer = REPATH_INTERVAL;
        }

        MoveAlongPath(deltaTime);

        TryAttack();

        UpdateFacingDirection();
    }
    // Beräknar ny väg till spelaren med A*
    protected void CalculatePathToPlayer()
    {
        if (_player == null)
            return;

        int tileSize = 42;
        Point start = new((int)(Position.X / tileSize), (int)(Position.Y / tileSize));
        Point goal = new((int)(_player.Position.X / tileSize), (int)(_player.Position.Y / tileSize));

        var newPath = _pathfinder.FindPath(start, goal);
        _path = new Queue<Point>(newPath);
    }
    // Flyttar fienden stegvis mot nästa tile i pathen.
    // Hanterar kollisioner mot väggar.
    protected virtual void MoveAlongPath(float deltaTime)
    {
        if (_path.Count == 0) return;

        var nextTile = _path.Peek();
        Vector2 target = new(nextTile.X * 42 + 21, nextTile.Y * 42 + 21);
        Vector2 dir = target - Position;

        if (dir.Length() < 4f)
        {
            _path.Dequeue();
            if (_path.Count == 0)
            {
                OnPathEnd();
                return;
            }
            nextTile = _path.Peek();
            target = new(nextTile.X * 42 + 21, nextTile.Y * 42 + 21);
            dir = target - Position;
        }

        dir.Normalize();
        Vector2 nextPos = Position + dir * Speed * deltaTime;
        Rectangle nextHitbox = new((int)(nextPos.X - Width / 2), (int)(nextPos.Y - Height / 2), Width, Height);

        bool blocked = _collisionChecker.IsBlocked(nextHitbox, this);
        if (!blocked)
        {
            Position = nextPos;
            LastMoveDirection = dir;
        }
        else
        {
            TryAttackWall(dir);
        }
    }
    // Försöker attackera en vägg i rörelseriktningen
    protected void TryAttackWall(Vector2 direction)
    {
        var maze = GameManager.Instance.Maze;
        int tileSize = 42;

        int tx = (int)((Position.X + direction.X * tileSize) / tileSize);
        int ty = (int)((Position.Y + direction.Y * tileSize) / tileSize);

        if (tx < 0 || ty < 0 || tx >= maze.Width || ty >= maze.Height)
            return;

        var tile = maze.GetTile(tx, ty);
        if (tile != null && tile.IsDestructible() && _currentCooldown <= 0)
        {
            
            tile.TakeDamage(Damage);

            _currentCooldown = AttackCooldown;
            IsAttacking = true;
            _attackAnimTimer = 0.4f;
            AttackFlashTimer = 0.4f;
        }
    }
    // Anropas när fienden når slutet av sin path.
    // Basimplementation försöker attackera i spelarens riktning.
    protected virtual void OnPathEnd() => TryAttackWall(_player.Position - Position);

    // Uppdaterar fiendens visuella riktning baserat på senaste rörelse.
    protected void UpdateFacingDirection()
    {
        if (LastMoveDirection == Vector2.Zero) return;

        if (Math.Abs(LastMoveDirection.X) > Math.Abs(LastMoveDirection.Y))
            Direction = LastMoveDirection.X > 0 ? Direction.Right : Direction.Left;
        else
            Direction = LastMoveDirection.Y > 0 ? Direction.Down : Direction.Up;
    }
    // Flyttar fienden till närmaste gångbara tile om startpositionen är blockerad.
    protected void SnapToNearestWalkableTile()
    {
        var maze = GameManager.Instance.Maze;
        int tileSize = 42;
        int startX = (int)(Position.X / tileSize);
        int startY = (int)(Position.Y / tileSize);

        if (maze.IsWalkableTile(startX, startY)) return;

        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2; x <= 2; x++)
            {
                int nx = startX + x;
                int ny = startY + y;
                if (nx >= 0 && ny >= 0 && nx < maze.Width && ny < maze.Height &&
                    maze.IsWalkableTile(nx, ny))
                {
                    Position = new Vector2(nx * tileSize + tileSize / 2, ny * tileSize + tileSize / 2);
                    return;
                }
            }
        }
    }
    public void TakeDamage(float dmg)
    {
        Health -= dmg;
        if (Health <= 0)
            Kill();
    }
    // Initierar dödssekvens med timer för animation.

    public void Kill()
    {
        if (IsDying || !IsAlive) return;
        IsDying = true;
        _deathAnimTimer = 1.5f;
    }
    public virtual void Reset(Vector2 pos)
    {
        Position = pos;
        IsAlive = true;
        IsDying = false;
        IsAttacking = false;
        AttackFlashTimer = 0f;
        Health = 100f;
        _currentCooldown = 0f;
        LastMoveDirection = Vector2.Zero;
        Direction = Direction.Down;
        _path.Clear();
        _repathTimer = 0f;
    }
    protected abstract bool TryAttack();
}