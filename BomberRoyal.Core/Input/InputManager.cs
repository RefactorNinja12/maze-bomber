using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BomberRoyal.Core.Input
{
    public class InputManager
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance ??= new InputManager();

        private KeyboardState _currentState; 

    }
}
