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
    public partial class frmLogin : Form
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True";

        public frmLogin()
        {
            InitializeComponent();
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
            string password = txtPassword.Text.Trim();
            string username = txtUsername.Text.Trim();

            isValid valid = new isValid();
            if (valid.isValidLogin(username, password))
                valid.alertLogin(username, password);

            AuthenticateUser(username, password);
        }

        private void AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // FIXED: Query to get ALL user details including ID and role
                    string query = @"SELECT id, username, password, role, status , profile_img
                                   FROM users 
                                   WHERE username = @username AND status = @status";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@status", "Active");

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashed = reader["password"].ToString();
                                string enterHashed = SecurityHelper.HashPassword(password);

                                if (storedHashed == enterHashed)
                                {
                                    // SUCCESSFUL LOGIN - Get user details from reader
                                    int userId = Convert.ToInt32(reader["id"]);
                                    string userRole = reader["role"].ToString();
                                    string userName = reader["username"].ToString();
                                    string profilePath = reader["profile_img"].ToString();

                                    // Initialize UserSession
                                    UserSession.Initialize(userId, userName, userRole,profilePath);

                                    MessageBox.Show($"Login successful! Welcome {userName}", "Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    this.Hide();
                                    frmDashboard dashboard = new frmDashboard();
                                    dashboard.Show();
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect password!", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username does not exist or account is inactive!", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // Connection automatically closed by using statement
            }
        }
    }
}