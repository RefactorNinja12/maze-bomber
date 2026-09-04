using BomberRoyal.Core.Event;
using BomberRoyal.Core.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.GameState
{
    
    public class MainMenuState : IGameState
    {
        private readonly GameManager _gm; 
        public MainMenuState(GameManager gm)
        {
            _gm = gm;
            EventBus.Subscribe(GameEvents.StartGame, OnStartGame);
        }

        private void OnStartGame(object obj)
        {
            _gm.ChangeState(new PlayingState(_gm));
        }

        public void Enter()
        {
            
        }

        public void Exit()
        {
            EventBus.Unsubscribe(GameEvents.StartGame, OnStartGame);
        }

        public void Update(float delta)
        {
            
        }
    }
}
