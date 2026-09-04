using BomberRoyal.Core.Event;
using BomberRoyal.Core.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.GameState
{
    public class GameOverState : IGameState
    {
        private readonly GameManager _gm;
        public GameOverState(GameManager gm)
        {
            _gm = gm;
        }

        public void Enter()
        {
            Console.WriteLine("GameOverState entered – subscribed to ReturnToMenu");
            EventBus.Subscribe(GameEvents.ReturnToMenu, OnReturnToMenu);
            EventBus.Subscribe(GameEvents.StartGame, OnRestartGame);
        }

        private void OnRestartGame(object obj)
        {
            _gm.ChangeState(new PlayingState(_gm));
        }

        private void OnReturnToMenu(object obj)
        {
            Console.WriteLine("ReturnToMenu event received!");
            _gm.ChangeState(new MainMenuState(_gm));
        }

        public void Exit()
        {
            EventBus.Unsubscribe(GameEvents.ReturnToMenu, OnReturnToMenu);
            EventBus.Unsubscribe(GameEvents.StartGame, OnRestartGame);
        }

        public void Update(float delta)
        {
            
        }
    }
}
