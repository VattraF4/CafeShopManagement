using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace OOADCafeShopManagement.Models
{
    public class LoginHelper : DbConnection
    {
        private frmLogin _loginForm;
        public LoginHelper(frmLogin loginForm) {
            _loginForm = loginForm;
        }
        public void AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    string query = @"SELECT id, username, password, role, status, profile_img
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

                                bool isMatch = SecurityHelper.VerifyPassword(password, storedHashed);

                                if (isMatch)
                                {
                                    int userId = Convert.ToInt32(reader["id"]);
                                    string userRole = reader["role"].ToString();
                                    string userName = reader["username"].ToString();
                                    string profilePath = reader["profile_img"].ToString();

                                    // Initialize user session
                                    UserSession.Initialize(userId, userName, userRole, profilePath);

                                    MessageBox.Show($"Login successful! Welcome {userName}",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Open next form
                                    Profile frmProfile = new Profile();
                                    frmProfile.Show();

                                    //close login form 
                                    _loginForm.Hide();
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
            }
        }
    }
}
