using Microsoft.Xna.Framework.Input;
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

        private KeyboardState _currentKeyboard;
        private KeyboardState _previousKeyboard; 
        private InputManager() { }

        public void Update()
        {
            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();
        }
        public void Reset()
        {
            _previousKeyboard = _currentKeyboard = Keyboard.GetState(); 
        }
        public bool IsKeyDown(Keys key) => _currentKeyboard.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => _currentKeyboard.IsKeyUp(key);
        public bool isKeyPressed(Keys key) => _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);
        public bool IsKeyRelease(Keys key) => _currentKeyboard.IsKeyUp(key) && _previousKeyboard.IsKeyDown(key);
    }
}
