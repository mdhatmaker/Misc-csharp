using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using DumpFileLib;

namespace DumpFileViewer
{
    public partial class MainForm : Form
    {
        private InstrumentSymbolLookup _lookup;

        public MainForm()
        {
            InitializeComponent();

            DumpReports.PrintMethod = this.Print;
            DumpReports.PrintLineMethod = this.PrintLine;

            _lookup = InstrumentSymbolLookup.Load();
            updateInstrumentSymbolList(_lookup);
        }

        private void btnIidsFromClipboard_Click(object sender, EventArgs e)
        {
            var lookup = DumpFile.GetSymbolsFromClipboard();

            if (lookup == null || lookup.Symbols.Count == 0)
            {
                MessageBox.Show("No valid clipboard data available.", "Instrument Ids from Clipboard");
            }
            else
            {
                updateInstrumentSymbolList(lookup);
                lookup.Save();
            }
        }

        private void btnImportIidFile_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = openFileDlg.ShowDialog();

            if (dlgResult == DialogResult.OK)
            {
                var lookup = DumpFile.GetSymbolsFromTextFile(openFileDlg.FileName);

                if (lookup == null || lookup.Symbols.Count == 0)
                {
                    MessageBox.Show("No valid InstrumentId/Symbol data available in selected file.", "Instrument Ids from Text File");
                }
                else
                {
                    updateInstrumentSymbolList(lookup);
                    lookup.Save();
                }
            }

        }

        private void updateInstrumentSymbolList(InstrumentSymbolLookup lookup)
        {
            listViewInstruments.Items.Clear();

            string[] subitems = new string[2];
            foreach (uint iid in lookup.Symbols.Keys)
            {
                subitems[1] = lookup.Symbols[iid];
                var item = new ListViewItem(subitems);
                item.Text = iid.ToString();

                listViewInstruments.Items.Add(item);
            }
        }

        private void btnOpenDumpFile_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = openFileDlg.ShowDialog();

            if (dlgResult == DialogResult.OK)
            {
                txtDump.Text = "";

                DumpFile dump = new DumpFile(openFileDlg.FileName);
                dump.Lookup = _lookup;

                string outputFilename = @"C:\Users\mhatmaker\Desktop\Dump 101\dumpfile." + DumpFile.DateTimeNumber + ".txt";

                DumpReports.DepthBidAskSpread(dump, outputFilename, 6);
            }
        }

        public void Print(string format, params string[] data)
        {
            Console.Write(format, data);
            txtDump.AppendText(string.Format(format, data));
        }

        public void PrintLine(string format, params string[] data)
        {
            Console.WriteLine(format, data);
            txtDump.AppendText(string.Format(format + "\n", data));
        }

        /*public void Print(string text)
        {
            Console.Write(text);
            txtDump.AppendText(text);
        }

        public void PrintLine(string text)
        {
            Console.WriteLine(text);
            txtDump.AppendText(text + "\n");
        }*/
    } // END OF CLASS
} // END OF NAMESPACE
