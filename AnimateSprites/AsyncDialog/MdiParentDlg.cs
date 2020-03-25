using System;
using System.Windows.Forms;

namespace AsyncDialog {

    public partial class MdiParentDlg : Form {

        public MdiParentDlg() {
            InitializeComponent();

            new MdiChildDlg(this).Show();

            new ModalDlg().Show(this);
        }

    }//class

}//namespace
