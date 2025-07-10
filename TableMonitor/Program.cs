using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableMonitor.Class;

namespace TableMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Here, you need to pass a guid or any other unique string.
            if (!SingleInstance.Start("0f03c714-b597-4c17-a351-62qw5115512a"))
            {
                MessageBox.Show("Application is already running, Please call the vendor for support.");
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.FrmHome());

            // mark single instance as closed
            SingleInstance.Stop();
        }
    }
}
