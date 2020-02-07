using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

// https://www.codeguru.com/columns/dotnet/making-music-with-midi-and-c.html

// Video Game MIDI files at:   https://vgmusic.com/


namespace LibMidi
{
    public class Midi
    {
        // MCI INterface
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command,
           StringBuilder returnValue, int returnLength,
           IntPtr winHandle);

        // Midi API
        [DllImport("winmm.dll")]
        private static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int midiOutGetDevCaps(Int32 uDeviceID,
           ref MidiOutCaps lpMidiOutCaps, UInt32 cbMidiOutCaps);

        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(ref int handle,
           int deviceID, MidiCallBack proc, int instance, int flags);

        [DllImport("winmm.dll")]
        private static extern int midiOutShortMsg(int handle,
           int message);

        [DllImport("winmm.dll")]
        private static extern int midiOutClose(int handle);

        private delegate void MidiCallBack(int handle, int msg, int instance, int param1, int param2);

        int _handle = 0;
        //MidiOutCaps _myCaps;

        private string Mci(string command)
        {
            StringBuilder reply = new StringBuilder(256);
            mciSendString(command, reply, 256, IntPtr.Zero);
            return reply.ToString();
        }

        public void PlayMidi(string filename, string folder)
        {
            var res = String.Empty;

            //res = Mci("open \"M:\\anger.mid\" alias music");
            //var folder = "C:\\Users\\mhatm\\Downloads";
            //var filename = "Arcanoide.mid";
            res = Mci(string.Format("open \"{0}\\{1}\" alias music", folder, filename));
            res = Mci("play music");
            Console.WriteLine("MIDI: Playing '{0}'", filename);
            //res = Mci("close music");
        }

        public void StopMidi()
        {
            var res = Mci("stop music");
            res = Mci("close music");
        }

        public void CloseAudio()
        {
            var res = Mci("close all");
        }

        public void PlayNote()
        {
            byte command = 0x90;
            byte note = 0x3C;
            byte velocity = 0x7F;
            int message = (velocity << 16) + (note << 8) + command;
            //var res = midiOutShortMsg(handle, 0x007F3C90);
            //var res = midiOutShortMsg(_handle, 0x007F3C90);
            int res;
            res = midiOutOpen(ref _handle, 0, null, 0, 0);
            res = midiOutShortMsg(_handle, 0x000019C0);
            res = midiOutShortMsg(_handle, 0x007F3C90);
            res = midiOutClose(_handle);
        }

        public void CloseMidi()
        {
            /*var res = String.Empty;
            res = Mci("close crooner");*/

            var ires = midiOutClose(_handle);
        }

        public Midi()
        {
            var numDevs = midiOutGetNumDevs();
            Console.WriteLine("MIDI: You have {0} midi output devices", numDevs);

            /*//int handle = 0;

            var numDevs = midiOutGetNumDevs();
            //MidiOutCaps myCaps = new MidiOutCaps();
            _myCaps = new MidiOutCaps();

            var res = midiOutGetDevCaps(0, ref _myCaps, (UInt32)Marshal.SizeOf(_myCaps));

            //res = midiOutOpen(ref _handle, 0, null, 0, 0);
            //res = midiOutShortMsg(_handle, 0x000019C0);
            //res = midiOutShortMsg(_handle, 0x007F3C90);
            //res = midiOutClose(_handle);*/
        }

    } // end of class Midi
} // end of namespace
