using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using LibMidi;
using System.IO;

// https://www.codeguru.com/columns/dotnet/making-music-with-midi-and-c.html

// Video Game MIDI files at:   https://vgmusic.com/

namespace MidiConsole
{
    
    class Program
    {
        
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: playmidi filename.mid\n");
                return;
            }

            //string folder = FileTools.GetParent(FileTools.AssemblyDirectory, 3);
            //string midiFolder = Path.Combine(folder, "midi_files");
            //string filename = "Arcanoide.mid";
            string path = args[0];
            string filename = Path.GetFileName(path);
            string midiFolder = Path.GetDirectoryName(path);
            Console.WriteLine("'{0}'  '{1}'", midiFolder, filename);

            string pathname = Path.Combine(midiFolder, filename);
            if (File.Exists(pathname))
            {
                var midi = new Midi();
                midi.PlayMidi(filename, midiFolder);
            }
            else
            {
                Console.WriteLine("File not found: '{0}'", pathname);
            }

            //Console.Write("Press any key ... ");
            //Console.ReadLine();
        }

    } // end of class Program

} // end of namespace