namespace Droid_trading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TradeMonitor());

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TS_Trading(args)
            };
            ServiceBase.Run(ServicesToRun);

        }
    }
}
