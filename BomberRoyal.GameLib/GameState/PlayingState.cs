using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.AbilityFeature;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.CurrencyFeature;
using BomberRoyal.Core.Features.PathfindingFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using BomberRoyal.Core.Input;
using System.Collections.Generic;


namespace BomberRoyal.Core.GameState
{
    public class PlayingState : IGameState
    {
        
        private AbilityManager _abilityManager;
        private AbilityMenu _abilityMenu;
        public GameManager _gm;
        private MazeData _maze; 
        public EnemySpawner _enemySpawner; 
        private Player _player; 
        private PlayerController _playerController;
        public Player Player => _player;
        public BombSystem _bombSystem;
        private CollisionChecker _collisonChecker;
        private EnemyFlowFieldService _flowField;
        private ExplosionSystem _explosionSystem;
        private CurrencySystem _currencySystem;
        public AbilityMenu AbilityMenu => _abilityMenu;
        public CurrencySystem CurrencySystem => _currencySystem;
        private readonly List<EnemyBase> _updateBuffer = new();

        public PlayingState(GameManager gm)
        {
            _gm = gm;
            
        }
        public void Enter()
        {
            const int TileSize = 42;
            _gm.ResetMaze();
            _maze = _gm.Maze;

            
            var enemies = new List<EnemyBase>();
            var bombs = new List<Bomb>();          

            var spawnPos = SpawnHelper.FindValidSpawn(_maze, TileSize);

            _player = new Player(spawnPos, _gm.EventBus); 
            
            _collisonChecker = new CollisionChecker(_maze, bombs, enemies, _player);
            _flowField = new EnemyFlowFieldService(_maze, bombs, _player);
            _bombSystem = new BombSystem(_gm.EventBus, _collisonChecker, bombs);
            _player.AssignBombSystem(_bombSystem);
            _currencySystem = new CurrencySystem(_player);
            _enemySpawner = new EnemySpawner(_maze, _player, _collisonChecker, enemies, _gm.EventBus, _flowField);
            _enemySpawner.StartNewWave();

            enemies.AddRange(_enemySpawner._sharedEnemies);

            _abilityManager = new AbilityManager(_player, _currencySystem, _maze, _gm.EventBus, _collisonChecker);
            _abilityMenu = new AbilityMenu(_abilityManager);
            _playerController = new PlayerController(_player, InputManager.Instance, _collisonChecker, _abilityManager);
            _explosionSystem = new ExplosionSystem(_maze, _collisonChecker, _gm.EventBus);

            _gm.EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            _gm.EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            _currencySystem.SpawnPickup(e.Enemy.Position, 5);
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            _gm.ChangeState(new GameOverState(_gm));
            _gm.EventBus.Publish(new GameOverEvent());
        }

        public void Exit()
        {
            _gm.EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
            _gm.EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
        }

        public void Update(float delta)
        {
            var input = InputManager.Instance;

            if (input.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.Tab))
                _abilityMenu.Toggle();

            _abilityMenu.Update(delta);
            if (_abilityMenu.IsPaused) return;

            _playerController.Update(delta);
            _player.Update(delta);

            _collisonChecker.Update(delta);
            _flowField.Update(delta);

            _bombSystem.Update(delta);
            _enemySpawner.Update(delta);
            _currencySystem.Update();

            _updateBuffer.Clear();
            _updateBuffer.AddRange(_enemySpawner._sharedEnemies);
            foreach (var enemy in _updateBuffer)
                enemy.Update(delta);

            if (!_player.IsAlive)
                _gm.ChangeState(new GameOverState(_gm));
        }
    }
}
