using BomberRoyal.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.GameState
{
    public class PlayingState : IGameState
    {
        private readonly GameManager _gm; 
        public PlayingState(GameManager gm)
        {
            _gm = gm;
        }
        public void Enter()
        {
            EventBus.Subscribe(GameEvents.PlayerDied, OnPlayerDied);
        }

        private void OnPlayerDied(object obj)
        {
            _gm.ChangeState(new GameOverState(_gm));
            EventBus.Publish(GameEvents.GameOver);
        }

        public void Exit()
        {
            EventBus.Unsubscribe(GameEvents.PlayerDied, OnPlayerDied);
        }

        public void Update(float delta)
        {
            // Spelloop här. 
            
        }
    }
}
