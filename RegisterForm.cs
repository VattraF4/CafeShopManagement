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

//Encryption
using System.Security.Cryptography;
using System.IO;

namespace OOADCafeShopManagement
{
    public partial class RegisterForm : Form
    {
        SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-C39HSFU\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True");

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.Show();

            this.Hide(); // use to hide current form
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
            txtConfirmPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            isValid valid = new isValid();
            if(!valid.isValidInputReg(username, password, confirmPassword))
            {
                valid.alertRegister(username, password, confirmPassword);
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-C39HSFU\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True"))
                {
                    connection.Open();

                    // Step 1: Check if username exists
                    string queryUsername = "SELECT password FROM users WHERE username = @username";
                    using (SqlCommand cmdUser = new SqlCommand(queryUsername, connection))
                    {
                        cmdUser.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        object result = cmdUser.ExecuteScalar();

                        if (result != null)
                        {
                            MessageBox.Show("Username already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Step 2: Insert new user with hashed password
                    string insertQuery = "INSERT INTO users (username, password, profile_img, role, status) " +
                                         "VALUES (@username, @password, 'profiles/default.png', 'user', 'Active')";

                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery, connection))
                    {
                        if (password == confirmPassword)
                        {
                            string hashedPassword = SecurityHelper.HashPassword(confirmPassword);
                            cmdInsert.Parameters.AddWithValue("@password", hashedPassword);

                        }
                        else
                        {
                            MessageBox.Show("Password and Confirm Password do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        cmdInsert.Parameters.AddWithValue("@username", txtUsername.Text.Trim());

                        int rows = cmdInsert.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("User registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            //redirect to login pages
                            frmLogin frmLogin = new frmLogin();
                            frmLogin.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Failed to register user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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
