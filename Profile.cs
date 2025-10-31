using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OOADCafeShopManagement
{
    public partial class Profile : Form
    {
        //Config for moving form
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        
        public Profile()
        {
            InitializeComponent();
            MakeDraggable(pnlSepBar);
            MakeDraggable(pnlMenu);
            MakeDraggable(lblTitle);
            // Attach events to lbX label
            lbX.MouseEnter += lbX_MouseEnter;
            lbX.MouseLeave += lbX_MouseLeave;
            lbX.Click += lbX_Click;
            Display();
        }

        //use to move window form by properties tools or control box
        private void MakeDraggable(Control control)
        {
            Display();
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
            Environment.Exit(0);
            //this.Close();
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
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.Show();

        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            frmDashboard frmDashboard = new frmDashboard();
            this.Hide();
            frmDashboard.Show();

        }
        public void Display()
        {
            //picCurrentUser.Image = Image.FromFile(UserSession.ProfilePath);

            if (GetUsername() != null)
            {
                lblUsername.Text = GetUsername();

            }
            if (GetUserRole() != "admin")
            {
                //hide button
                btnCustomers.Visible = false;
                btnProducts.Visible = false;
                btnCashiers.Visible = false;
                btnProducts.Visible = false;
            }
        }
        public string GetUsername()
        {
            return UserSession.Username;
        }
        public string GetUserRole()
        {
            return UserSession.Role;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutUser();
        }
        private void LogoutUser()
        {
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to logout, {UserSession.Username}?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                

                // Clear user session
                UserSession.Logout();

                // Show logout success message
                MessageBox.Show("You have been logged out successfully.", "Logout",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Redirect to login form
                RedirectToLogin();
            }
        }
        
        private void RedirectToLogin()
        {
            frmLogin loginForm = new frmLogin();
            loginForm.Show();
            this.Close(); // Close dashboard
        }
    }
}
