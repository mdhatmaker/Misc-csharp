using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AnimateSprites
{
    public class AnimSprite
    {
        private int frame, interval, width, height;
        private string imgFile;
        private Image img;
        private Timer frameTimer;

        public AnimSprite(string f_imgFile, int f_width)
        {
            frame = 0;
            width = f_width;
            imgFile = f_imgFile;

            img = new Bitmap(imgFile);
            height = img.Height;
        }

        public void Start(int f_interval)
        {
            interval = f_interval;

            frameTimer = new Timer();
            frameTimer.Interval = interval;
            frameTimer.Tick += new EventHandler(advanceFrame);
            frameTimer.Start();
        }

        public void Start()
        {
            Start(100);
        }

        public void Stop()
        {
            frameTimer.Stop();
            frameTimer.Dispose();
        }

        public Bitmap Paint(Graphics e)
        {
            Bitmap temp;
            Graphics tempGraphics;

            temp = new Bitmap(width, height, e);
            tempGraphics = Graphics.FromImage(temp);

            tempGraphics.DrawImageUnscaled(img, 0 - (width * frame), 0);

            tempGraphics.Dispose();
            return (temp);
        }

        private void advanceFrame(Object sender, EventArgs e)
        {
            frame++;
            if (frame >= img.Width / width)
                frame = 0;
        }
    } // end of class AnimSprite
} // end of namespace