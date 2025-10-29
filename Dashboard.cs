using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OOADCafeShopManagement.Models;


namespace OOADCafeShopManagement
{
    public partial class frmDashboard : Form
    {

        private Dashboard model;
        //Config for moving form
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        public frmDashboard()
        {
            InitializeComponent();
            MakeDraggable(pnlDashboard);
            MakeDraggable(pnlMenu);
            MakeDraggable(pnlSepBar);


        }
        private void MakeDraggable(Control control)
        {
            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, 0xA1, 0x2, 0);
                }
            };
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);  
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            frmMainDashboard mainDashboard = new frmMainDashboard();
            mainDashboard.Show();
            this.Hide();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
