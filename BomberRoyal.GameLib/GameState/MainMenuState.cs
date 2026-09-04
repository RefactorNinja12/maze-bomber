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
            _gm.EventBus.Subscribe<StartGameEvent>(OnStartGame);
        }

        private void OnStartGame(StartGameEvent e)
        {
            _gm.ChangeState(new PlayingState(_gm));
        }

        public void Enter()
        {
            
        }

        public void Exit()
        {
            _gm.EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        }

        public void Update(float delta)
        {
            
        }
    }
}
