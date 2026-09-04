using BomberRoyal.Core.Event;
using BomberRoyal.Core.GameState;
using BomberRoyal.Core.Input;
using Microsoft.Xna.Framework.Input;
using System;

namespace BomberRoyal.Core.GameState
{
    public class GameOverState : IGameState
    {
        private readonly GameManager _gm;
        private readonly bool _canProcessInput = true;
        public GameOverState()
        {
            
        }
        public GameOverState(GameManager gm)
        {
            _gm = gm;
        }

        public void Enter()
        {
            _gm.EventBus.Subscribe<ReturnToMenuEvent>(OnReturnToMenu);
            _gm.EventBus.Subscribe<StartGameEvent>(OnRestartGame);
        }

        private void OnRestartGame(StartGameEvent e)
        {
            _gm.ChangeState(new PlayingState(_gm));
        }

        private void OnReturnToMenu(ReturnToMenuEvent e)
        {
            _gm.ChangeState(new MainMenuState(_gm));
        }

        public void Exit()
        {
            _gm.EventBus.Unsubscribe<ReturnToMenuEvent>(OnReturnToMenu);
            _gm.EventBus.Unsubscribe<StartGameEvent>(OnRestartGame);
        }

        public void Update(float delta)
        {
            if (!_canProcessInput) return;

            var input = InputManager.Instance;

            if (input.isKeyPressed(Keys.R))
                _gm.EventBus.Publish(new StartGameEvent());

            if (input.isKeyPressed(Keys.Escape))
                _gm.EventBus.Publish(new ReturnToMenuEvent());
        }
    }
}