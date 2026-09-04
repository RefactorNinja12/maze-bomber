using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.EnemyFeature;
using BomberRoyal.Core.Features.PathfindingFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using BomberRoyal.Core.Shared;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class EnemySpawner
{
    private readonly IEventBus _eventBus;
    private readonly MazeData _maze;
    private readonly Player _player;
    private readonly CollisionChecker _collisionChecker;
    private readonly EnemyFlowFieldService _flowField;
    public List<EnemyBase> _sharedEnemies;

    private readonly ObjectPool<BasicEnemy> _basicPool;
    private readonly ObjectPool<ExplodingEnemy> _exploderPool;
    private readonly Random _rand = new();

    private float _spawnTimer = 0f;
    private float _waveDelay = 12f;      
    private float _enemySpawnDelay = 0.8f; 
    private float _enemySpawnTimer = 0f; 
    private int _enemiesToSpawn = 0;     
    private int _wave = 1;

    private const int MaxEnemies = 50;
    private const float _minDistanceToPlayer = 10f;

    public EnemySpawner(MazeData maze, Player player, CollisionChecker collisionChecker, List<EnemyBase> sharedEnemies, IEventBus eventBus, EnemyFlowFieldService flowField)
    {
        _eventBus = eventBus;
        _maze = maze;
        _player = player;
        _collisionChecker = collisionChecker;
        _sharedEnemies = sharedEnemies;
        _flowField = flowField;

        _eventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);

        _basicPool = new ObjectPool<BasicEnemy>(
            () => new BasicEnemy(Vector2.Zero, _collisionChecker, _player, _eventBus, _flowField),
            InitialCapacity: 10
        );

        _exploderPool = new ObjectPool<ExplodingEnemy>(
            () => new ExplodingEnemy(Vector2.Zero, _collisionChecker, _player, _eventBus, _flowField),
            InitialCapacity: 5
        );
    }

    public void Update(float delta)
    {
       
        _spawnTimer += delta;

       
        if (_enemiesToSpawn > 0)
        {
            _enemySpawnTimer += delta;
            if (_enemySpawnTimer >= _enemySpawnDelay)
            {
                _enemySpawnTimer = 0f;
                SpawnEnemy(GetNextEnemyFromPool());
                _enemiesToSpawn--;
            }
            return;
        }

    
        if (_spawnTimer >= _waveDelay)
        {
            _spawnTimer = 0f;
            StartNewWave();
        }
    }

    public void StartNewWave()
    {
        if (_sharedEnemies.Count >= MaxEnemies)
            return;

       
        int totalEnemies = Math.Min(3 + (_wave / 3), 8);

        _enemiesToSpawn = totalEnemies;

        _waveDelay = Math.Min(15f, 10f + _wave * 0.5f);

        _wave++;
    }

    private EnemyBase GetNextEnemyFromPool()
    {
       
        bool spawnExploder = _wave > 3 && _rand.Next(0, 4) == 0;
        return spawnExploder ? _exploderPool.Get() : _basicPool.Get();
    }

    private void SpawnEnemy(EnemyBase enemy)
    {
        var pos = GetRandomWalkablePosition();
        enemy.Reset(pos);
        _sharedEnemies.Add(enemy);
    }

    private Vector2 GetRandomWalkablePosition()
    {
        int attempts = 10;
        int playerTileX = (int)(_player.Position.X / 42);
        int playerTileY = (int)(_player.Position.Y / 42);

        for (int i = 0; i < attempts; i++)
        {
            int x = _rand.Next(1, _maze.Width - 1);
            int y = _rand.Next(1, _maze.Height - 1);
            float distance = Math.Abs(x - playerTileX) + Math.Abs(y - playerTileY);

            var tile = _maze.GetTile(x, y);
            if (tile.IsWalkable() && distance > _minDistanceToPlayer)
                return new Vector2(x * 42 + 21, y * 42 + 21);
        }

       
        for (int y = 1; y < _maze.Height - 1; y++)
        {
            for (int x = 1; x < _maze.Width - 1; x++)
            {
                var tile = _maze.GetTile(x, y);
                if (tile.IsWalkable())
                    return new Vector2(x * 42 + 21, y * 42 + 21);
            }
        }

        return _player.Position;
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        if (_sharedEnemies.Remove(e.Enemy))
        {
            switch (e.Enemy)
            {
                case BasicEnemy basic:
                    _basicPool.Return(basic);
                    break;
                case ExplodingEnemy exploder:
                    _exploderPool.Return(exploder);
                    break;
            }
        }
    }

    public void Cleanup()
    {
        _eventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }
}