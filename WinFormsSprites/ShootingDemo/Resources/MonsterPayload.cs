using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingDemo
{
    /// <summary>
    /// This is going to be used for when we make monsters.  Some of them will be given this "payload."
    /// This is additional information that the various sprites hold onto so we can do different things
    /// with them.  In this case, the sprites will absorb damage until the health drops to zero, then they
    /// will explode.  You can make your own class to hold information about the sprite ai, or other bits
    /// of information.
    /// </summary>
    public class MonsterPayload : SpriteLibrary.SpritePayload
    {
        public int Health = 1;
    }
}
