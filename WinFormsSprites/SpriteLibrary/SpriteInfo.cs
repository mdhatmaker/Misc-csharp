using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Resources;


namespace SpriteLibrary
{
    /// <summary>
    /// The SpriteInfo only recognizes a few ways to create a sprite.  Here are the different ways.
    /// </summary>
    public enum AnimationType {
        /// <summary>
        /// A Sprite definition knows an image, an X and Y, a size, and a few other items.
        /// </summary>
        SpriteDefinition =0,
        /// <summary>
        /// A rotated sprite is based off a pre-existing animation, but it is rotated by some degrees.
        /// </summary>
        Rotation =1,
        /// <summary>
        /// A mirrored sprite is based off a pre-existing animation, but is mirrored vertically or horizontally.
        /// </summary>
        Mirror =2 }

    /// <summary>
    /// An AnimationInfo class is used by the <see cref="SpriteLibrary.SpriteInfo">SpriteInfo</see> class to 
    /// contain the instructions for creating a sprite through the dictionary.  Most people will not want
    /// to manually use these.  It is simplest to use the <see cref="SpriteLibrary.SpriteDatabase.OpenEditWindow(int)">
    /// SpriteDatabase.OpenEditWindow</see> function and use the built-in sprite editor.  That editor will create a file
    /// that can be used in the database without your needing to know about the AnimationInfo.
    /// Again, you do not want to use these within
    /// your program.  Let the SpriteDatabase use this.  The reason these are visible to the program is because
    /// This code uses "XML Serialization" to load and save.  XML Serialization requires the items you are
    /// serializing to be "public", which makes them visible.
    /// </summary>
    public class AnimationInfo
    {
        /// <summary>
        /// The FieldsToUse tracks which of the values in AnimationInfo are important
        /// </summary>
        public AnimationType FieldsToUse = AnimationType.SpriteDefinition;
        /// <summary>
        /// If the sprite is either a mirror sprite, or a rotated sprite, it must be based off of
        /// a pre-existing animation.  This value states which animation we copy.
        /// </summary>
        public int AnimationToUse = 0;
        /// <summary>
        /// If the sprite is a rotated copy of a pre-existing sprite, this value tells how many degrees to
        /// rotate the sprite.
        /// </summary>
        public int RotationDegrees=0;
        /// <summary>
        /// If the sprite is a mirrored copy of a pre-existing sprite, this value states whether or not
        /// the sprite is mirrored Horizontally.
        /// </summary>
        public bool MirrorHorizontally = false;
        /// <summary>
        /// If the sprite is a mirrored copy of a pre-existing sprite, this value states whether or not
        /// the sprite is mirrored Vertically.
        /// </summary>
        public bool MirrorVertically = false;
        /// <summary>
        /// If the sprite is based off of an image, this value is the starting point of the top-left corner
        /// of the sprite on the image.  You will also want to include a Width and Height.
        /// </summary>
        public Point StartPoint = new Point(-1, -1);
        /// <summary>
        /// This is the image name which contains the sprite.  This image should be in the Properties.Resources
        /// of your project.  The name you want to give is case-sensitive, and should be the exact name as
        /// listed in Properties.Resources.  For example, if your image name were Properties.Resources.Runner
        /// you would want to use the string "Runner"  Note the caps are identical, and we have removed the
        /// "Properties.Resources. from the front.
        /// </summary>
        public string ImageName = "";
        /// <summary>
        /// The width of the sprite to pull from the specified image.
        /// </summary>
        public int Width = -1;
        /// <summary>
        /// The height of the sprite to pull from the specified image.
        /// </summary>
        public int Height = -1;
        /// <summary>
        /// The number of frames to pull, one following the other, from the specified image.
        /// </summary>
        public int NumFrames = 1;
        /// <summary>
        /// The delay in milliseconds in-between frames of the sprite.  This number is not exact, but is pretty
        /// close to what happens.  Never use a number less than 20.
        /// </summary>
        public int AnimSpeed = 200;

        /// <summary>
        /// A generic cloning method that works when everything is public
        /// </summary>
        /// <returns>A clone of the specified AnimationInfo</returns>
        public AnimationInfo Clone()
        {
            return SpriteDatabase.CloneByXMLSerializing<AnimationInfo>(this);
        }
    }

    /// <summary>
    /// A class which is used by the <see cref="SpriteLibrary.SpriteDatabase">SpriteDatabase</see> to build
    /// Sprites.  You should not need to use this in your programming.  It is simplest to use the Load/Save features
    /// of the SpriteDatabase, which will load and save the SpriteInfo.
    /// Again, you do not want to use these within
    /// your program.  Let the SpriteDatabase use this.  The reason these are visible to the program is because
    /// This code uses "XML Serialization" to load and save.  XML Serialization requires the items you are
    /// serializing to be "public", which makes them visible.
    /// </summary>
    public class SpriteInfo
    {
        /// <summary>
        /// The name of the sprite.  It should be unique within your application
        /// </summary>
        public string SpriteName = "";
        /// <summary>
        /// The percentage size when the sprite is normally displayed.  For example:  If the image you drew your
        /// sprite on has your sprite drawn on a 200x200 grid, but you want your sprite to normally be 100x100, 
        /// you would tell it to be 50 (50 percent of the original size).  
        /// </summary>
        public int ViewPercent = 100;  //The percent size of the sprite.  100 is full.  50 is half-size
        /// <summary>
        /// This is the list of animations that make up the sprite.  Again, you do not want to use these within
        /// your program.  Let the SpriteDatabase use this.  The reason these are visible to the program is because
        /// This code uses "XML Serialization" to load and save.  XML Serialization requires the items you are
        /// serializing to be "public", which makes them visible.
        /// </summary>
        public List<AnimationInfo> Animations = new List<AnimationInfo>();

        /// <summary>
        /// A generic cloning method that works when everything is public
        /// </summary>
        /// <returns>A duplicate of the sprite info.</returns>
        public SpriteInfo Clone()
        {
            return SpriteDatabase.CloneByXMLSerializing<SpriteInfo>(this);
        }

        /// <summary>
        /// Update the current SpriteInfo class such that it is identical to the class you are copying from.
        /// </summary>
        /// <param name="toCopyFrom">A spriteInfo class</param>
        public void CopyFrom(SpriteInfo toCopyFrom)
        {
            if (toCopyFrom == null) return;
            SpriteName = toCopyFrom.SpriteName;
            ViewPercent = toCopyFrom.ViewPercent;
            Animations.Clear();
            foreach(AnimationInfo AI in toCopyFrom.Animations)
            {
                Animations.Add(AI.Clone());
            }
        }

        /// <summary>
        /// Create a sprite using the database sprite information.  This does not do any checking to make sure
        /// the named sprite already exists.  Usually, what you want to do is to create your SpriteController and
        /// register your SpriteDatabase with the controller.  Then, when you ask the SpriteController for a sprite,
        /// if that sprite does not exist yet, it will create it from the database.
        /// </summary>
        /// <param name="ControllerToUse">The sprite controller that will end up controlling the sprite</param>
        /// <param name="TheDatabaseToUse">The database</param>
        /// <returns></returns>
        internal Sprite CreateSprite(SpriteController ControllerToUse, SpriteDatabase TheDatabaseToUse)
        {
            Sprite DestSprite = null;
            if (ControllerToUse == null) return null;
            for (int index = 0; index < Animations.Count; index++)
            {
                AnimationInfo CurrentAnimation = Animations[index];
                Image myImage = TheDatabaseToUse.GetImageFromName(CurrentAnimation.ImageName, true);
                if (myImage == null) return null;  //break out if we do not have the image defined for this
                AnimationType AT = CurrentAnimation.FieldsToUse;
                if (index == 0) AT = AnimationType.SpriteDefinition;  //the first one MUST be this.
                switch(AT)
                {
                    case AnimationType.SpriteDefinition:
                        if(DestSprite == null)//Creating the sprite from scratch
                        {
                            DestSprite = new Sprite(CurrentAnimation.StartPoint, ControllerToUse, myImage, CurrentAnimation.Width, CurrentAnimation.Height, CurrentAnimation.AnimSpeed, CurrentAnimation.NumFrames);
                        }
                        else
                        {
                            DestSprite.AddAnimation(CurrentAnimation.StartPoint, myImage, CurrentAnimation.Width, CurrentAnimation.Height, CurrentAnimation.AnimSpeed, CurrentAnimation.NumFrames);
                        }
                        break;
                    case AnimationType.Rotation:
                        DestSprite.AddAnimation(CurrentAnimation.AnimationToUse, CurrentAnimation.RotationDegrees);
                        break;
                    case AnimationType.Mirror:
                        DestSprite.AddAnimation(CurrentAnimation.AnimationToUse, CurrentAnimation.MirrorHorizontally,CurrentAnimation.MirrorVertically);
                        break;
                }
            }
            int sizepercent = ViewPercent;
            if (sizepercent < 5) sizepercent = 100;
            if (sizepercent > 300) sizepercent = 100;
            double delta = (double)sizepercent / 100.0; //turn it into a double, and into something we can multiply.
            DestSprite.SetSize(new Size((int)(DestSprite.GetSize.Width * delta), (int)(DestSprite.GetSize.Height * delta)));
            DestSprite.SetName(SpriteName);
            //We have created a new sprite.  Now, return a duplicate of that sprite.
            return DestSprite;
        }
    }
}
