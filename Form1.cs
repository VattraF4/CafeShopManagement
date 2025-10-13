using System;
using System.Collections.Generic;
using System.ComponentModel;
//Database Connection
using System.Data;
using System.Data.SqlClient;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace OOADCafeShopManagement
{
    public partial class frmLogin : Form
    {
        SqlConnection connection = new SqlConnection
            (@"Data Source=DESKTOP-C39HSFU\SQLEXPRESS;
            Initial Catalog=cafe;
            Integrated Security=True;
            TrustServerCertificate=True");
        

        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-C39HSFU\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True");
        public frmLogin()
        {
            InitializeComponent();

       
        }


        private void btnSignUp_Click(object sender, EventArgs e)
        {

            RegisterForm frmRegister = new RegisterForm();
            frmRegister.Show();

            this.Hide(); // hide current form

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
            string password = txtPassword.Text.Trim();
            string username = txtUsername.Text.Trim();

            isValid valid = new isValid();
            if (valid.isValidLogin(username, password))
                valid.alertLogin(username,password);


            try
            {

                // Step 2: Check if username exists
                string queryUsername = "SELECT password FROM users WHERE username = @username";
                using (SqlCommand cmdUser = new SqlCommand(queryUsername, connection))
                {
                    connection.Open();
                    cmdUser.Parameters.AddWithValue("@username", txtUsername.Text.Trim());

                    object result = cmdUser.ExecuteScalar(); //use to get a single value from first colomn

                    if (result == null)
                    {
                        // Username not found
                        MessageBox.Show("Username does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Step 3: If username exists, compare passwords
                    string storedHashed = result.ToString();
                    string enterHashed = SecurityHelper.HashPassword(txtPassword.Text.Trim());

                    if (storedHashed == enterHashed)
                    {
                        MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // TODO: Redirect to dashboard
                        this.Hide();
                        frmDashboard dashboard = new frmDashboard();
                        dashboard.Show();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                // Always close the connection after using it
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
    }
}
