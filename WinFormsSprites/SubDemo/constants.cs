using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SubDemo
{
    public class constants
    {
        public const int WaterLevel = 80;
        public const int GroundLevel = 470;
        public const int DistanceFromSide = 20;
        public const int DistanceFromTop = 20;
        public const int DistanceFromBot = 20;

        public static int StartingItemCount = 3;

        public static Point StartingPoint { get { return new Point(250, 250); } }
        public static Size TorpedoSize {  get { return new Size(10, 10); } }
        public static Size DepthChargeSize { get { return new Size(12, 12); } }
        public static Size WhaleSize { get { return new Size(62, 25); } }
        public static Size CargoSize { get { return new Size(70, 30); } }
        public static Size EnemySubSize { get { return new Size(50, 20); } }
        public static Size DestroyerSize { get { return new Size(60, 20); } }
        public static Size DepthChargeExplosionSize { get { return new Size(40, 40); } }
        public static Size BackgroundSize { get { return new Size(500, 500); } }

        public static int TorpedoSpeed = 7;
        public static int DepthChargeSpeed = 3;
        public static int TimeForPlayerToReloadTorpedos = 500; //Time in MS to reload
        public static int TimeForBadGuysToReloadTorpedos = 3000; //Time in MS to reload
        public static int TimeForBadGuysToReloadDepthCharges = 2000; //Time in MS to reload
        public static int TimeBetweenHeals = 1000; //Time to repair one health

        public static int TimeBetweenReplenishBadguys = 500; //Time in MS between adding bad guys

        public static int PlayerSpeed = 4;

        public static int WhaleSpeed = 2;
        public static int DestroyerSpeed = 3;
        public static int CargoSpeed = 2;
        public static int BadSubSpeed = 3;

        public static int TorpedoDamageToPlayer = 50; //When we get shot by a torpedo
        public static int WhaleDamageToPlayer = 30; //When whale hits sub
        public static int SubDamageToPlayer = 70;  //When two subs collide
        public static int DepthChargeDamageToPlayer = 50;  //When hit with depth charge
    }
}
