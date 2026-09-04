using BomberRoyal.Core.Input;
using Microsoft.Xna.Framework;


namespace BomberRoyal.Scene
{
    public class SceneManager
    {
        private static SceneManager? _instance;
        public static SceneManager Instance => _instance ??= new SceneManager();
        private IScene _currentScene; 

        public void SetScene(IScene scene)
        {
            _currentScene = scene;
            InputManager.Instance.Reset();
            _currentScene.LoadContent();
        }
        public void Update(GameTime gameTime)
        {
            _currentScene?.Update(gameTime);
        }
        public void Draw(GameTime gameTime)
        {
            _currentScene?.Draw(gameTime);
        }
    }
}
