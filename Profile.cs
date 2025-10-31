using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOADCafeShopManagement
{
    public partial class Profile : Form
    {
        // Config for moving form
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

        // Use to move window form by properties tools or control box
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
            Environment.Exit(0);
        }

        private void lbX_MouseEnter(object sender, EventArgs e)
        {
            lbX.ForeColor = Color.Red;
        }

        private void lbX_MouseLeave(object sender, EventArgs e)
        {
            lbX.ForeColor = Color.White;
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
            LoadProfilePicture();
            LoadUserInfo();
            ApplyRoleBasedAccess();
        }

        private void LoadProfilePicture()
        {
            try
            {
                // Try to get profile picture from UserSession or database
                string profilePath = GetProfilePath();

                if (!string.IsNullOrEmpty(profilePath) && File.Exists(profilePath))
                {
                    picCurrentUser.Image = Image.FromFile(profilePath);
                }
                else
                {
                    // Load default image
                    LoadDefaultProfilePicture();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile picture: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadDefaultProfilePicture();
            }
        }

        private string GetProfilePath()
        {
            // Option 1: Check if UserSession has ProfilePicturePath property
            // If you added this property to UserSession
            if (UserSessionHasProfilePath() && !string.IsNullOrEmpty(GetUserSessionProfilePath()))
            {
                return GetUserSessionProfilePath();
            }

            // Option 2: Get from database
            return GetProfilePathFromDatabase();

            // Option 3: Hardcoded path for testing (remove in production)
            // return @"C:\Users\Lenovo\Pictures\fav-image.png";
        }

        private bool UserSessionHasProfilePath()
        {
            // Check if ProfilePicturePath property exists in UserSession
            try
            {
                var test = UserSession.ProfilePath;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetUserSessionProfilePath()
        {
            try
            {
                return UserSession.ProfilePath ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetProfilePathFromDatabase()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True"))
                {
                    connection.Open();
                    string query = "SELECT profile_img FROM users WHERE id = @userId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", UserSession.UserId);
                        object result = command.ExecuteScalar();
                        return result?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void LoadDefaultProfilePicture()
        {
            try
            {
                // Try to load from default path
                string defaultPath = "profiles/default.png";
                if (File.Exists(defaultPath))
                {
                    picCurrentUser.Image = Image.FromFile(defaultPath);
                }
                else
                {
                    // Create a blank image
                    CreateBlankProfilePicture();
                }
            }
            catch (Exception)
            {
                CreateBlankProfilePicture();
            }
        }

        private void CreateBlankProfilePicture()
        {
            Bitmap blankImage = new Bitmap(picCurrentUser.Width, picCurrentUser.Height);
            using (Graphics g = Graphics.FromImage(blankImage))
            {
                g.Clear(Color.LightGray);
                using (Font font = new Font("Arial", 10))
                using (Brush brush = new SolidBrush(Color.DarkGray))
                {
                    g.DrawString("No Image", font, brush, 10, picCurrentUser.Height / 2 - 10);
                }
            }
            picCurrentUser.Image = blankImage;
        }

        private void LoadUserInfo()
        {
            string username = GetUsername();
            if (!string.IsNullOrEmpty(username))
            {
                lblUsername.Text = username;
            }
            else
            {
                lblUsername.Text = "Unknown User";
            }

            // Display additional user info if you have labels for them
            if (lblRole != null)
            {
                lblRole.Text = $"{GetUserRole()}";
            }

           
        }

        public string GetUsername()
        {
            return UserSession.Username ?? "Guest";
        }

        public string GetUserRole()
        {
            return UserSession.Role ?? "Unknown";
        }

        private void ApplyRoleBasedAccess()
        {
            string userRole = GetUserRole();

            if (userRole != "admin")
            {
                // Hide admin-only features
                if (btnCustomers != null) btnCustomers.Visible = false;
                if (btnProducts != null) btnProducts.Visible = false;
                if (btnCashiers != null) btnCashiers.Visible = false;

                // Remove duplicate: btnProducts.Visible = false;
            }

           
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
                UserSession.Logout(); // Use Clear() method if you have it, or Logout()

                MessageBox.Show("You have been logged out successfully.", "Logout",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RedirectToLogin();
            }
        }

        private void RedirectToLogin()
        {
            frmLogin loginForm = new frmLogin();
            loginForm.Show();
            this.Close();
        }

        // Add these methods if they don't exist in your UserSession class
        // Or update your UserSession class to include them

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            // Add functionality to update profile picture
            UpdateProfilePicture();
        }

        private void UpdateProfilePicture()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.Title = "Select Profile Picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string selectedPath = openFileDialog.FileName;
                        picCurrentUser.Image = Image.FromFile(selectedPath);

                        // Save to database
                        SaveProfilePicturePath(selectedPath);

                        // Update UserSession if needed
                        UpdateUserSessionProfilePath(selectedPath);

                        MessageBox.Show("Profile picture updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating profile picture: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveProfilePicturePath(string imagePath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True"))
                {
                    connection.Open();
                    string query = "UPDATE users SET profile_img = @profilePath WHERE id = @userId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@profilePath", imagePath);
                        command.Parameters.AddWithValue("@userId", UserSession.UserId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving profile path: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUserSessionProfilePath(string profilePath)
        {
            // If your UserSession has ProfilePicturePath property
            try
            {
                // This will only work if you added ProfilePicturePath to UserSession
                UserSession.ProfilePath = profilePath;
            }
            catch
            {
                // Silently fail if property doesn't exist
            }
        }

        // Handle form closing
        private void Profile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to exit?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}