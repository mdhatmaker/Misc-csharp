using System;

namespace AsyncDialog {

    public partial class ModalDlg : AsyncBaseDialog {

        public ModalDlg() {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e) {
            AsyncProcessDelegate d = delegate() {
                System.Threading.Thread.Sleep(3000);
            };

            AsyncProcessDelegate db = delegate() {
                System.Threading.Thread.Sleep(9000);
            };

            RunAsyncOperation(d);
            RunAsyncOperation(d);
            RunAsyncOperation(db);
        }

    }//class

}//namespace
