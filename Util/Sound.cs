using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Util
{
    public class Sound
    {
        // PlaySound()
        [DllImport("winmm.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern bool PlaySound(
            string pszSound,
            IntPtr hMod,
            SoundFlags sf);

        // Flags for playing sounds.  For this example, we are reading 
        // the sound from a filename, so we need only specify 
        // SND_FILENAME | SND_ASYNC
        [Flags]
        public enum SoundFlags : int
        {
            SND_SYNC = 0x0000,  // play synchronously (default) 
            SND_ASYNC = 0x0001,  // play asynchronously 
            SND_NODEFAULT = 0x0002,  // silence (!default) if sound not found 
            SND_MEMORY = 0x0004,  // pszSound points to a memory file
            SND_LOOP = 0x0008,  // loop the sound until next sndPlaySound 
            SND_NOSTOP = 0x0010,  // don't stop any currently playing sound 
            SND_NOWAIT = 0x00002000, // don't wait if the driver is busy 
            SND_ALIAS = 0x00010000, // name is a registry alias 
            SND_ALIAS_ID = 0x00110000, // alias is a predefined ID
            SND_FILENAME = 0x00020000, // name is file name 
            SND_RESOURCE = 0x00040004  // name is resource name or atom 
        }

        [DllImport("user32.dll")]
        private static extern int MessageBeep(uint n);

        [DllImport("kernel32.dll")]
        private static extern bool Beep(int Frequency, int Duration);

        public static void Beep()
        {
            MessageBeep(0x0);
        }

        public static void Tone(int frequency, int duration)
        {
            //TBD: This is crashing my system!!!
            //Beep(220, 250);
        }

        //-----------------------------------------------------------------
        static public void PlayWAV(string wfname, SoundFlags soundFlags)
        {
            PlaySound(wfname, IntPtr.Zero, soundFlags);
        }

        //-----------------------------------------------------------------
        static public void PlayWAV(string wfname)
        {
            PlaySound(wfname, IntPtr.Zero, SoundFlags.SND_FILENAME | SoundFlags.SND_ASYNC);
        }

        //-----------------------------------------------------------------
        /*static public void StopPlay()
        {
            PlaySound(null, SND_PURGE);
        }*/

    }   // Sound
}
