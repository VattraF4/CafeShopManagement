using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OOADCafeShopManagement.Observer;

namespace OOADCafeShopManagement
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize Observer Pattern - Setup all observers
            InitializeObservers();

            Application.Run(new frmLogin());
            //Application.Run(new Profile());
        }

        /// <summary>
        /// Initialize Observer Pattern for Order Events
        /// This sets up all observers to listen to order events
        /// </summary>
        private static void InitializeObservers()
        {
            var orderSubject = OrderSubject.Instance;

            // Attach all observers
            orderSubject.Attach(new DashboardObserver());
            orderSubject.Attach(new NotificationObserver("System"));
            orderSubject.Attach(new InventoryObserver());
            orderSubject.Attach(new AnalyticsObserver());
            orderSubject.Attach(new LoggingObserver());
        }
    }
}
