using System;
using System.Windows.Forms;

namespace AsyncDialog {

    public partial class MdiChildDlg : AsyncBaseDialog {

        public MdiChildDlg(Form parent) {
            InitializeComponent();
            MdiParent = parent;

            //BeginAsyncIndication();
        }

        private void button3_Click(object sender, EventArgs e) {
            BeginAsyncIndication();
        }

    }//class

}//namespace