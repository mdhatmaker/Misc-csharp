/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
//Imports DxVBLib

namespace LibMidi
{
    // Basic Wave and MIDI Player Class for .net 1.1
    // Requires COM-Object : DirectX7 

    public class SoundPlayer
    {
        // MCI INterface
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command,
           StringBuilder returnValue, int returnLength,
           IntPtr winHandle);

        private string _file;
        private DirectX7 m_dx = new DirectX7();
        private DirectSound m_ds;

        public string FileName { get { return _file; } }

        public bool PlaySound()
        {
            if (InitAudio() == true)
                if (PlayFile() == true)
                    return true;
                else
                    return false;
            else
                // Audio hardware not found
                return false;
        }

        public bool StopSound()
        {
            return StopFile();
        }

        private bool PlayFile()
        {
            long lRet;

            var empty = new StringBuilder(String.Empty);

            try
            {
                StopFile();
                lRet = mciSendString("open " + _file + " alias track", empty, 0, 0);
                lRet = mciSendString("play track", empty, 0, 0);
                return (lRet == 0);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public void CloseAudio()
        {
            mciSendString("close all", 0, 0, 0);
        }

        private bool StopFile()
        {
            long lRet;

            var empty = new StringBuilder(String.Empty);

            try
            {
                lRet = mciSendString("stop track", empty, 0, 0);
                lRet = mciSendString("close track", empty, 0, 0);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool InitAudio()
        {
            try
            {
                m_ds = m_dx.DirectSoundCreate("");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }

}
*/