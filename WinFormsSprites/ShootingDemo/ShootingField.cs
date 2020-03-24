using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using SpriteLibrary;


namespace ShootingDemo
{
    /// <summary>
    /// The names of the different sprites I use.  I do this just so I spell the names identically every time.
    /// </summary>
    public enum SpriteNames { shot, spaceship, explosion, jelly, dragon, walker, flier }
    public enum MyDir {  left, right, stopped }

    public partial class ShootingField : Form
    {
        //The SpriteController is the heart of the system.  We do not instantiate it here; we need
        //to pass it a picturebox before we can make it.  and we do not have one of those until after
        //"InitializeComponents" has run
        SpriteController MySpriteController;
        Sprite OneSprite;
        Sprite Spaceship;
        MyDir LastDirection = MyDir.stopped;
        
        Point SpaceshipPoint = new Point(100, 150); //Where the spaceship first appears
        DateTime LastShot = DateTime.Now; //Used in giving a delay between shots
        DateTime LastMovement = DateTime.Now; //Used to give a slight delay in checking for keypress.

        Random myRandomGen = new Random();
        
        //Our test for winning the game happens many times a second.  When we win, we need to
        //have something that allows us to exit early.  Without this we have many popups telling us that
        //we have won.
        bool alreadywon = false;  


        public ShootingField()
        {
            InitializeComponent();

            //Put the background on the picturebox.  We could do this here, in the design mode, or
            //pass the image into the spritecontroller at a later time.
            MainDrawingArea.BackgroundImage = Properties.Resources.Background;
            //Right now the spritecontroller is mainly set up to use a backgroundlayout of stretch.
            //Again, I am doing this here just to show that it is a property that ought to be set.
            MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;

            //The MainDrawingArea is my PictureBox, which I created in the DesignMode.
            MySpriteController = new SpriteController(MainDrawingArea);

            //The SpriteController has a timer that fires off many times a second.  We
            //Want to check for keypresses and do something when a key is pressed.  This is the
            //easiest location to do that using the spritecontroller
            MySpriteController.DoTick += CheckForKeyPress;

            //*********************************//
            //** Make In The Sprite Animations // 
            //The shot does not get used here.  We duplicate it when we need a "shot"
            //We create an animation by taking a number of "frames" from a single image.  Look at the images
            //in the ShootingDemo properties to see the images we use.
            OneSprite = new Sprite(new Point(0, 0), MySpriteController, Properties.Resources.shot, 50, 50, 200, 4);
            OneSprite.SetSize(new Size(30, 30));
            OneSprite.SetName(SpriteNames.shot.ToString());
            OneSprite.SpriteHitsPictureBox += ShotHitsEdge;

            //read in the explosion shot
            OneSprite = new Sprite(MySpriteController, Properties.Resources.explode, 50, 50, 50);
            OneSprite.SetSize(new Size(30, 30));
            OneSprite.SetName(SpriteNames.explosion.ToString());
            //The function to run when the explosion animation completes
            OneSprite.SpriteAnimationComplete += ExplosionCompletes;

            //Read in the Jelly Monster Sprite
            Sprite JellyMonster = new Sprite(new Point(0, 100), MySpriteController, Properties.Resources.monsters, 100, 100, 200, 4);
            JellyMonster.SetSize(new Size(30, 30));  //this is small
            //JellyMonster.CannotMoveOutsideBox = true;
            JellyMonster.SetName(SpriteNames.jelly.ToString());
            JellyMonster.SpriteHitsSprite += MonsterHitBySprite;

            //Read in the dragon monster sprite
            Sprite DragonMonster = new Sprite(new Point(0, 300), MySpriteController, Properties.Resources.monsters, 100, 100, 220, 4);
            DragonMonster.SetSize(new Size(75, 75));
            DragonMonster.SetName(SpriteNames.dragon.ToString());
            DragonMonster.SpriteHitsSprite += MonsterHitBySprite;

            //Read in the walking monster sprite
            Sprite WalkerMonster = new Sprite(new Point(0, 000), MySpriteController, Properties.Resources.monsters, 100, 100, 300, 4);
            WalkerMonster.SetName(SpriteNames.walker.ToString());
            WalkerMonster.SpriteHitsSprite += MonsterHitBySprite;

            //Read in the flying monster sprite
            Sprite FlyerMonster = new Sprite(new Point(0, 200), MySpriteController, Properties.Resources.monsters, 100, 100, 300, 4);
            FlyerMonster.SetName(SpriteNames.flier.ToString());
            FlyerMonster.SpriteHitsSprite += MonsterHitBySprite;

            //Make the spaceship
            Spaceship = new Sprite(new Point(0, 0), MySpriteController, Properties.Resources.Spaceship, 200, 200, 1000, 1);
            Spaceship.SetSize(new Size(100, 100));
            //The spaceship has two other animations.  Read in the animation with fire coming from it (up)
            Spaceship.AddAnimation(new Point(200, 0), Properties.Resources.Spaceship, 200, 200, 300, 3);
            //read in the animation where we blow up.  (down)
            Spaceship.AddAnimation(new Point(0, 200), Properties.Resources.Spaceship, 200, 200, 150, 4);
            Spaceship.PutPictureBoxLocation(SpaceshipPoint);
            //Make it so the spaceship does not exit the box
            Spaceship.CannotMoveOutsideBox = true;

            ////
            //For now, add a jellymonster
            AddMonster(SpriteNames.jelly);
            AddMonster(SpriteNames.jelly);
            AddMonster(SpriteNames.jelly);
            AddMonster(SpriteNames.walker);
            AddMonster(SpriteNames.walker);
            AddMonster(SpriteNames.dragon);
            AddMonster(SpriteNames.dragon);
            AddMonster(SpriteNames.flier);
            AddMonster(SpriteNames.flier);
            AddMonster(SpriteNames.flier);
        }

        /// <summary>
        /// This adds a monster of the given name.  The Y position on the screen is determined by the
        /// name of the monster.  We choose a random X value
        /// </summary>
        /// <param name="What"></param>
        public void AddMonster(SpriteNames What)
        {
            if (What == SpriteNames.spaceship) return; //We cannot add space-ships this way
            int starty = 50;
            int startx = 50;
            int speed = myRandomGen.Next(2);
            int direction = 0;
            MonsterPayload NewPayload = null;
            switch (What)
            {
                case SpriteNames.dragon:
                    starty = 200;
                    speed += 10;
                    NewPayload = new MonsterPayload();
                    NewPayload.Health = 3;
                    break;
                case SpriteNames.flier:
                    starty = 100;
                    speed += 15;
                    break;
                case SpriteNames.jelly:
                    starty = 50;
                    speed += 10;
                    break;
                case SpriteNames.walker:
                    starty = 300;
                    speed += 14;
                    break;
                default:
                    return;
            }
            startx = myRandomGen.Next(600) + 50;
            if (myRandomGen.Next(2) == 0) direction = 180;
            Sprite NewSprite = MySpriteController.DuplicateSprite(What.ToString());
            NewSprite.AutomaticallyMoves = true;
            NewSprite.CannotMoveOutsideBox = true;
            NewSprite.SpriteHitsPictureBox += SpriteBounces;
            NewSprite.payload = NewPayload; //half the time, this is null.  For dragons, they have a payload with a health of 3
            NewSprite.SetSpriteDirectionDegrees(direction);
            NewSprite.PutBaseImageLocation(new Point(startx, starty));
            NewSprite.MovementSpeed = speed;
        }


        /// <summary>
        /// This is the event that fires off when the sprite hits a wall.  The sprite notices that it has
        /// hit the wall, and the function below is called.  The "Sender" is the sprite that bounced.
        /// </summary>
        /// <param name="sender">The sprite that hit the wall</param>
        /// <param name="e"></param>
        public void SpriteBounces(object sender, EventArgs e)
        {
            Sprite me = (Sprite)sender;
            int degrees = (int)me.GetSpriteDegrees();
            if (Math.Abs(degrees) > 120)
            {
                me.SetSpriteDirectionDegrees(0);//go right
            }
            else
            {
                me.SetSpriteDirectionDegrees(180); //go back left
            }
        }

        //When the window resizes, redraw the window
        private void DemoWindow_ResizeEnd(object sender, EventArgs e)
        {
            MainDrawingArea.Invalidate();
        }

        /// <summary>
        /// This is set up as an event on the shot sprite.  When the sprite hits the window, this event is called
        /// (We to this with the line, OneSprite.SpriteHitsPictureBox += ShotHitsEdge
        /// </summary>       
        public void ShotHitsEdge(object sender, EventArgs e)
        {
            if (sender == null) return;
            HaveShotExplode((Sprite)sender);
        }

        public void HaveShotExplode(Sprite shot)
        {
            SoundPlayer newPlayer = new SoundPlayer(Properties.Resources.thud);
            newPlayer.Play();
            Sprite nSprite = MySpriteController.DuplicateSprite(SpriteNames.explosion.ToString());
            nSprite.PutBaseImageLocation(shot.BaseImageLocation);
            nSprite.AnimateOnce(0);
            shot.Destroy();
        }

        /// <summary>
        /// This is what happens when a monster gets hit.  We check to see if it was a "shot" that got us.
        /// If it was a shot, we explode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MonsterHitBySprite(object sender, SpriteEventArgs e)
        {
            Sprite me = (Sprite)sender;
            bool doExplode = true;
            if (e.TargetSprite.SpriteOriginName == SpriteNames.shot.ToString())
            {
                if(me.payload != null && me.payload is MonsterPayload)
                {
                    //we have a payload.
                    MonsterPayload tPayload = (MonsterPayload)me.payload;
                    tPayload.Health--;
                    if (tPayload.Health > 0)
                    {
                        //We do not want to kill the monster this time.  Just have the shot explode
                        doExplode = false;
                        //Have shot explode
                        HaveShotExplode(e.TargetSprite);
                    }

                }
                //we got shot.  DIE!
                if (doExplode)
                {
                    Sprite nSprite = MySpriteController.DuplicateSprite(SpriteNames.explosion.ToString());
                    nSprite.PutBaseImageLocation(me.BaseImageLocation);
                    nSprite.SetSize(me.GetSize);
                    nSprite.AnimateOnce(0);
                    SoundPlayer newPlayer = new SoundPlayer(Properties.Resources.Tboom);
                    newPlayer.Play();
                    me.Destroy();
                    e.TargetSprite.Destroy();
                }
            }
        }

        /// <summary>
        /// This is how we count to see if we have any monsters left.  If we have some, we keep on playing
        /// </summary>
        public void CountMonsters()
        {
            if (alreadywon) return;
            int Many = 0;
            Many += MySpriteController.CountSpritesBasedOff(SpriteNames.dragon.ToString());
            Many += MySpriteController.CountSpritesBasedOff(SpriteNames.flier.ToString());
            Many += MySpriteController.CountSpritesBasedOff(SpriteNames.jelly.ToString());
            Many += MySpriteController.CountSpritesBasedOff(SpriteNames.walker.ToString());
            if (Many == 0)
            {
                alreadywon = true;
                Spaceship.MovementSpeed = 0;//stop the spaceship
                MessageBox.Show("You have won!");
                Close();
            }
        }

        /// <summary>
        /// This is an event that happens when an explosion finishes.  Basically, get rid of the explosion.
        /// We also use this as an excuse to check to see if any monsters are alive.  If not, we have won
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExplosionCompletes(object sender, EventArgs e)
        {
            Sprite tSprite = (Sprite)sender;
            tSprite.Destroy();
            CountMonsters();
        }

        /// <summary>
        /// Check for keypress is what controls our player movement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForKeyPress(object sender, EventArgs e)
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;
            bool space = false;
            bool didsomething = false;
            TimeSpan duration = DateTime.Now - LastMovement;
            if (duration.TotalMilliseconds < 100)
                return;

            if (alreadywon) return;

            LastMovement = DateTime.Now;
            if (MySpriteController.IsKeyPressed(Keys.A) || MySpriteController.IsKeyPressed(Keys.Left))
            {
                left = true;
            }
            if (MySpriteController.IsKeyPressed(Keys.S) || MySpriteController.IsKeyPressed(Keys.Down))
            {
                down = true;
            }
            if (MySpriteController.IsKeyPressed(Keys.D) || MySpriteController.IsKeyPressed(Keys.Right))
            {
                right = true;
            }
            if (MySpriteController.IsKeyPressed(Keys.W) || MySpriteController.IsKeyPressed(Keys.Up))
            {
                up = true;
            }
            if (MySpriteController.IsKeyPressed(Keys.Space))
            {
                space = true;
            }
            if(MySpriteController.IsKeyPressed(Keys.L))
            {
                MySpriteController.Pause();
            }
            if (MySpriteController.IsKeyPressed(Keys.K))
            {
                MySpriteController.UnPause();
            }

            if (up && down) return; //do nothing if we conflict
            if (left && right) return; //do nothing if we conflict
            if (left)
            {               
                if (LastDirection != MyDir.left)
                {
                    Spaceship.SetSpriteDirectionDegrees(180);
                    //If our animation had actual frames, we want to only change animation once.  Every time we change
                    //the animation, it starts at the first frame again.
                    Spaceship.ChangeAnimation(0);
                    LastDirection = MyDir.left;
                }
                didsomething = true;
                Spaceship.MovementSpeed = 15;
                Spaceship.AutomaticallyMoves = true;
            }
            if (right)
            {                
                if (LastDirection != MyDir.right)
                {
                    Spaceship.SetSpriteDirectionDegrees(0);
                    Spaceship.ChangeAnimation(0);
                    LastDirection = MyDir.right;
                }
                didsomething = true;
                Spaceship.AutomaticallyMoves = true;
                Spaceship.MovementSpeed = 15;
            }
            if (up)
            {
                //Just for kicks, we have an animation with fire that comes out of the bottom
                Spaceship.ChangeAnimation(1);
            }
            if (down)
            {
                Spaceship.AnimateOnce(2);
            }
            //Here is where we fire.
            if (space)
            {
                //Check if we have had enough time since we last shot.  If so, we can shoot again
                TimeSpan Duration = DateTime.Now - LastShot;
                if (Duration.TotalMilliseconds > 300)
                {
                    //We make a new shot sprite.
                    Sprite newsprite = MySpriteController.DuplicateSprite(SpriteNames.shot.ToString());
                    //Checking if newsprite==null is just a safeguard.  It is only null if we use a string that does not exist.
                    if (newsprite != null)
                    {
                        //We figure out where to put the shot
                        Point where = Spaceship.PictureBoxLocation;
                        int halfwit = Spaceship.VisibleWidth / 2;
                        halfwit = halfwit - (newsprite.VisibleWidth / 2);
                        int halfhit = newsprite.VisibleHeight / 2;
                        where = new Point(where.X + halfwit, where.Y - halfhit);
                        newsprite.PutPictureBoxLocation(where);
                        //We tell the sprite to automatically move
                        newsprite.AutomaticallyMoves = true;
                        //We give it a direction, up
                        newsprite.SetSpriteDirectionDegrees(90);
                        //we give it a speed for how fast it moves.
                        newsprite.MovementSpeed = 20;
                    }
                    LastShot = DateTime.Now;
                }
            }
            if(!didsomething)
            {
                LastDirection = MyDir.stopped;
                //No keys pressed.  Stop moving
                Spaceship.MovementSpeed = 0;
            }
        }
    }
}
