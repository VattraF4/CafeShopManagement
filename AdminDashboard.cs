using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOADCafeShopManagement
{
    public partial class frmDashboard : Form
    {
        //Config for moving form
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public frmDashboard()
        {
            InitializeComponent();
            MakeDraggable(pnlTitleBar);
            MakeDraggable(lbTitle);
            // Attach events to lbX label
            lbX.MouseEnter += lbX_MouseEnter;
            lbX.MouseLeave += lbX_MouseLeave;
            lbX.Click += lbX_Click;
        }

        //use to move window form by properties tools or control box
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
        private void lbX_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lbX_MouseEnter(object sender, EventArgs e)
        {
            lbX.ForeColor = Color.Red;
        }
        private void lbX_MouseLeave(object sender, EventArgs e)
        {
            lbX.ForeColor = Color.White; // Change to your original color
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Dashboard adminDashboard = new Dashboard();
            adminDashboard.Show();
            this.Hide();
        }
    }
}
