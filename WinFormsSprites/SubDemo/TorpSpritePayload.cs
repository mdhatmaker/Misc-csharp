using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteLibrary;

namespace SubDemo
{
    public class TorpSpritePayload : SpritePayload
    {
        public bool isGood = false;
        public Direction lastdirection = Direction.none;
        public int WorthDestroyed;
        public int WorthGetAway;
        public DateTime LastTimeOnScreen = DateTime.UtcNow;
        public DateTime LastTorpedoTime = DateTime.UtcNow;
        public DateTime LastDepthChargeTime = DateTime.UtcNow;
        public int DepthChargeDepth = constants.GroundLevel;
    }
}
