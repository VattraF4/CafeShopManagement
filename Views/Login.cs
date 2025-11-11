using OOADCafeShopManagement.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOADCafeShopManagement
{
    public partial class frmLogin : Form
    {
        private readonly LoginHelper login;
        private IsValidationHelper isValid = new IsValidationHelper();

        public frmLogin()
        {
            InitializeComponent();
            login = new LoginHelper(this);
            lblAlertLogin.Text = "";
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

            // Check Valid is False or True
            var valid = isValid.isValidLogin(username, password);

            // show alert message
            var result = isValid.alertLogin(username, password);
            lblAlertLogin.ForeColor = result.isSuccess ? Color.Green : Color.Red;
            lblAlertLogin.Text = result.message;

            //If true authenticate user
            if (valid)
            {
                login.AuthenticateUser(username, password);
            }
        }
    }
}
