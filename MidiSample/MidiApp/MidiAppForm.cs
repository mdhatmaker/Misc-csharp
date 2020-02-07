using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibMidi;

namespace MidiApp
{
    public partial class FormMidiApp : Form
    {
        Midi _midi;

        public FormMidiApp()
        {
            InitializeComponent();

            txtFolder.Text = Path.Combine(FileTools.GetParent(FileTools.AssemblyDirectory, 3), "midi_files");

            _midi = new Midi();
        }

        void Status(string msg)
        {
            statusLabel.Text = msg;
        }

        void PopulateFileList()
        {
            listMidiFiles.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(txtFolder.Text);
            try
            {
                FileInfo[] Files = d.GetFiles("*.mid"); //Getting Midi files
                foreach (FileInfo file in Files)
                {
                    listMidiFiles.Items.Add(file.Name);
                }
                if (listMidiFiles.Items.Count > 0) listMidiFiles.SelectedIndex = 0;
                Status("Ok.");
            }
            catch (Exception ex)
            {
                Status(ex.Message);

            }
        }

        void PlayMidi(string filename, string folder)
        {
            //_midi.PlayNote();
            Status(string.Format("MIDI: playing '{0}'", filename));
            _midi.PlayMidi(filename, folder);
        }

        void StopMidi()
        {
            _midi.StopMidi();
            Status("");
        }

        private void FormMidiApp_Load(object sender, EventArgs e)
        {
            PopulateFileList();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (listMidiFiles.SelectedItem == null) return;
            PlayMidi(listMidiFiles.SelectedItem.ToString(), txtFolder.Text);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMidi();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateFileList();
        }
    } // end of class MidiAppForm
} // end of namespace
