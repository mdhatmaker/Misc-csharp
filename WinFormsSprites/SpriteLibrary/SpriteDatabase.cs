using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Resources;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace SpriteLibrary
{
    internal struct ImageStruct
    {
        internal Image TheImage;
        internal string ImageName;
    }

    /// <summary>
    /// Store Sprite information in a database.  You can preload your database with sprite definitions, and then
    /// create the sprites as needed.  This can drastically reduce the initial load time of a game or something.
    /// Though, what it really does is spread out the load time.  It still takes the same amount of time to
    /// load all the sprites, it just loads them on-demand.  Using a SpriteDatabase often hides any load time issues.
    /// </summary>
    /// <example>
    /// This is an example of how to use a SpriteDatabase.
    /// When you begin developing your project, you want to start by creating a SpriteDatabase and pointing
    /// it to a file, and then opening up an <see cref="SpriteDatabase.OpenEditWindow(int)">EditorWindow.</see>
    /// <code lang="C#">
    /// public partial class MyGameForm : Form
    /// {
    ///     SpriteController mySpriteController = null;
    ///     SpriteDatabase mySpriteDatabase = null;
    ///     
    ///     public MyGameForm()
    ///     {
    ///         InitializeComponent();
    ///         MainDrawingArea.BackgroundImage = Properties.Resources.Background;
    ///         MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;
    ///         
    ///         string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    ///         string MyFile = Path.Combine(Desktop, "myFile.xml");
    ///         mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, MyFile);
    ///         
    ///         mySpriteController = new SpriteController(MainDrawingArea, mySpriteDatabase);
    ///         
    ///         mySpriteDatabase.OpenEditWindow();
    ///         mySpriteDatabase.Save();
    ///     }
    /// }
    /// </code>
    /// The Editor Window will let you find the sprites that are contained in the various images you have
    /// as resources in your program, and it will save a file with those sprite templates.  Any SpriteController
    /// that you have instantiated with a Sprite Database (see <see cref="SpriteController(PictureBox, SpriteDatabase)"/>)
    /// will now be able to create named sprites from the templates defined in the database.  After the first use, the
    /// named sprites will be accessible from within that controller just like any other named sprites.
    /// <para/>
    /// After you have created your SpriteDatabase file, you will want to add your file to your program resources.
    /// Then, you will change the SpriteDatabase to use the resource instead of a file.  If we named the file
    /// "MySpriteDatabase.xml", and it got added to your resources with the name "MySpriteDatabase", you would
    /// pass "MySpriteDatabase" to the database instantiation.
    /// <code lang="C#">
    /// public partial class MyGameForm : Form
    /// {
    ///     SpriteController mySpriteController = null;
    ///     SpriteDatabase mySpriteDatabase = null;
    ///     
    ///     public MyGameForm()
    ///     {
    ///         InitializeComponent();
    ///         MainDrawingArea.BackgroundImage = Properties.Resources.Background;
    ///         MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;
    ///         
    ///         //string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    ///         //string MyFile = Path.Combine(Desktop, "myFile.xml");
    ///         //mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, MyFile);
    ///         mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, "MySpriteDatabase");
    ///         
    ///         mySpriteController = new SpriteController(MainDrawingArea, mySpriteDatabase);
    ///         
    ///         //mySpriteDatabase.OpenEditWindow();
    ///         //mySpriteDatabase.Save();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class SpriteDatabase
    {
        /// <summary>
        /// This is the list of SpriteInfo records that the database knows about.  You can create your own list,
        /// modify this list, or whatever.  The database has some reasonable functions for loading and saving a
        /// sprite database.
        /// </summary>
        public List<SpriteInfo> SpriteInfoList = new List<SpriteInfo>();
        List<ImageStruct> TheImages = new List<ImageStruct>();
        ResourceManager myResourceManager = null;
        string Filename = "";
        Size SnapGridSize = new Size(5, 5);
        System.Drawing.Icon LibIcon = null;

        /// <summary>
        /// The sprite database instantiation function.  The filename can either be a file on the computer or it
        /// can be the string name of a resource (the filename without the extension.  If your file is accessed
        /// by Properties.Resources.MySprites, the "filename" would be "MySprites")
        /// </summary>
        /// <example>
        /// This is an example of how to use a SpriteDatabase.
        /// When you begin developing your project, you want to start by creating a SpriteDatabase and pointing
        /// it to a file, and then opening up an EditorWindow.
        /// <code lang="C#">
        /// public partial class MyGameForm : Form
        /// {
        ///     SpriteController mySpriteController = null;
        ///     SpriteDatabase mySpriteDatabase = null;
        ///     
        ///     public MyGameForm()
        ///     {
        ///         InitializeComponent();
        ///         MainDrawingArea.BackgroundImage = Properties.Resources.Background;
        ///         MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;
        ///         
        ///         string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        ///         string MyFile = Path.Combine(Desktop, "MySprites.xml");
        ///         mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, MyFile);
        ///         
        ///         mySpriteController = new SpriteController(MainDrawingArea, mySpriteDatabase);
        ///         
        ///         mySpriteDatabase.OpenEditWindow();
        ///         mySpriteDatabase.Save();
        ///     }
        /// }
        /// </code>
        /// The Editor Window will let you find the sprites that are contained in the various images you have
        /// as resources in your program, and it will save a file with those sprite templates.  Any SpriteController
        /// that you have instantiated with a Sprite Database (see <see cref="SpriteController(PictureBox, SpriteDatabase)"/>)
        /// will now be able to create named sprites from the templates defined in the database.  After the first use, the
        /// named sprites will be accessible from within that controller just like any other named sprites.
        /// <para/>
        /// After you have created your SpriteDatabase file, you will want to add your file to your program resources.
        /// Then, you will change the SpriteDatabase to use the resource instead of a file.  If we named the file
        /// "MySpriteDatabase.xml", and it got added to your resources with the name "MySpriteDatabase", you would
        /// pass "MySpriteDatabase" to the database instantiation.
        /// <code lang="C#">
        /// public partial class MyGameForm : Form
        /// {
        ///     SpriteController mySpriteController = null;
        ///     SpriteDatabase mySpriteDatabase = null;
        ///     
        ///     public MyGameForm()
        ///     {
        ///         InitializeComponent();
        ///         MainDrawingArea.BackgroundImage = Properties.Resources.Background;
        ///         MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;
        ///         
        ///         //string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        ///         //string MyFile = Path.Combine(Desktop, "MySprites.xml");
        ///         //mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, MyFile);
        ///         mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, "MySprites");
        ///         
        ///         mySpriteController = new SpriteController(MainDrawingArea, mySpriteDatabase);
        ///         
        ///         //mySpriteDatabase.OpenEditWindow();
        ///         //mySpriteDatabase.Save();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="theResourceManager">The ResourceManager for your project.  Usually 
        /// Properties.Resources.ResourceManager</param>
        /// <param name="filename">Either a path and file (like: @"c:\users\me\Desktop\myfile.xml") or 
        /// the name of a resource (like: "myfile")</param>
        public SpriteDatabase(ResourceManager theResourceManager, string filename)
        {
            myResourceManager = theResourceManager;
            Filename = filename;
            Load();
        }

        internal void Load()
        {
            LoadSpriteInfo();
        }

        internal ResourceManager GetResourceManager()
        {
            return myResourceManager;
        }

        /// <summary>
        /// Tell the database to save the sprite definitions.  Use this while you are creating your game.
        /// When you are done, you will usually want to take your sprite definition file and add it to the
        /// resources of your game.  The resources cannot be saved to, so you cannot continue to add new sprites
        /// once you are loading and saving them from a resources file.  But, the resources file is included with
        /// the program when you build it.
        /// </summary>
        public void Save()
        {
            if(!DoesResourceExist(Filename))
            {
                //we will try to save it as a file
                try
                {
                    WriteToXmlFile<List<SpriteInfo>>(Filename, SpriteInfoList);
                }
                catch (Exception e)
                {
                    throw new Exception("SpriteDatabase failed to save: Filename:" + Filename +"\n" + "ERROR: " + e.ToString(), e);
                }
            }
        }

        /// <summary>
        /// Change the Icon for the SpriteEntryForm
        /// </summary>
        /// <param name="toSet">An icon image</param>
        public void SetIcon(System.Drawing.Icon toSet)
        {
            LibIcon = toSet;
        }

        /// <summary>
        /// The SnapGrid is the block-size that your sprite will be.  For example, I will often have sprites with
        /// a snapgrid of 50,50.  This means that the sprite can be 50x50, 100x50, or anything with a step-size
        /// specified in the snap-grid.  It takes a "Size" specified by System.Drawing.Size.
        /// </summary>
        /// <param name="GridSize">The size of the grid space to snap to when dragging</param>
        public void SetSnapGridSize(Size GridSize)
        {
            if (GridSize.Width <= 0) return;
            if (GridSize.Height <= 0) return;
            if (GridSize.Width > 500) return;
            if (GridSize.Height > 500) return;
            SnapGridSize = GridSize;
        }

        //*******************************
        //****  Sprite Info Functions ***
        //*******************************
        #region SpriteInfo Functions
        void LoadSpriteInfo()
        {
            if (DoesResourceExist(Filename))
            {
                //This clears out the old list, as it gets replaced.
                SpriteInfoList = LoadObjectFromXmlFile<List<SpriteInfo>>(Filename, myResourceManager);
            }
            else
            {
                //try loading it from an actual filename
                if (File.Exists(Filename))
                    SpriteInfoList = ReadFromXmlFile<List<SpriteInfo>>(Filename);
            }
            //If neither works, we end up with an empty file.
            //If it fails, SpriteInfoList is null and things explode.
            if (SpriteInfoList == null)
                SpriteInfoList = new List<SpriteInfo>(); //make an empty one so things do not explode.
        }

        /// <summary>
        /// Return a list of the SpriteNames that this Database knows how to create.
        /// </summary>
        /// <returns>A list of strings, each one is the name of a sprite</returns>
        public List<string> SpriteNames()
        {
            List<string> theNames = new List<string>();
            foreach (SpriteInfo si in SpriteInfoList)
            {
                theNames.Add(si.SpriteName);
            }
            return theNames;
        }

        internal bool DoesResourceExist(string resourcename)
        {
            if (myResourceManager == null) return false;
            if (myResourceManager.GetObject(resourcename) != null)
                return true;
            return false;
        }

        /// <summary>
        /// Open a Sprite Edit Window.  This window does not let you draw a sprite.  What it does is to help
        /// you define your sprites and makes the process of using Sprites in your program a lot easier.
        /// </summary>
        /// <example>
        /// This is an example of how to use a SpriteDatabase.
        /// When you begin developing your project, you want to start by creating a SpriteDatabase and pointing
        /// it to a file, and then opening up an EditorWindow.
        /// <code lang="C#">
        /// public partial class MyGameForm : Form
        /// {
        ///     SpriteController mySpriteController = null;
        ///     SpriteDatabase mySpriteDatabase = null;
        ///     
        ///     public MyGameForm()
        ///     {
        ///         InitializeComponent();
        ///         MainDrawingArea.BackgroundImage = Properties.Resources.Background;
        ///         MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;
        ///         
        ///         string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        ///         string MyFile = Path.Combine(Desktop, "myFile.xml");
        ///         mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, MyFile);
        ///         
        ///         mySpriteController = new SpriteController(MainDrawingArea, mySpriteDatabase);
        ///         
        ///         mySpriteDatabase.OpenEditWindow();
        ///         mySpriteDatabase.Save();
        ///     }
        /// }
        /// </code>
        /// The Editor Window will let you find the sprites that are contained in the various images you have
        /// as resources in your program, and it will save a file with those sprite templates.  Any SpriteController
        /// that you have instantiated with a Sprite Database (see <see cref="SpriteController(PictureBox, SpriteDatabase)"/>)
        /// will now be able to create named sprites from the templates defined in the database.  After the first use, the
        /// named sprites will be accessible from within that controller just like any other named sprites.
        /// <para/>
        /// After you have created your SpriteDatabase file, you will want to add your file to your program resources.
        /// Then, you will change the SpriteDatabase to use the resource instead of a file.  If we named the file
        /// "MySpriteDatabase.xml", and it got added to your resources with the name "MySpriteDatabase", you would
        /// pass "MySpriteDatabase" to the database instantiation.
        /// <code lang="C#">
        /// public partial class MyGameForm : Form
        /// {
        ///     SpriteController mySpriteController = null;
        ///     SpriteDatabase mySpriteDatabase = null;
        ///     
        ///     public MyGameForm()
        ///     {
        ///         InitializeComponent();
        ///         MainDrawingArea.BackgroundImage = Properties.Resources.Background;
        ///         MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;
        ///         
        ///         //string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        ///         //string MyFile = Path.Combine(Desktop, "myFile.xml");
        ///         //mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, MyFile);
        ///         mySpriteDatabase = new SpriteDatabase(Properties.Resources.ResourceManager, "MySpriteDatabase");
        ///         
        ///         mySpriteController = new SpriteController(MainDrawingArea, mySpriteDatabase);
        ///         
        ///         //mySpriteDatabase.OpenEditWindow();
        ///         //mySpriteDatabase.Save();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="FirstItemIndex"></param>
        public void OpenEditWindow(int FirstItemIndex=-1)
        {
            SpriteEntryForm SEF = new SpriteEntryForm(this, SpriteInfoList, SnapGridSize);
            SEF.SetInitialSprite(FirstItemIndex);
            if (LibIcon != null) SEF.SetIcon(LibIcon);
            SEF.ShowDialog();
            //Use the updated list returned from the form.
            SpriteInfoList.Clear();
            SpriteInfoList.AddRange(SEF.GetUpdatedList());
        }

        /// <summary>
        /// Generate a new, named sprite from a sprite template stored in the database.  Most of the time you do
        /// not want to use this yourself.  SpriteControllers that are defined with a database will automatically
        /// look up sprite templates that they do not have sprites for.  This function is just a wrapper for SmartDuplicateSprite.    
        /// </summary>
        /// <param name="Name">The name of the sprite to load.  Names are case-sensitive.</param>
        /// <param name="ControllerToUse">The sprite controller that will store the sprite in its cache</param>
        /// <returns>A new, named sprite, or null if no such template is found.</returns>
        public Sprite SpriteFromName(string Name, SpriteController ControllerToUse)
        {
            return SmartDuplicateSprite(ControllerToUse, Name, true);
        }
        #endregion
        
        #region General Functions
        /// <summary>
        /// This function returns an image from the Properties.Resources.  If we tell it to UseSmartImages, then
        /// it caches the image in memory.  This makes it a little faster to return.  If you have a lot of sprites
        /// to load, using this system can speed up things a fair bit.  But, try to remember not to change the
        /// image that this returns unless you duplicate it first.  Otherwise you will end up changing the image
        /// for all the other times you reference it.  This is usualy a bad thing.
        /// </summary>
        /// <param name="Name">The string name of the image.  If your image is usually named
        /// Properties.Resources.mySpriteImage, you will want to have "mySpriteImage" as the Name passed
        /// to GetImageFromName</param>
        /// <param name="UseSmartImages">A parameter stating whether we should cache the image in memory
        /// or simply retrieve it from the resource manager.</param>
        /// <returns>The resource image with the specified name</returns>
        public Image GetImageFromName(string Name, bool UseSmartImages)
        {
            Image MyImage = null;
            if (UseSmartImages)
            {
                foreach (ImageStruct IS in TheImages)
                {
                    if (IS.ImageName.Equals(Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        MyImage = IS.TheImage;
                        break;
                    }
                }
            }
            if (MyImage == null)
            {
                ResourceManager rm = myResourceManager;
                MyImage = (Bitmap)rm.GetObject(Name);
                if (UseSmartImages)
                {
                    ImageStruct NewIS = new ImageStruct();
                    NewIS.ImageName = Name;
                    NewIS.TheImage = MyImage;
                    TheImages.Add(NewIS);
                }
            }
            return MyImage;
        }


        /// <summary>
        /// Return a list of the image names in the Properties.Resources
        /// </summary>
        /// <returns>A list of image names in the Properties.Resources</returns>
        public List<string> GetImageNames()
        {
            List<string> Names = new List<string>();
            if (myResourceManager == null) return Names;
            ResourceSet Rs = myResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry entry in Rs)
            {
                string resourceKey = entry.Key.ToString(); //The name
                object resource = entry.Value; //The object itself
                if (resource is Image) Names.Add(resourceKey);
            }
            return Names;
        }
            
        /// <summary>
        /// This code is mostly handled by the sprite controller.  If the SpriteController has a SpriteDatabase
        /// registered, then it will automatically ask the SpriteDatabase to create any sprite it does not already
        /// have.
        /// </summary>
        /// <param name="theController">The controller that will manage the newly created Sprite</param>
        /// <param name="SpriteName">The name of the sprite to look up and then create</param>
        /// <param name="UseSmartImages">Whether or not we should cache images to give a very small increase in speed</param>
        /// <returns></returns>
        internal Sprite SmartDuplicateSprite(SpriteController theController, string SpriteName, bool UseSmartImages = true)
        {
            Sprite DestSprite = theController.SpriteFromNameInternal(SpriteName);
            if (DestSprite != null) return new Sprite(DestSprite);

            //If it does not exist, make it
            foreach (SpriteInfo SI in SpriteInfoList)
            {
                if (SI.SpriteName == SpriteName)
                {
                    SI.CreateSprite(theController, this);
                    return theController.DuplicateSprite(SpriteName);
                }
            }
            return null;
        }
        #endregion

        #region Generic XML Funcs

        /// <summary>
        /// Load in an XML serialized item from the specified ResourceManager.  You will usually make an XML file by
        /// creating an object (as a variable) and using <see cref="WriteToXmlFile{T}(string, T)"/> to
        /// serialize it and save it to a file on your desktop.  Then you can drag and drop that file into your project and then use this 
        /// LoadObjectFromXmlFile function.  You can google XML Serialization for more information.
        /// </summary>
        /// <example>
        /// XML Serialization takes an object (a class, a variable, or whatever) and will store any public values in XML.
        /// You can choose to save the resulting XML as a string, or to save it to a file.  This function Loads it from a 
        /// resource file (one which has been added to Properties.Resources.)  The corresponding write function: 
        /// <see cref="WriteToXmlFile{T}(string, T)"/> writes to a file that is outside of Properties.Resources; the
        /// resources of a program are read-only.  Once you write to a file, you can drag the resulting XML into your project
        /// and load it from there.  If you want to load from an XML file that is not a resource, use <see cref="ReadFromXmlFile{T}(string)"/>
        /// <para/>Here is code to create an item and save it to a file.
        /// <code Lang="C#">
        ///     MyClass MyVariable = new MyClass();
        ///     MyVariable.Name = "StoreThis!";
        ///     
        ///     SpriteDatabase.WriteToXmlFile&lt;MyClass&gt;("c:\xml_file.xml", MyClass);
        /// </code>
        /// Now that we have an XML file, we drag that file into our project so that it shows up in our Properties.Resources
        /// and then we can use this code to load it.
        /// <code Lang="C#">
        ///     MyClass MyVariable = SpriteDatabase.LoadObjectFromXmlFile&lt;MyClass&gt;("xml_file",Properties.Resources.ResourceManager);
        ///     Console.WriteLine(MyVariable.Name);
        /// </code>
        /// </example>
        /// <typeparam name="T">The type of object to load.  It could be something as simple as an int, a class, or a list of classes.</typeparam>
        /// <param name="XMLResourceToLoad">The resource item to load.  If you would access it like: properties.resources.myFile,
        /// the correct value to put here would be "myFile"</param>
        /// <param name="MyManager">The resource manager.  Usually Properties.Resources.ResourceManager</param>
        /// <returns>An object of the value you specified.  Or null if it fails.</returns>
        public static T LoadObjectFromXmlFile<T>(string XMLResourceToLoad, ResourceManager MyManager) where T : new()
        {
            //Load in the sprite data
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            // Retrieves String and Image resources.
            object titem = MyManager.GetObject(XMLResourceToLoad);
            byte[] item = (byte[])System.Text.Encoding.UTF8.GetBytes((string)titem);

            try
            {
                return (T)serializer.Deserialize(new MemoryStream(item));
            }
            finally
            {

            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// Only Public properties and variables will be written to the file. These can be any type though, even other classes.
        /// If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.
        /// Object type must have a parameterless constructor.
        /// </summary>
        /// <example>
        /// XML Serialization takes an object (a class, a variable, or whatever) and will store any public values in XML.
        /// You can choose to save the resulting XML as a string, or to save it to a file.  This function 
        /// writes to a file that is outside of Properties.Resources; the
        /// resources of a program are read-only.  Once you write to a file, you can drag the resulting XML into your project
        /// and load it from there.  If you want to load from an XML file that is not a resource, use <see cref="ReadFromXmlFile{T}(string)"/>
        /// <para/>Here is code to create an item and save it to a file.
        /// <code Lang="C#">
        ///     MyClass MyVariable = new MyClass();
        ///     MyVariable.Name = "StoreThis!";
        ///     
        ///     SpriteDatabase.WriteToXmlFile&lt;MyClass&gt;("c:\xml_file.xml", MyClass);
        /// </code>
        /// Now that we have an XML file, we can use this code to load it.
        /// <code Lang="C#">
        ///     MyClass MyVariable = SpriteDatabase.ReadFromXmlFile&lt;MyClass&gt;("c:\xml_file.xml");
        ///     Console.WriteLine(MyVariable.Name);
        /// </code>
        /// </example>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <example>
        /// XML Serialization takes an object (a class, a variable, or whatever) and will store any public values in XML.
        /// You can choose to save the resulting XML as a string, or to save it to a file.  This function 
        /// reads in a file that probably has been written by <see cref="SpriteDatabase.WriteToXmlFile{T}(string, T)"/>.
        /// <code Lang="C#">
        ///     MyClass MyVariable = new MyClass();
        ///     MyVariable.Name = "StoreThis!";
        ///     
        ///     SpriteDatabase.WriteToXmlFile&lt;MyClass&gt;("c:\xml_file.xml", MyClass);
        /// </code>
        /// Now that we have an XML file, we can use this code to load it.
        /// <code Lang="C#">
        ///     MyClass MyVariable = SpriteDatabase.ReadFromXmlFile&lt;MyClass&gt;("c:\xml_file.xml");
        ///     Console.WriteLine(MyVariable.Name);
        /// </code>
        /// </example>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// This is a generic function which the SpriteDatabase uses.  It does XML Serialization of most anything,
        /// and generates an XML String.  XML Serialization will take any public value of a public class and 
        /// make an XML entry for it.  It is a very convienent way to save data.  You can "Deserialize" the value
        /// with the <see cref="SpriteDatabase.ReadFromXmlString{T}(string)">ReadFromXMLString</see> function.
        /// </summary>
        /// <typeparam name="T">The type of the item that you are trying to serialize</typeparam>
        /// <param name="toSerialize">the variable you are trying to turn into XML</param>
        /// <returns>An XML string</returns>
        public static string WriteToXMLString<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// This is a generic function which the SpriteDatabase uses.  It does XML Deserialization of most anything,
        /// and generates an XML String.  XML Serialization will take any public value of a public class and 
        /// make an XML entry for it.  It is a very convienent way to save and retrieve data.  You can "Serialize" the value
        /// with the <see cref="SpriteDatabase.WriteToXMLString{T}(T)">WriteToXMLString</see> function.
        /// </summary>
        /// <typeparam name="T">The type of the item that you are trying to deserialize</typeparam>
        /// <param name="toDeserialize">an XML string, of something you serialized previously</param>
        /// <returns>An object of type T</returns>
        public static T ReadFromXmlString<T>(string toDeserialize) where T : new()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(toDeserialize))
                return (T)xmlSerializer.Deserialize(textReader);
        }

        /// <summary>
        /// This is an inefficient, but simple function to clone a class.  It works by serializing an item
        /// to a string, and then deserializing it into a class.  The end result is that any value which is
        /// publically visible is duplicated, but it is a completely separate class from the original.
        /// </summary>
        /// <typeparam name="T">The type of the item to clone</typeparam>
        /// <param name="ObjectToClone">The actual object to clone</param>
        /// <returns>A duplicate of the original</returns>
        public static T CloneByXMLSerializing<T>(T ObjectToClone)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            string dest;
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToClone);
                dest = textWriter.ToString();
            }

            using (StringReader textReader = new StringReader(dest))
                return (T)xmlSerializer.Deserialize(textReader);
        }
        #endregion
    }

}
