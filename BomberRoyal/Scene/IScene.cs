using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Scene
{
    public interface IScene
    {
        void LoadContent();
        void Update(GameTime gametime); 
        void Draw (GameTime gametime);
    }
}
