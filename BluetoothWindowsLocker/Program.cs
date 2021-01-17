using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluetoothWindowsLocker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var bluetoothLock = new BluetoothLock();
            try
            {
                bluetoothLock.Initialize(args);
            }
            catch (Exception e) {
                MessageBox.Show(@"Ooops.", string.Format(
        "Looks like you've found a bug. Technical info: {0}: {1}{2}{3}",
        e.GetType(), e.Message, Environment.NewLine, e.StackTrace));
                return;
            }


            if (bluetoothLock.IsHidden)
            {
                Application.Run();
            }
            else
            {
                Application.Run(bluetoothLock);
            }
        }
    }
}
