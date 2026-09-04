using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.GameState
{
    public interface IGameState
    {
        void Enter();
        void Update(float delta); 
        void Exit();
    }
}
