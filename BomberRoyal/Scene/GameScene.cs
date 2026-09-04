using BomberRoyal.Core;
using BomberRoyal.Core.Features.AbilityFeature;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.CurrencyFeature;
using BomberRoyal.Core.Features.EnemyFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using BomberRoyal.Core.Features.UIFeature;
using BomberRoyal.Core.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BomberRoyal.Scene
{
    // kör alla renderers som rendererar spel object
    public class GameScene : IScene
    {
        private TextureAtlas _slimeAtlas;
        private PlayerRenderer _playerRenderer;
        private MazeRenderer _mazeRenderer;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private readonly GraphicsDevice _graphics;
        private BombRenderer _bombRenderer;
        private ExplosionRenderer _explosionRenderer;
        private PlayingState _playingState;
        private EnemyRenderer _enemyRenderer;
        private CurrencyRenderer _currencyRenderer;
        private AbilityMenuRenderer _abilityMenuRenderer;
        private TextureRegion _borderTextureRegion;
        private Tileset _borderTileset;
        private TextureAtlas _enemyAttackAtlas;
        private TextureAtlas _enemyDeathAtlas;
        private TextureAtlas _playerWalkAtlas;
        private TextureAtlas _playerIdleAtlas;
        private TextureAtlas _playerHurtAtlas;
        private TextureAtlas _playerDeathAtlas;
        private TextureAtlas _bombAtlas;
        private TextureAtlas _explosionAtlas;
        private TextureAtlas _explodingEnemyWalk;
        private TextureAtlas _explodingEnemyAttack;
        private GameUI _ui;

       
        public GameScene(GraphicsDevice graphics)
        {
            _graphics = graphics;

        }
        public void Draw(GameTime gametime)
        {
            _graphics.Clear(Color.Black);
            _spriteBatch.Begin();
            _mazeRenderer.Draw();
            _spriteBatch.End();
            _playerRenderer.Draw(gametime);
            _bombRenderer.Draw(gametime);
            _enemyRenderer.Draw(gametime);
            _explosionRenderer.Draw();


            _currencyRenderer.Draw(_spriteBatch);
            _abilityMenuRenderer.Draw();

            _ui.Draw(gametime);
        }
        public void LoadContent()
        {
            _explodingEnemyAttack = TextureAtlas.FromFile(Game1.GlobalContent, "explodingenemy-attack.xml");
            _explodingEnemyWalk = TextureAtlas.FromFile(Game1.GlobalContent, "explodingenemy-walk.xml");
            _explosionAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "explosion.xml");
            _playerDeathAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "player-death.xml");
            _playerHurtAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "player-hurt.xml");
            _playerIdleAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "player-idle.xml");
            _playerWalkAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "player-walk.xml");
            _enemyDeathAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "basicenemy-death.xml");
            _slimeAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "basicenemy-walk.xml");
            _enemyAttackAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "basicenemy-attack.xml");
            _bombAtlas = TextureAtlas.FromFile(Game1.GlobalContent, "bomb-slime.xml");
            _borderTextureRegion = Game1.Atlas.GetRegion(name: "border_tiles");
            _borderTileset = new Tileset(_borderTextureRegion, tileHeight: 16, tileWidth: 16);
            var font = Game1.GlobalFont;
            _spriteBatch = new SpriteBatch(_graphics);
            _pixel = new Texture2D(_graphics, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _playingState = GameManager.Instance.CurrentState as PlayingState ?? throw new System.Exception("GameScene Need PlayingState");
            var coinTexture = Game1.GlobalContent.Load<Texture2D>("1");
            _currencyRenderer = new CurrencyRenderer(coinTexture, _playingState.CurrencySystem);
            _ui = new GameUI(_graphics, _spriteBatch, Game1.GlobalFont, _pixel,
                 _playingState.Player,
                 _playingState.CurrencySystem,
                 _playingState._bombSystem);
            _mazeRenderer = new MazeRenderer(_borderTileset, _pixel, 32, _spriteBatch, _playingState._gm.EventBus);
            _mazeRenderer.SetMaze(GameManager.Instance.Maze);
            _enemyRenderer = new EnemyRenderer(
            _spriteBatch,
            _slimeAtlas,
            _enemyAttackAtlas,
            _enemyDeathAtlas,
            _playingState._enemySpawner._sharedEnemies,
            _explodingEnemyWalk,
            _explodingEnemyAttack
            );
            _playerRenderer = new PlayerRenderer(_playingState.Player, _playerWalkAtlas, _playerIdleAtlas, _playerHurtAtlas, _playerDeathAtlas, _spriteBatch);
            _bombRenderer = new BombRenderer(_bombAtlas, _spriteBatch, _playingState._gm.EventBus);
            _explosionRenderer = new ExplosionRenderer(_spriteBatch, _explosionAtlas, _playingState._gm.EventBus);
            _abilityMenuRenderer = new AbilityMenuRenderer(
                _spriteBatch,
                _pixel,
                font,
                _playingState.AbilityMenu,
                _playingState.CurrencySystem);
        }
        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _playingState._enemySpawner.Update(delta);
            _explosionRenderer.Update(delta);
        }
    }
}
