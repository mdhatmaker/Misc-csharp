using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpriteLibrary;

//The Sub Demo
//Tim Young - 7/16/16
//Read the README file included for more information about how, why, and stuff.
//Created for the CodeProject, to give another example for the SpriteLibrary system
namespace SubDemo
{
    public enum SpriteName { CargoShip, Destroyer, PlayerSub, EnemySub, Torpedo, DepthCharge, Whale, Explosion }
    public enum Direction { none, up, down, left, right, up_left, up_right, down_left, down_right }
    public enum ChargeDir { down=1, toss_left=2, toss_right=4 }
    public enum SoundItem { splat, big_bang, small_bang, none }

    public partial class SubDemoForm : Form
    {
        SpriteController TheGameController;  //The Sprite controller that draws all our sprites

        //Set initial values.  When each of these time-out, we allow ourselves to check for a keypress,
        //fire a torpedo, etc.  Start with them all as NOW, since the delay is not long at all
        DateTime LastMovement = DateTime.UtcNow;  //The last time we checked for a keypress.
        DateTime LastTorpedoL = DateTime.UtcNow;  //The last time the player shot left
        DateTime LastTorpedoR = DateTime.UtcNow;  //The last time the player shot Right
        DateTime LastTorpedoU = DateTime.UtcNow;  //The last time the player shot Up
        DateTime LastAddItem = DateTime.UtcNow - TimeSpan.FromSeconds(1); //We want to start by adding things
        DateTime LastHealTime = DateTime.UtcNow;

        //Create the player submarine.  It is null until defined after the sprites are loaded
        Sprite PlayerSub = null;   //The player sub.
        Direction LastDirection = Direction.none;  //The direction our player sub was going last

        //We start with constants.StartingItemCount bad guys, but increase that as time goes on.
        int NumberOfBadguys = constants.StartingItemCount;

        //We have some values, which we start with "false", but set to "true" as the game progresses.
        bool BadSubsDoTorpedoes = false;
        bool BadDestroyersDoDepthCharges = false;
        bool BadTorpedosCanBeAimed = false;
        bool BadDestroyersDoTorpedos = false;
        bool BadDestroyersDoMultipleDepthCharges = false;

        //Start with normal values of these things.  They get reset during StartGame.
        int Health = 100;
        int Score = 0;
        bool PlayingGame = false;
        int level = 1;

        DateTime GameStartTime = DateTime.UtcNow;
        
        //Create the SubDemo form. This is the form in which everything happens. 
        public SubDemoForm()
        {
            //Build the main form.  The form was built using Visual Studio, and the InitializeComponents
            //function was created by that.
            InitializeComponent();

            //Remember the previous height / width settings of the window and use them.
            if (Properties.Settings.Default.StartWidth >= MinimumSize.Width)
                Width = Properties.Settings.Default.StartWidth;
            if (Properties.Settings.Default.StartHeight >= MinimumSize.Height)
                Height = Properties.Settings.Default.StartHeight;

            //Set up the background.  We need to do this before making our sprite controller.
            pbOcean.BackgroundImage = new Bitmap(Properties.Resources.Background, constants.BackgroundSize);

            //Make a new sprite controller using the PictureBox
            TheGameController = new SpriteController(pbOcean, DoTick);

            pbOcean.BackgroundImageLayout = ImageLayout.Stretch;

            //Load the sprites from the resources.  Only need to do this once
            LoadSprites();

            //Put the instructions for the game on the image and wait for someone to hit "Enter"
            PrintDirections();
        }

        /// <summary>
        /// Print the specified text, centered, on the specified graphics, at the specified Y position
        /// </summary>
        /// <param name="G">The graphics object</param>
        /// <param name="theText">The text to use</param>
        /// <param name="Y">The Y position on the image</param>
        void PrintOneLine(Graphics G, string theText, int Y)
        {
            //The font to use
            Font stringFont = new Font("Consolas", 15);
            //Figure out how big the text is, X and Y.
            SizeF Big = G.MeasureString(theText, stringFont);
            //Find the place where to put it. Find the center of the image, then subtract half the width of the text
            int X = (pbOcean.BackgroundImage.Width / 2) - (int)(Big.Width / 2);
            //Write it twice.  First, in white (and just a little offset)
            G.DrawString(theText, stringFont, Brushes.White, X-1, Y-1);
            //Then, write it in black, the main text
            G.DrawString(theText, stringFont, Brushes.Black, X, Y);
        }

        /// <summary>
        /// Print the directions for the game on the image background
        /// </summary>
        void PrintDirections()
        {
            //Make a copy of the image so we can write to it
            Image nImage = new Bitmap(Properties.Resources.Background, constants.BackgroundSize);
            
            pbOcean.BackgroundImage = nImage;
            //Print the instructions for the game on the background
            using (Graphics G = Graphics.FromImage(nImage))
            {
                PrintOneLine(G, "SubDemo", 20);
                PrintOneLine(G, "Use the keys, W A S D to move the sub", 100);
                PrintOneLine(G, "Use the keys, < L > to shoot torpedos", 180);
                PrintOneLine(G, "Press ENTER to begin", 260);
            }
            Image nImageCopy = new Bitmap(nImage, constants.BackgroundSize);
            //Replace the background of the image.  We need to do this so remaining badguys do not erase the text
            //The background is what we use to write on top of the sprites when we erase them.
            if(TheGameController != null)
                TheGameController.ReplaceOriginalImage(nImageCopy);
            //Now, replace the foreground of the image.
            pbOcean.BackgroundImage = nImage;
            pbOcean.Invalidate();
        }

        /// <summary>
        /// Read in all the sprites into the game.
        /// </summary>
        void LoadSprites()
        {
            //The cargo ship
            Image oImage = Properties.Resources.CargoShipR;
            Sprite one = new Sprite(TheGameController, oImage, oImage.Width, oImage.Height);
            one.AddAnimation(Properties.Resources.CargoShipL, oImage.Width, oImage.Height);
            one.SetName(SpriteName.CargoShip.ToString());
            one.CheckBeforeMove += BadItemCheckBeforeMove;
            one.SetSize(constants.CargoSize);

            //The destroyer
            oImage = Properties.Resources.DestroyerR;
            one = new Sprite(TheGameController, oImage, oImage.Width, oImage.Height);
            one.AddAnimation(Properties.Resources.DestroyerL, oImage.Width, oImage.Height);
            one.SetName(SpriteName.Destroyer.ToString());
            one.CheckBeforeMove += BadItemCheckBeforeMove;
            one.SetSize(constants.DestroyerSize);

            //The enemy sub
            oImage = Properties.Resources.EnemySubR;
            one = new Sprite(TheGameController, oImage, oImage.Width, oImage.Height);
            one.AddAnimation(Properties.Resources.EnemySubL, oImage.Width, oImage.Height);
            one.SetName(SpriteName.EnemySub.ToString());
            one.CheckBeforeMove += BadItemCheckBeforeMove;
            one.SetSize(constants.EnemySubSize);

            //The player's sub
            oImage = Properties.Resources.SubmarineR;
            one = new Sprite(TheGameController, oImage, oImage.Width, oImage.Height);
            one.AddAnimation(Properties.Resources.SubmarineL, oImage.Width, oImage.Height);
            one.SetName(SpriteName.PlayerSub.ToString());
            one.SetSize(new Size(50, 20));

            //A torpedo.  Lots of directions.
            oImage = Properties.Resources.torpedoR;
            one = new Sprite(TheGameController, oImage, oImage.Width, oImage.Height); //animation 0 = right
            one.AddAnimation(Properties.Resources.torpedoL, oImage.Width, oImage.Height);  //animation 1 = left
            one.AddAnimation(Properties.Resources.torpedoUP, oImage.Width, oImage.Height); //animation 2 = up
            one.AddAnimation(Properties.Resources.torpedoDN, oImage.Width, oImage.Height); //animation 3 = down
            one.SetSize(constants.TorpedoSize);
            one.SetName(SpriteName.Torpedo.ToString());
            one.CheckBeforeMove += TorpedoCheckBeforeMovement;

            //Whales
            oImage = Properties.Resources.whaleR;
            one = new Sprite(new Point(0,0), TheGameController, oImage, 250, 100, 300,4);
            one.AddAnimation(new Point(0, 0), Properties.Resources.whaleL, 250, 100, 300, 4);
            one.SetName(SpriteName.Whale.ToString());
            one.SetSize(constants.WhaleSize);
            one.CheckBeforeMove += BadItemCheckBeforeMove;

            //Depth Charge
            oImage = Properties.Resources.depthcharge;
            one = new Sprite(TheGameController, oImage, oImage.Width, oImage.Height);
            one.SetName(SpriteName.DepthCharge.ToString());
            one.SetSize(constants.DepthChargeSize);
            one.CheckBeforeMove += DepthChargeCheckBeforeMovement;

            //read in the explosion shot
            one = new Sprite(TheGameController, Properties.Resources.explode, 50, 50, 50);
            one.SetSize(new Size(30, 30));
            one.SetName(SpriteName.Explosion.ToString());
            //The function to run when the explosion animation completes
            one.SpriteAnimationComplete += ExplosionCompletes;
        }


        //Set up for the start of the game.  We do this at the beginning of each game.
        //Just before this, we had done a PrintDirections() and were waiting for someone
        //to press enter.
        void StartGame()
        {
            Health = 100;
            Score = 0;
            NumberOfBadguys = constants.StartingItemCount;

            //Make a copy of the image so we can write to it
            Image nImage = new Bitmap(Properties.Resources.Background, constants.BackgroundSize);
            TheGameController.ReplaceOriginalImage(nImage);
            pbOcean.BackgroundImage = nImage;
            pbOcean.Invalidate();

            //Get rid of all sprites.  We will add new ones for the game
            foreach (Sprite one in TheGameController.SpritesBasedOffAnything())
            {
                one.Destroy();
            }

            //Pull out the player sprite.
            PlayerSub = TheGameController.DuplicateSprite(SpriteName.PlayerSub.ToString());
            //Add this function to keep the player sub from exiting the box
            PlayerSub.CheckBeforeMove += PlayerCheckBeforeMovement;
            //Add an event for when our sub collides with something
            PlayerSub.SpriteHitsSprite += PlayerSubHitsSomething;
            //Create a payload for the sprite
            TorpSpritePayload PlayerTSP = new TorpSpritePayload();
            PlayerTSP.isGood = true;
            PlayerSub.payload = PlayerTSP;

            //This is a fail-safe check.  If the main sprite does not load, we are in trouble.
            if (PlayerSub != null)
            {
                PlayerSub.PutBaseImageLocation(constants.StartingPoint);
            }
            else
            {
                PlayingGame = false;
                MessageBox.Show("Unable to load the primary sprite.  Closing!");
                Close();
            }

            //Reset things so the second game does no start insanely
            BadSubsDoTorpedoes = false;
            BadDestroyersDoDepthCharges = false;
            BadTorpedosCanBeAimed = false;
            BadDestroyersDoTorpedos = false;
            BadDestroyersDoMultipleDepthCharges = false;

            //Set the game start to be now.  The level is calculated off this value
            GameStartTime = DateTime.UtcNow;

            //Update the text of the form (level, score, health)
            UpdateForm();

            //Set it so we are playing the game.  Now DoTick will continue processing stuff
            PlayingGame = true;
        }

        /// <summary>
        /// DoTick.  This is the main function of the game.  It happens every few milliseconds
        /// read the README file in the resources for a bigger description of how it works
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        void DoTick(object Sender, EventArgs e)
        {
            //If the game is not currently playing (the instructions are visible)
            if (!PlayingGame)
            {
                //All we do is "press Enter to start the game"
                if (TheGameController.IsKeyPressed(Keys.Enter))
                    StartGame();
                //Exit DoTick if we are not currently playing the game.
                return;
            }

            //Check to see if the player has pressed a key, and process it.
            //Do sub movement and firing of player torpedoes
            CheckForKeyPress(Sender, e);

            //If the player sub has been healed, try to heal it if we can.  First, see if enough time has passed
            if ((DateTime.UtcNow - LastHealTime).TotalMilliseconds > constants.TimeBetweenHeals)
            {
                pbOcean.Invalidate();
                //If we are hurt
                if (Health < 100)
                {
                    //If the time has passed, and we are hurt, then heal up one
                    Health++;
                    //Update the form that says health, level, etc.
                    UpdateForm();
                    //Set the time, so we wait a little bit before we heal again.
                    LastHealTime = DateTime.UtcNow;
                }
            }

            //Add bad-guys to the game if we need to do that.
            if ((DateTime.UtcNow - LastAddItem).TotalMilliseconds > constants.TimeBetweenReplenishBadguys)
            {
                LastAddItem = DateTime.UtcNow;

                //BadguyCount counts all the bad guys.
                ItemCount Items = BadguyCount();

                //We could get a count of every individual item, but we only care about the total
                if(Items.Total < NumberOfBadguys)
                {
                    //If we have fewer bad-guys on the screen than we ought, add one
                    int choice = TheGameController.RandomNumberGenerator.Next(10);
                    switch(choice)
                    {
                        case 0:
                        case 1:
                            //Add a whale
                            CreateOneItem(SpriteName.Whale);
                            break;
                        case 2: case 3:
                        case 4: case 5:
                            //Add a Cargo Ship
                            CreateOneItem(SpriteName.CargoShip);
                            break;
                        case 6: case 7:
                            //Add A destroyer
                            CreateOneItem(SpriteName.Destroyer);
                            break;
                        case 8: case 9:
                            //Add an enemy sub
                            CreateOneItem(SpriteName.EnemySub);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// This happens when the player submarine sprite hits something.  It could be a player torpedo, which
        /// gets ignored.  But it could be a whale, an enemy sub, an enemy torpedo, or an enemy depth-charge.
        /// Deal with all those options here.  This function is added to the player sub when we load sprites.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayerSubHitsSomething(object sender, SpriteEventArgs e)
        {
            //If we are not playing the game, then do not do anything.
            if (!PlayingGame) return;
            //If the player sprite is being destroyed already, do not do anything.
            if (e.TargetSprite.Destroying) return;

            //If we run into an enemy sub...  Crash.  Take damage and kill the enemy sub.
            if (e.TargetSprite.SpriteOriginName == SpriteName.EnemySub.ToString())
            {
                Explode(e.TargetSprite);
                PlayerSubTakesDamage(constants.SubDamageToPlayer);
            }
            if (e.TargetSprite.SpriteOriginName == SpriteName.Whale.ToString())
            {
                Explode(e.TargetSprite);
                PlayerSubTakesDamage(constants.WhaleDamageToPlayer);
            }
            if (e.TargetSprite.SpriteOriginName == SpriteName.DepthCharge.ToString())
            {
                Explode(e.TargetSprite);
                PlayerSubTakesDamage(constants.DepthChargeDamageToPlayer);
            }
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
            bool shootl = false;
            bool shootr = false;
            bool shootu = false;
            bool didsomething = false;
            TimeSpan duration = DateTime.UtcNow - LastMovement;
            if (duration.TotalMilliseconds < 200)
                return;

            //pbOcean.Invalidate();

            UpdateLevel(); //Change the game level if needed

            LastMovement = DateTime.Now;
            if (TheGameController.IsKeyPressed(Keys.A) || TheGameController.IsKeyPressed(Keys.Left))
            {
                left = true;
            }
            if (TheGameController.IsKeyPressed(Keys.S) || TheGameController.IsKeyPressed(Keys.Down))
            {
                down = true;
            }
            if (TheGameController.IsKeyPressed(Keys.D) || TheGameController.IsKeyPressed(Keys.Right))
            {
                right = true;
            }
            if (TheGameController.IsKeyPressed(Keys.W) || TheGameController.IsKeyPressed(Keys.Up))
            {
                up = true;
            }
            if (TheGameController.IsKeyPressed(Keys.OemPeriod))
            {
                //The > key
                shootr = true;
            }
            if (TheGameController.IsKeyPressed(Keys.L))
            {
                //The L key
                shootu = true;
            }
            if (TheGameController.IsKeyPressed(Keys.Oemcomma))
            {
                //The > key
                shootl = true;
            }

            if (down && up) return;
            if (left && right) return;


            //We want to also check for torpedo fire commands, so we do not exit yet
            if(!didsomething && left && up)
            {
                // if we are already giong one direction, do not tell it to go the same direction
                if (LastDirection != Direction.up_left)
                {
                    LastDirection = Direction.up_left;
                    PlayerSub.ChangeAnimation(1); //left
                    PlayerSub.SetSpriteDirectionDegrees(135);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && left && down)
            {
                if (LastDirection != Direction.down_left)
                {
                    LastDirection = Direction.down_left;
                    PlayerSub.ChangeAnimation(1); //left
                    PlayerSub.SetSpriteDirectionDegrees(225);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && left)
            {
                if (LastDirection != Direction.left)
                {
                    LastDirection = Direction.left;
                    PlayerSub.ChangeAnimation(1); //left
                    PlayerSub.SetSpriteDirectionDegrees(180);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && right && up)
            {
                if (LastDirection != Direction.up_right)
                {
                    LastDirection = Direction.up_right;
                    PlayerSub.ChangeAnimation(0); //right
                    PlayerSub.SetSpriteDirectionDegrees(45);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && right && down)
            {
                if (LastDirection != Direction.down_right)
                {
                    LastDirection = Direction.down_right;
                    PlayerSub.ChangeAnimation(0); //right
                    PlayerSub.SetSpriteDirectionDegrees(315);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && right)
            {
                if (LastDirection != Direction.right)
                {
                    LastDirection = Direction.right;
                    PlayerSub.ChangeAnimation(0); //right
                    PlayerSub.SetSpriteDirectionDegrees(0);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && up)
            {
                if (LastDirection != Direction.up)
                {
                    LastDirection = Direction.up;
                    PlayerSub.SetSpriteDirectionDegrees(90);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            if (!didsomething && down)
            {
                if (LastDirection != Direction.down)
                {
                    LastDirection = Direction.down;
                    PlayerSub.SetSpriteDirectionDegrees(270);
                    PlayerSub.AutomaticallyMoves = true;
                    PlayerSub.MovementSpeed = constants.PlayerSpeed;
                }
                didsomething = true;
            }
            //Check for torpedo commands
            if(shootl)
            {
                if ((DateTime.UtcNow - LastTorpedoL).TotalMilliseconds > constants.TimeForPlayerToReloadTorpedos)
                {
                    LastTorpedoL = DateTime.UtcNow;
                    ShootTorpedo(PlayerSub, Direction.left);
                }
            }
            if (shootr)
            {
                if ((DateTime.UtcNow - LastTorpedoR).TotalMilliseconds > constants.TimeForPlayerToReloadTorpedos)
                {
                    LastTorpedoR = DateTime.UtcNow;
                    ShootTorpedo(PlayerSub, Direction.right);
                }
            }
            if (shootu)
            {
                if ((DateTime.UtcNow - LastTorpedoU).TotalMilliseconds > constants.TimeForPlayerToReloadTorpedos)
                {
                    LastTorpedoU = DateTime.UtcNow;
                    ShootTorpedo(PlayerSub, Direction.up);
                }
            }
            // if we have not been told to move, we stop
            if (!didsomething)
            {
                PlayerSub.MovementSpeed = 0;
                LastDirection = Direction.none;
            }
        }

        //update the text on the form
        void UpdateForm()
        {
            Text = "SubDemo - Level: " + level + "  Score: " + Score + "  Health: " + Health;
        }

        //create a depth charge for the destroyer.  DropDepthCharge below sends it on its way
        Sprite CreateDepthCharge(Sprite DestroyerSprite)
        {
            Sprite DepthCharge = TheGameController.DuplicateSprite(SpriteName.DepthCharge.ToString());
            TorpSpritePayload TSP = new TorpSpritePayload();
            DepthCharge.payload = TSP;
            TSP.isGood = false;
            if (TheGameController.RandomNumberGenerator.Next(3) == 0)
                TSP.DepthChargeDepth = PlayerSub.BaseImageLocation.Y;  //explode where the player is
            else
                TSP.DepthChargeDepth = TheGameController.RandomNumberGenerator.Next(pbOcean.BackgroundImage.Height) + constants.WaterLevel;
            return DepthCharge;
        }

        /// <summary>
        /// Tell the destroyer to drop one to three depth charges
        /// </summary>
        /// <param name="DestroyerSprite"></param>
        /// <param name="Where">An enum that may contain one or more direcions combined together with OR</param>
        void DropDepthCharge(Sprite DestroyerSprite, ChargeDir Where)
        {
            //  the depth charge (we use up to three of them)
            Sprite Charge = null;
            //Wehere the charge starts (three possible locations)
            Point StartPoint;
            //A list of points, for the two depth charges that get tossed left or right
            List<Point> travel = new List<Point>();
            //We use the & here.  The direction may contain multiple directions inside it.
            //The enum is defined as down=1, toss_left=2, toss_right=4
            //In binary, we can add them together (using the OR (|), which looks like: ChargeDir.up | ChargeDir.toss_left)
            //Which comes to 011.  Left, right, and down would look like 111.  Down, by itself would be 001.
            //Read up on binary AND / Binary OR comparisons to understand this better.
            if ((Where & ChargeDir.down) == ChargeDir.down)
            {
                //Get one depth charge
                Charge = CreateDepthCharge(DestroyerSprite);
                //The down depth-charge starts in the middle of the destroyer
                StartPoint = new Point(DestroyerSprite.BaseImageLocation.X + (DestroyerSprite.GetSize.Width / 2) 
                    - (Charge.GetSize.Width / 2),
                    DestroyerSprite.BaseImageLocation.Y + (DestroyerSprite.GetSize.Height / 2));
                Charge.PutBaseImageLocation(StartPoint);
                //We tell it to drop straight down
                Charge.SetSpriteDirectionDegrees(270);
                Charge.AutomaticallyMoves = true;
                Charge.MovementSpeed = constants.DepthChargeSpeed;
            }
            //If we tell it to go left
            if ((Where & ChargeDir.toss_left) == ChargeDir.toss_left)
            {
                //Get a new depth-charge
                Charge = CreateDepthCharge(DestroyerSprite);
                //Tell it to start on the left side of the destroyer
                StartPoint = new Point(DestroyerSprite.BaseImageLocation.X + (DestroyerSprite.GetSize.Width / 2)
                    - Charge.GetSize.Width,
                    DestroyerSprite.BaseImageLocation.Y + (DestroyerSprite.GetSize.Height / 2));
                Charge.PutBaseImageLocation(StartPoint);
                //Make a series of points, which we will pass to a sprite move-to function.
                //The sprite will move to each point, one after the other.  We do this to
                //make it look like the charge is being tossed to the front and back of the destroyer
                travel.Clear();
                travel.Add(DeltaPoint(StartPoint, -5, -15));
                travel.Add(DeltaPoint(StartPoint, -10, -7));
                travel.Add(DeltaPoint(StartPoint, -15, 0));
                travel.Add(DeltaPoint(StartPoint, -20, 5));
                travel.Add(DeltaPoint(StartPoint, -20, pbOcean.BackgroundImage.Height));
                //Tell the sprite to move-to each of the points
                Charge.MoveTo(travel);
                Charge.AutomaticallyMoves = true;
                Charge.MovementSpeed = constants.DepthChargeSpeed;
            }
            if ((Where & ChargeDir.toss_right) == ChargeDir.toss_right)
            {
                //Get a new depth-charge
                Charge = CreateDepthCharge(DestroyerSprite);
                //Tell it to start on the right side of the destroyer
                StartPoint = new Point(DestroyerSprite.BaseImageLocation.X + (DestroyerSprite.GetSize.Width / 2)
                    + Charge.GetSize.Width,
                    DestroyerSprite.BaseImageLocation.Y + (DestroyerSprite.GetSize.Height / 2));
                Charge.PutBaseImageLocation(StartPoint);
                //Make a series of points, which we will pass to a sprite move-to function.
                //The sprite will move to each point, one after the other.  We do this to
                //make it look like the charge is being tossed to the front and back of the destroyer
                travel.Clear();
                travel.Add(DeltaPoint(StartPoint, 5, -15));
                travel.Add(DeltaPoint(StartPoint, 10, -7));
                travel.Add(DeltaPoint(StartPoint, 15, 0));
                travel.Add(DeltaPoint(StartPoint, 20, 5));
                travel.Add(DeltaPoint(StartPoint, 20, pbOcean.BackgroundImage.Height));
                //Tell the sprite to move-to each of the points
                Charge.MoveTo(travel);
                Charge.AutomaticallyMoves = true;
                Charge.MovementSpeed = constants.DepthChargeSpeed;
            }

        }

        /// <summary>
        /// Return a point, based off the original point, but adjusted by X and Y
        /// </summary>
        /// <param name="start"></param>
        /// <param name="changex"></param>
        /// <param name="changey"></param>
        /// <returns></returns>
        Point DeltaPoint(Point start, int changex, int changey)
        {
            return new Point(start.X + changex, start.Y + changey);
        }

        //Have the player sub, or enemy sub, shoot a torpedo
        Sprite ShootTorpedo(Sprite WhoShoots, Direction Where)
        {
            if (!(WhoShoots.payload is TorpSpritePayload)) return null; //we cannot shoot
            Sprite Torpedo = null;
            TorpSpritePayload ShooterTSP = (TorpSpritePayload)WhoShoots.payload;
            Point SubPoint = new Point(0,0);
            Point TorpPoint = new Point(0,0);
            int xdelta;
            int Xpos;
            TorpSpritePayload TorpPayload;

            //The direction the torpedo is sent (specify a starting location, torpedo image, and direction)
            switch (Where)
            {
                case Direction.left:
                    //Make a torpedo
                    Torpedo = TheGameController.DuplicateSprite(SpriteName.Torpedo.ToString());
                    //Figure out where to send it
                    SubPoint = WhoShoots.BaseImageLocation;
                    xdelta = WhoShoots.GetSize.Width - 2 * (WhoShoots.GetSize.Width / 5);
                    Xpos = SubPoint.X + (int)(((double)WhoShoots.GetSize.Width / 2) - ((double)Torpedo.GetSize.Width / 2));
                    TorpPoint = new Point(SubPoint.X - Torpedo.GetSize.Width, SubPoint.Y + (WhoShoots.GetSize.Height / 2) - (Torpedo.GetSize.Height / 2));
                    //Make a payload so we know something about who sent it (friend, foe, etc)
                    TorpPayload = new TorpSpritePayload();
                    TorpPayload.isGood = ShooterTSP.isGood;
                    TorpPayload.lastdirection = Direction.left;
                    Torpedo.payload = TorpPayload;

                    //Put it where it needs to be put
                    Torpedo.PutBaseImageLocation(TorpPoint);
                    //Send it on its way
                    Torpedo.AutomaticallyMoves = true;
                    Torpedo.ChangeAnimation(1); //Left
                    Torpedo.SetSpriteDirectionDegrees(180); //left
                    Torpedo.MovementSpeed = constants.TorpedoSpeed;
                    break;
                case Direction.right:
                    Torpedo = TheGameController.DuplicateSprite(SpriteName.Torpedo.ToString());
                    SubPoint = WhoShoots.BaseImageLocation;
                    xdelta = WhoShoots.GetSize.Width - 2 * (WhoShoots.GetSize.Width / 5);
                    Xpos = SubPoint.X + (int)(((double)WhoShoots.GetSize.Width / 2) - ((double)Torpedo.GetSize.Width / 2));
                    TorpPoint = new Point(Xpos, SubPoint.Y + (WhoShoots.GetSize.Height / 2) - (Torpedo.GetSize.Height / 2));
                    TorpPoint = new Point(SubPoint.X + Torpedo.GetSize.Width + PlayerSub.GetSize.Width, SubPoint.Y + (PlayerSub.GetSize.Height / 2) - (Torpedo.GetSize.Height / 2));
                    TorpPayload = new TorpSpritePayload();
                    TorpPayload.isGood = ShooterTSP.isGood;
                    Torpedo.ChangeAnimation(0); //Right
                    TorpPayload.lastdirection = Direction.right;
                    Torpedo.payload = TorpPayload;

                    Torpedo.PutBaseImageLocation(TorpPoint);
                    Torpedo.AutomaticallyMoves = true;
                    Torpedo.SetSpriteDirectionDegrees(0); //up
                    Torpedo.MovementSpeed = constants.TorpedoSpeed;
                    break;
                case Direction.up:
                    Torpedo = TheGameController.DuplicateSprite(SpriteName.Torpedo.ToString());
                    SubPoint = WhoShoots.BaseImageLocation;
                    xdelta = WhoShoots.GetSize.Width - 2 * (WhoShoots.GetSize.Width / 5);
                    Xpos = SubPoint.X + (int)(((double)WhoShoots.GetSize.Width / 2) - ((double)Torpedo.GetSize.Width / 2));
                    TorpPoint = new Point(Xpos, SubPoint.Y + (WhoShoots.GetSize.Height / 2) - (Torpedo.GetSize.Height / 2));
                    TorpPayload = new TorpSpritePayload();
                    TorpPayload.isGood = ShooterTSP.isGood;
                    Torpedo.ChangeAnimation(2); //Up
                    TorpPayload.lastdirection = Direction.up;
                    Torpedo.payload = TorpPayload;

                    Torpedo.PutBaseImageLocation(TorpPoint);
                    Torpedo.AutomaticallyMoves = true;
                    Torpedo.SetSpriteDirectionDegrees(90); //up
                    Torpedo.MovementSpeed = constants.TorpedoSpeed;
                    break;
                case Direction.down:
                    Torpedo = TheGameController.DuplicateSprite(SpriteName.Torpedo.ToString());
                    SubPoint = WhoShoots.BaseImageLocation;
                    xdelta = WhoShoots.GetSize.Width - 2 * (WhoShoots.GetSize.Width / 5);
                    Xpos = SubPoint.X + (int)(((double)WhoShoots.GetSize.Width / 2) - ((double)Torpedo.GetSize.Width / 2));
                    TorpPoint = new Point(Xpos, SubPoint.Y + (WhoShoots.GetSize.Height / 2) - (Torpedo.GetSize.Height / 2));
                    TorpPayload = new TorpSpritePayload();
                    TorpPayload.isGood = ShooterTSP.isGood;
                    Torpedo.ChangeAnimation(3); //down
                    TorpPayload.lastdirection = Direction.down;
                    Torpedo.payload = TorpPayload;

                    Torpedo.PutBaseImageLocation(TorpPoint);
                    Torpedo.AutomaticallyMoves = true;
                    Torpedo.SetSpriteDirectionDegrees(270); //down
                    Torpedo.MovementSpeed = constants.TorpedoSpeed;
                    break;
            }
            return Torpedo;
        }

        /// <summary>
        /// Shoot a torpedo at a specific target (the player sub)
        /// </summary>
        /// <param name="WhoShoots"></param>
        /// <param name="Target"></param>
        /// <returns>The torpedo.</returns>
        Sprite ShootTorpedo(Sprite WhoShoots, Sprite Target)
        {
            Direction startdirection = Direction.none;
            //Set the direction of the torpedo (left, right, up, down)
            //Find the greatest distance and use that
            int BigX = WhoShoots.BaseImageLocation.X - Target.BaseImageLocation.X;
            int BigY = WhoShoots.BaseImageLocation.Y - Target.BaseImageLocation.Y;
            //If the x distance is greater than the Y distance, shoot left/right
            if(Math.Abs(BigX) > Math.Abs(BigY))
            {
                //If the distance X is positive, it is left
                if (BigX >= 0) startdirection = Direction.left;
                else startdirection = Direction.right;
            }
            else
            {
                //If the Y distance is greater, it is up/down
                if (BigY >= 0)
                    startdirection = Direction.up;
                else
                    startdirection = Direction.down;
            }

            //Get a torpedo that is shot the general direction (up, down, left, right)
            Sprite torp = ShootTorpedo(WhoShoots, startdirection);
            //Now, create a list of points, where the torpedo will go
            List<Point> targets = new List<Point>();
            //The first point is the center of the player sub (target)
            Point PlayerPoint = new Point(Target.BaseImageLocation.X + Target.GetSize.Width / 2, Target.BaseImageLocation.Y + Target.GetSize.Height / 2);
            targets.Add(PlayerPoint);
            //Then, the torpedo continues in a straight line, off the screen.
            if (startdirection == Direction.left)
                targets.Add(new Point(-10, Target.BaseImageLocation.Y));
            if (startdirection == Direction.right)
                targets.Add(new Point(pbOcean.BackgroundImage.Width + 100, Target.BaseImageLocation.Y));
            if (startdirection == Direction.up)
                targets.Add(new Point(Target.BaseImageLocation.X,-10));
            if (startdirection == Direction.down)
                targets.Add(new Point(Target.BaseImageLocation.X,pbOcean.BackgroundImage.Height + 100));
            //Tell the torpedo to move to these points.  The speed has already been specified when the torpedo was created
            torp.MoveTo(targets);
            //Return the torpedo.  The main reason to do this is because the other ShootTorpedo function returns a torpedo sprite too
            return torp;
        }

        //Play a specific sound.  I have one function to do this, in case I want to add a setting
        //to disable sound, change volume, etc.  If all sound goes through one function, we can
        //Affect the sound in one place.
        void PlaySound(SoundItem What)
        {
            switch (What)
            {
                case SoundItem.big_bang:
                    TheGameController.SoundPlay(Properties.Resources.Tboom, "Boom");
                    break;
                case SoundItem.splat:
                    TheGameController.SoundPlay(Properties.Resources.splat, "Splat");
                    break;
                case SoundItem.small_bang:
                    TheGameController.SoundPlay(Properties.Resources.thud, "Bang");
                    break;
            }
        }

        //If a topedo hits something
        void TorpedoKillsSomething(Sprite torpedo, Sprite Target)
        {
            //If the game is over, do nothing.
            if (!PlayingGame) return;
            //Get rid of the torpedo
            torpedo.Destroy();
            //Get the torpedo payload so we can tell what we hit.
            if(Target.payload is TorpSpritePayload)
            {
                TorpSpritePayload TSP = (TorpSpritePayload)Target.payload;
                if(TSP.isGood == false)
                {
                    //If the target is bad, and it is a whale, we lose points
                    if (Target.SpriteOriginName == SpriteName.Whale.ToString())
                    {
                        Score -= TSP.WorthDestroyed;
                    }
                    else
                    {
                        //If it is not a whale, we get points.
                        Score += TSP.WorthDestroyed;
                    }
                    Explode(Target);
                }
                else
                {
                    //If it is the player sub that was hit
                    if (Target.SpriteOriginName == SpriteName.PlayerSub.ToString())
                    {
                        //Our sub was hit
                        PlayerSubTakesDamage(constants.TorpedoDamageToPlayer);
                        Explode(torpedo);
                    }
                }
            }
            //We had our health, or points, or both modified.  Update the text on the screen.
            UpdateForm();
        }

        //When the player sub takes damage
        void PlayerSubTakesDamage(int HowMuchDamage)
        {
            //How much damage do we take
            Health -= HowMuchDamage;
            //Our health never drops below zero
            if (Health <= 0) Health = 0;
            //Update our health information
            UpdateForm();
            //If we have died...
            if(Health == 0)
            {
                //we explode
                Explode(PlayerSub);
                //We set it so we are no longer playing the game
                PlayingGame = false;
                //We change the background, so the directions are showing
                PrintDirections();
                //We pop up a box saying the game is over.
                MessageBox.Show("Game over.  Your score was: " + Score.ToString() + "\n Level: " + level);
            }
        }

        //Have the depth-charge check if it hits the ground as it falls through the water
        //This function was added when we loaded sprites.
        void DepthChargeCheckBeforeMovement(object Sender, SpriteEventArgs e)
        {
            //If the sender is not a sprite, exit
            if (!(Sender is Sprite)) return;
            Sprite TorpSprite = (Sprite)Sender;
            //If the sender does not have a proper payload, exit
            if (!(TorpSprite.payload is TorpSpritePayload)) return;

            TorpSpritePayload TSP = (TorpSpritePayload)TorpSprite.payload;

            if(e.NewLocation.Y >= TSP.DepthChargeDepth || e.NewLocation.Y >= constants.GroundLevel)
            {
                Explode(TorpSprite); //Explode when we get to the bottom
            }
        }

        /// <summary>
        /// Check to see if a torpedo hits anything
        /// </summary>
        /// <param name="Sender">The torpedo</param>
        /// <param name="e"></param>
        void TorpedoCheckBeforeMovement(object Sender, SpriteEventArgs e)
        {
            //If the sender is not a sprite, exit (just a precaution)
            if (!(Sender is Sprite)) return;
            Sprite TorpSprite = (Sprite)Sender;

            //If the sender sprite does not have a valid payload, exit as a precaution
            if (!(TorpSprite.payload is TorpSpritePayload)) return;
            TorpSpritePayload TSP = (TorpSpritePayload)TorpSprite.payload;

            //Get a list of sprites that we hit.  They might be on the same team as the torpedo is.
            List<Sprite> SpritesWeHit = new List<Sprite>();
            //Add the sprites on both sides of the sprite
            int yPos = TorpSprite.BaseImageLocation.Y + (TorpSprite.GetSize.Height / 2); //Get center-y position
            SpritesWeHit.AddRange(TheGameController.SpritesAtImagePoint(new Point(TorpSprite.BaseImageLocation.X, yPos)));
            SpritesWeHit.AddRange(TheGameController.SpritesAtImagePoint(new Point(TorpSprite.BaseImageLocation.X + TorpSprite.GetSize.Width, yPos)));

            foreach(Sprite one in SpritesWeHit)
            {
                //Are we the sprite at the same point that we are at?  If so, skip ourselevs
                if (one == TorpSprite) continue; //Nothing to do if we are where we are

                //do not do anything with it, if it does not have a valid payload.  (the payload tells us what team the sprite is on)
                if(one.payload is TorpSpritePayload)
                {
                    TorpSpritePayload TargetTSP = (TorpSpritePayload)one.payload;
                    if (TargetTSP.isGood == TSP.isGood) continue; //FriendlyFire - do not kill things on the same team
                    //We need to blow it up here..
                    TorpedoKillsSomething(TorpSprite, one);
                }
            }

            //Get rid of the sprite if it has exited to the right, left, top, or bottom
            if (e.NewLocation.X + TorpSprite.GetSize.Width < 0) TorpSprite.Destroy(); //It has left visible range
            if (e.NewLocation.X > pbOcean.BackgroundImage.Width) TorpSprite.Destroy(); //It has left visible range
            if (e.NewLocation.Y + TorpSprite.GetSize.Height < 0) TorpSprite.Destroy(); //It has left visible range
            if (e.NewLocation.Y > pbOcean.BackgroundImage.Height) TorpSprite.Destroy(); //It has left visible range

            //Explode if it hits the water-level and is going up
            if (TSP.lastdirection == Direction.up && e.NewLocation.Y < constants.WaterLevel)
                Explode(TorpSprite);

            //explode if it is going down and hits the ground.
            if (TSP.lastdirection == Direction.down && e.NewLocation.Y > constants.GroundLevel)
                Explode(TorpSprite);
        }

        /// <summary>
        /// For all the ships, subs, or whales.  When we have moved to a new location...
        /// This function is added when the sprites are loaded.
        /// </summary>
        /// <param name="Sender">The sprite that is moving</param>
        /// <param name="e"></param>
        void BadItemCheckBeforeMove(object Sender, SpriteEventArgs e)
        {
            if (!(Sender is Sprite)) return;
            Sprite TorpSprite = (Sprite)Sender;
            //If it does not have a valid payload, destroy it.
            if (!(TorpSprite.payload is TorpSpritePayload))
            {
                TorpSprite.Destroy(); //It needs to be killed
                return;
            }

            //Track if it is on the screen or not.  Sometimes a sprite gets lost.
            TorpSpritePayload TSP = (TorpSpritePayload)TorpSprite.payload;
            if(e.NewLocation.X + TorpSprite.GetSize.Width >= 0 && e.NewLocation.X < pbOcean.BackgroundImage.Width )
            {
                if(e.NewLocation.Y + TorpSprite.GetSize.Height >=0  && e.NewLocation.Y < pbOcean.BackgroundImage.Height)
                {
                    TSP.LastTimeOnScreen = DateTime.UtcNow;
                }
            }

            //If it has been off the screen for more than half a second, it has a problem.
            if((DateTime.UtcNow - TSP.LastTimeOnScreen).TotalMilliseconds > 500)
            {
                TorpSprite.Destroy();
                return;
            }

            //remove the sprite if it successfully exits the screen, and dock our score.
            if (TSP.lastdirection == Direction.left && e.NewLocation.X + TorpSprite.GetSize.Width < 0)
            {
                Score -= TSP.WorthGetAway;
                TorpSprite.Destroy(); //It has left visible range
            }
            if (TSP.lastdirection == Direction.right && e.NewLocation.X > pbOcean.BackgroundImage.Width)
            {
                Score -= TSP.WorthGetAway;
                TorpSprite.Destroy(); //It has left visible range
            }
            if (e.NewLocation.Y + TorpSprite.GetSize.Height < 0)
            {
                Score -= TSP.WorthGetAway;
                TorpSprite.Destroy(); //It has left visible range
            }
            if (e.NewLocation.Y > pbOcean.BackgroundImage.Height)
            {
                Score -= TSP.WorthGetAway;
                TorpSprite.Destroy(); //It has left visible range
            }

            //If the enemy is a sub, do we want to shoot a torpedo?
            if(TorpSprite.SpriteOriginName == SpriteName.EnemySub.ToString())
            {
                bool TargetedTorpedo = false;
                if (BadSubsDoTorpedoes && (DateTime.UtcNow - TSP.LastTorpedoTime).TotalMilliseconds > constants.TimeForBadGuysToReloadTorpedos)
                {
                    //Figure out which direction to send the torpedo
                    Direction where = Direction.none;
                    if (TorpSprite.BaseImageLocation.X < PlayerSub.BaseImageLocation.X) where = Direction.right;
                    if (TorpSprite.BaseImageLocation.X > PlayerSub.BaseImageLocation.X) where = Direction.left;
                    if (where != Direction.none && TheGameController.RandomNumberGenerator.Next(100) < 10)
                    {
                        if (BadTorpedosCanBeAimed && TheGameController.RandomNumberGenerator.Next(5) == 0)
                            TargetedTorpedo = true;
                        if (TargetedTorpedo) //If we are shooting a targeted torpedo, we do it differently
                            ShootTorpedo(TorpSprite, PlayerSub);
                        else //Otherwise, we just shoot it one direction.
                            ShootTorpedo(TorpSprite, where);
                        //Update the time, saying we just shot a torpedo.  Now it needs to wait a bit before we can shoot another
                        TSP.LastTorpedoTime = DateTime.UtcNow;                
                    }
                }
            }
            //If we are a destroyer, we might do a torpedo, or depth-charges.
            if (TorpSprite.SpriteOriginName == SpriteName.Destroyer.ToString())
            {
                //If destroyers do torpedos, and we have not done one recently
                if (BadDestroyersDoTorpedos && (DateTime.UtcNow - TSP.LastTorpedoTime).TotalMilliseconds > constants.TimeForBadGuysToReloadTorpedos)
                {
                    //Decide if we are going to do one now
                    int WhatToDo = TheGameController.RandomNumberGenerator.Next(100);
                    if (WhatToDo < 20)
                    {
                        //Decide what sort of torpedo to shoot (a directed one, or a straight one)
                        WhatToDo = TheGameController.RandomNumberGenerator.Next(100);
                        if (BadTorpedosCanBeAimed && WhatToDo < 30)
                            ShootTorpedo(TorpSprite, PlayerSub);
                        else
                            ShootTorpedo(TorpSprite, Direction.down);
                    }
                    TSP.LastTorpedoTime = DateTime.UtcNow;
                }
                
                //Do we do a depth-charge?  Has enough time passed?
                if (BadDestroyersDoDepthCharges && (DateTime.UtcNow - TSP.LastDepthChargeTime).TotalMilliseconds > constants.TimeForBadGuysToReloadTorpedos)
                {
                    //Decide if we want to do it now
                    int WhatToDo = TheGameController.RandomNumberGenerator.Next(100);
                    if (WhatToDo < 20)
                    {
                        //Check if we do just one, or if multiple
                        if (BadDestroyersDoMultipleDepthCharges)
                        {
                            //randomly choose which sort of charges to do
                            WhatToDo = TheGameController.RandomNumberGenerator.Next(100);
                            if (WhatToDo < 40)
                                DropDepthCharge(TorpSprite, ChargeDir.down);
                            else if (WhatToDo < 70)
                                DropDepthCharge(TorpSprite, ChargeDir.toss_left | ChargeDir.toss_right);
                            else
                                DropDepthCharge(TorpSprite, ChargeDir.toss_left | ChargeDir.toss_right | ChargeDir.down);
                        }
                        else
                        {
                            //Just one depth-charge.  Which direction?
                            WhatToDo = TheGameController.RandomNumberGenerator.Next(100);
                            if (WhatToDo < 40)
                                DropDepthCharge(TorpSprite, ChargeDir.down);
                            else if (WhatToDo < 70)
                                DropDepthCharge(TorpSprite, ChargeDir.toss_left);
                            else
                                DropDepthCharge(TorpSprite, ChargeDir.toss_right);
                        }
                        TSP.LastDepthChargeTime = DateTime.UtcNow;
                    }
                }
            }
            UpdateForm();
        }

        /// <summary>
        /// Explode a sprite, making the appropriate sound.
        /// </summary>
        /// <param name="NeedsExploding"></param>
        void Explode(Sprite NeedsExploding)
        {
            NeedsExploding.Destroy();
            //Play the appropriate sound
            //CargoShip, Destroyer, PlayerSub, EnemySub, Torpedo, DepthCharge, Whale, Explosion
            switch (NeedsExploding.SpriteOriginName)
            {
                case "CargoShip":
                case "PlayerSub":
                case "Destroyer":
                case "EnemySub":
                case "DepthCharge":
                    PlaySound(SoundItem.big_bang);
                    break;
                case "Torpedo":
                    PlaySound(SoundItem.small_bang);
                    break;
                case "Whale":
                    PlaySound(SoundItem.splat);
                    break;
            }
            //Make an explosion
            Sprite Explosion = TheGameController.DuplicateSprite(SpriteName.Explosion.ToString());
            if (NeedsExploding.SpriteOriginName == SpriteName.DepthCharge.ToString())
            {
                //Depth-charges are bigger than the charge itself
                Explosion.SetSize(constants.DepthChargeExplosionSize);
                Point ExplodePoint = new Point((NeedsExploding.BaseImageLocation.X + NeedsExploding.GetSize.Width / 2) - constants.DepthChargeExplosionSize.Width / 2,
                    (NeedsExploding.BaseImageLocation.Y + NeedsExploding.GetSize.Height / 2) - constants.DepthChargeExplosionSize.Height / 2);
                Explosion.PutBaseImageLocation(ExplodePoint);
                //Check to see if the player is in range and damage them if they are
            }
            else
            {
                //Everything else is the same size as the item that got exploded
                Explosion.SetSize(NeedsExploding.GetSize);
                Explosion.PutBaseImageLocation(NeedsExploding.BaseImageLocation);
            }
            //Tell the explosion to animate once.  Once it is done the ExplosionCompletes event is triggered.
            //The trigger was set into place during the LoadSprites function.
            Explosion.AnimateOnce(0);
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
        }

        //Before the player moves.  Make sure it is a valid movement
        //Ensure he is not moving off the playing-field.
        void PlayerCheckBeforeMovement(object Sender, SpriteEventArgs e)
        {
            if (! (Sender is Sprite)) return;
            Sprite Target = (Sprite)Sender;
            Direction Where = SpriteDirection(Target);
            //Check left side
            if(e.NewLocation.X < constants.DistanceFromSide && (Where == Direction.down_left || Where == Direction.left || Where == Direction.up_left))
            {
                e.NewLocation.X = constants.DistanceFromSide;
            }

            //Check right side
            if (e.NewLocation.X > pbOcean.BackgroundImage.Width - constants.DistanceFromSide - PlayerSub.GetSize.Width && 
                (Where == Direction.down_right || Where == Direction.right || Where == Direction.up_right))
            {
                e.NewLocation.X = pbOcean.BackgroundImage.Width - constants.DistanceFromSide - PlayerSub.GetSize.Width;
            }

            //check top
            if (e.NewLocation.Y < constants.WaterLevel + constants.DistanceFromBot && (Where == Direction.up_left || Where == Direction.up || Where == Direction.up_right))
            {
                e.NewLocation.Y = constants.WaterLevel + constants.DistanceFromBot;
            }

            //Check bottom
            if (e.NewLocation.Y > constants.GroundLevel - constants.DistanceFromBot - PlayerSub.GetSize.Height &&
                (Where == Direction.down_right || Where == Direction.down || Where == Direction.down_left))
            {
                e.NewLocation.Y = constants.GroundLevel - constants.DistanceFromBot - PlayerSub.GetSize.Height;
            }

        }

        /// <summary>
        /// Return the direction a particular sprite is moving.
        /// </summary>
        /// <param name="Which"></param>
        /// <returns></returns>
        Direction SpriteDirection(Sprite Which)
        {
            Double dir = Which.GetSpriteDegrees();
            if (dir > 30 && dir < 50) return Direction.up_right; //close to 45 degrees
            if (dir > 85 && dir < 95) return Direction.up; //close to 90 degrees
            if (dir > 130 && dir < 140) return Direction.up_left; //close to 135 degrees
            if (dir > 175 && dir < 185) return Direction.left; //close to 180 degrees
            if (dir > 220 && dir < 230) return Direction.down_left; //close to 225 degrees
            if (dir > 265 && dir < 275) return Direction.down; //close to 270
            if (dir > 310 && dir < 320) return Direction.down_right; //close to 315
            if (dir > -5 && dir < 5) return Direction.right; //close to zero degrees
            if (dir > 355 && dir < 365) return Direction.right; //close to 360 degrees

            return Direction.none;
        }

        /// <summary>
        /// Count the bad-guys on the playing-field
        /// </summary>
        /// <returns></returns>
        ItemCount BadguyCount()
        {
            ItemCount Counter = new ItemCount();

            foreach (Sprite one in TheGameController.SpritesBasedOffAnything())
            {
                if (one.SpriteOriginName == SpriteName.CargoShip.ToString())
                    Counter.Cargo++;
                if (one.SpriteOriginName == SpriteName.Destroyer.ToString())
                    Counter.Destroyer++;
                if (one.SpriteOriginName == SpriteName.EnemySub.ToString())
                    Counter.Sub++;
                if (one.SpriteOriginName == SpriteName.Whale.ToString())
                    Counter.Whale++;
            }
            return Counter;
        }

        //Every so often, we check to see if we need to update the level.  When we do, we change
        //What happens in the game.
        void UpdateLevel()
        {
            int oldlevel = level;
            double seconds = (DateTime.UtcNow - GameStartTime).TotalSeconds;
            int count = (int)(Math.Round(seconds) / 15);
            level = count;
            if (level != oldlevel)
            {
                //The level has increased.
                if (level < 5)
                    NumberOfBadguys++; //increase the number of things that show up
                if (level == 5) BadSubsDoTorpedoes = true;
                if (level == 7) BadDestroyersDoDepthCharges = true;
                if (level == 9) { BadDestroyersDoTorpedos = true; BadTorpedosCanBeAimed = true; }
                if (level == 11) BadDestroyersDoMultipleDepthCharges = true;
                if (level == 13) NumberOfBadguys++;
                if (level == 15) NumberOfBadguys++;
                if (level == 16) NumberOfBadguys++;
                if (level > 17) NumberOfBadguys++;
                UpdateForm();
            }
        }

        //Add one thing to the game.  A whale, destroyer, cargo-ship, or enemy sub
        void CreateOneItem(SpriteName What)
        {
            Sprite newSprite = null;
            TorpSpritePayload TSP;
            TSP = new TorpSpritePayload();
            int distanceOut = 20;
            int Xlocation;
            int YLocation = -50;
            switch (What)
            {
                case SpriteName.CargoShip:
                    newSprite = TheGameController.DuplicateSprite(What.ToString());
                    YLocation = constants.WaterLevel + 7 - newSprite.GetSize.Height;
                    newSprite.MovementSpeed = constants.CargoSpeed;
                    TSP.WorthDestroyed = 50;
                    TSP.WorthGetAway = 15;
                    break;
                case SpriteName.Destroyer:
                    newSprite = TheGameController.DuplicateSprite(What.ToString());
                    YLocation = constants.WaterLevel + 7 - newSprite.GetSize.Height;
                    newSprite.MovementSpeed = constants.DestroyerSpeed;
                    TSP.WorthDestroyed = 200;
                    TSP.WorthGetAway = 5;
                    break;
                case SpriteName.EnemySub:
                    newSprite = TheGameController.DuplicateSprite(What.ToString());
                    YLocation = constants.WaterLevel + constants.DistanceFromTop +
                        TheGameController.RandomNumberGenerator.Next(constants.GroundLevel - constants.DistanceFromBot - constants.WaterLevel - constants.DistanceFromTop);
                    newSprite.MovementSpeed = constants.BadSubSpeed;
                    TSP.WorthDestroyed = 120;
                    TSP.WorthGetAway = 5;
                    break;
                case SpriteName.Whale:
                    newSprite = TheGameController.DuplicateSprite(What.ToString());
                    YLocation = constants.WaterLevel + constants.DistanceFromTop +
                        TheGameController.RandomNumberGenerator.Next(constants.GroundLevel - constants.DistanceFromBot - constants.WaterLevel - constants.DistanceFromTop);
                    newSprite.MovementSpeed = constants.WhaleSpeed;
                    TSP.WorthDestroyed = 100;
                    TSP.WorthGetAway = 0;
                    break;
            }
            Xlocation = 0 - distanceOut - newSprite.GetSize.Width;
            int Choice = TheGameController.RandomNumberGenerator.Next(2);
            if (Choice == 0)
                Xlocation = pbOcean.BackgroundImage.Width + distanceOut; //start at the right side
            if (newSprite != null)
            {
                TSP.isGood = false;                
                newSprite.PutBaseImageLocation(new Point(Xlocation, YLocation));
                if (Xlocation < 5)
                {
                    newSprite.SetSpriteDirectionDegrees(0);
                    TSP.lastdirection = Direction.right;
                    newSprite.ChangeAnimation(0); //right
                }
                else
                {
                    newSprite.SetSpriteDirectionDegrees(180);
                    newSprite.ChangeAnimation(1); //left
                }
                newSprite.payload = TSP;
                newSprite.AutomaticallyMoves = true;
            }
        }

        //Close the game, saving the size of the form when we do so
        private void SubDemoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Width >= MinimumSize.Width)
                Properties.Settings.Default.StartWidth = Width;
            if (Height >= MinimumSize.Height)
                Properties.Settings.Default.StartHeight = Height;
            Properties.Settings.Default.Save();
        }
    }
}
