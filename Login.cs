using OOADCafeShopManagement.Models;
using System;
using System.Windows.Forms;

namespace OOADCafeShopManagement
{
    public partial class frmLogin : Form
    {
        private readonly Login login;

        public frmLogin()
        {
            InitializeComponent();
            login = new Login(this);
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            RegisterForm frmRegister = new RegisterForm();
            frmRegister.Show();
            this.Hide();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            isValid valid = new isValid();
            if (valid.isValidLogin(username, password))
            {
                valid.alertLogin(username, password);
                login.AuthenticateUser(username, password);
            }
        }
    }
}
