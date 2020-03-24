Contents:
* About SubDemo:
* Why make this program
* How I Made This Program
- SubDemoForm
- DoTick
- CheckForKeypress
* Events
- BadItemCheckBeforeMove
- TorpedoItemCheckBeforeMove
- ExplosionCompletes
* One other function
- PlayerSubTakesDamage

About SubDemo:
SubDemo was created by Tim Young on 7/16/2016, primarily so the code could be posted on CodeProject.com.  The game was based off a game I played on the Commodore 64 many years back.  I do not remember what the name of that game was.  So I made something that was something like it.

Why Make This Program:
SubDemo is yet another demonstration of the SpriteLibrary system. (http://www.codeproject.com/Articles/1085446/Using-Sprites-Inside-Windows-Forms)  I made it, in part, to ensure that the SpriteLibrary was capable of making this sort of game.  Every so often, as I program something, I find a limitation of the SpriteLibrary, and I add new features.  But also, I wanted to have a few more demos out there, so programmers can see some examples of how to do things with it.


How I Made This Program
Once again, the main portion of the code flows through the DoTick function in the SubDemoForm.cs file.  That is where you will spend most of your time as you figure out how the game works.

SubDemoForm
The SubDemoForm instantiation, is where the entire game is set up.  We start by using the height and width of the window, from the last time the game was played (those values are set in the SubDemoForm_FormClosing function, which is called when the form is closed).  We create a new SpriteController (and, at the same time, register the DoTick function).  Notice that we set the pbOcean.BackgroundImage before we create the SpriteController.  The SpriteController gets some information from the BackgroundImage (namely the width and height, which it keeps until the SpriteController is destroyed), and so that image must be set to something before the SpriteController is created. 

We load the sprites from the resources, and then we print the directions on the starting screen.  Then, the function ends.  Every 10 milliseconds, DoTick is called from the SpriteController, which checks for a keypress.

DoTick starts by checking to see if a game is being played.  If not, the only thing it does is check to see if the Enter key has been pressed.  If Enter has not been pressed, then DoTick terminates and does nothing.  If Enter has been pressed, then it sets up the game, which sets “PlayingGame” = true.

DoTick
Once we are playing the game, then DoTick calls CheckForKeypress.  CheckForKeypress will deal with player movement and firing of player torpedoes.  After that, we see if the human sub needs to be healed.  Finally, DoTick adds extra bad-guys to the game if we need to do that.

CheckForKeypress
We exit out early if we have checked for keypress recently.  This is mainly so we do not overwhelm the processor with key-press checks.  We want to give the SpriteController time to do things.  So we check every 100 milliseconds instead of every 10 milliseconds.

Start by updating the level (changing the difficulty) if needed.

Then, we check to see if the various keys are depressed, to see if we should go left, right, up, down, or some other direction.

Once we know which directions we have been asked to go, we check to see if we have contradictory directions (up/down).  If so, we skip them.

One odd thing with the SpriteController, is that, if we keep telling the sprite to so something we have already told it to do, it usually does NOT do it.  It takes time for it to be told what to do, and it takes time for it to do it.  If we keep telling it to do something, it ends up spending all its time listening to instructions instead of having time to follow instructions.  We sometimes do that to each-other, telling them how to do something for more time than it takes to actually do the job.  So the CheckKeypress function checks to see if the Sprite is already going in the direction we are about to tell it to go.  If it is, we let it go unmolested.  If it is going in a different direction, only then do we tell it to change to the new direction.

After we deal with going directions, we take a moment to see if the player has asked to fire any torpedos.  If so, we fire them.

At the end, we check to see if we have told the submarine to move.  If there have been no instructions telling it to move, we tell it to stop until told to move again.

Events:
There are a number of events triggered on the various sprites, which make things work.  These events are specified at the time that the original sprites were created (see the LoadSprites function.)  There are lines, which look like:
            one.CheckBeforeMove += BadItemCheckBeforeMove;
This line comes from the CargoShip sprite.  It is a “BadItem” (not the player), and the event is triggered every time the Sprite moves to a new location.  The BadItemCheckBeforeMove function will check to see if the sprite has moved off the screen, and if so, remove it from play.  It also checks to see if the sprite should fire a torpedo, drop a depthcharge, or something like that.
Torpedoes have a different function:
            one.CheckBeforeMove += TorpedoCheckBeforeMovement;
The torpedo checks to see if it has hit the ground, is about to go into the sky, or impacts something it should kill.  The torpedo behaves very differently than a ship, whale, or enemy destroyer does.  So it needs to have a different function.
DepthCharges also work a bit differently, so, as you can imagine, they have their own CheckBeforeMovement function.
The Explosion Sprite has a very different function.
one.SpriteAnimationComplete += ExplosionCompletes;
When an explosion completes the animation, it is done.  Instead of flashing over and over, it explodes once, and then it is supposed to stop.  So, we tell the explosion to “AnimateOnce”, and then, when the explosion animation stops, the ExplosionCompletes event happens.  Basically, we simply remove the explosion sprite (We tell it to “destroy”, which erases it from the system).
One Other Function

PlayerSpriteTakesDamage
Probably the other main function needing an explanation is the PlayerSpriteTakesDamage function.  This one is what happens if we run into a whale, into a bad vehicle, get shot by a torpedo, or get damaged by a depth-charge.  The submarine takes damage.  If we take too much damage, the game is over.
When the game is over, we tell the player submarine to explode.  Something very funny would happen as we pop up this little “Game over” window.  If we were not very careful with things, either the game keeps popping up windows, or the game continues to go.  Remember, the DoTick function is firing off every 10 milliseconds.  That means it happens 100 times a second.  If it pops up a window when it notices that the player is dead, it can pop up 100 windows a second.  That is not what we were intending to do.  If you notice, we specify “PlayingGame = false” before we pop open our Game Over window.  The other sprites that existed will continue to do their thing, because the Sprite Controller continues to work, but our DoTick function exits out immediately.  This is probably the key to making things with the SpriteController.  Keep track of whatever mode the game is in, and exit out of most functions if you have somehow gotten to a mode that you were not expecting that function to work in.  You would be surprised how things keep popping up, even though you thought you were done.

