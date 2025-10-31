using OOADCafeShopManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OOADCafeShopManagement
{
    public partial class AdminAddUser : UserControl
    {
        private AdminAddUserData model;
        private string selectedImagePath = "";
        private int selectedUserId = 0;
        private bool isEditMode = false;

        public AdminAddUser()
        {
            InitializeComponent();
            model = new AdminAddUserData();
            DisplayUsersData();
            SetupComboBoxes();
            SetFormMode(false); // Start in Add mode
        }

        private void SetupComboBoxes()
        {
            cmbRole.Items.AddRange(new string[] { "admin", "staff", "manager", "cashier" });
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbStatus.Items.AddRange(new string[] { "Active", "Inactive", "Suspended" });
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void DisplayUsersData()
        {
            try
            {
                AdminAddUserData userData = new AdminAddUserData();
                List<AdminAddUserData> listData = userData.UsersListData();

                dgvListUsers.DataSource = listData;

                // Hide sensitive columns
                dgvListUsers.Columns["Password"].Visible = false;
                dgvListUsers.Columns["ProfilePicturePath"].Visible = false;
                dgvListUsers.Columns["RegisterDate"].Visible = false;

                // Set column widths
                dgvListUsers.Columns["ID"].Width = 60;
                dgvListUsers.Columns["Username"].Width = 150;
                dgvListUsers.Columns["Role"].Width = 120;
                dgvListUsers.Columns["Status"].Width = 100;

                ApplyRoleBasedLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyRoleBasedLayout()
        {
            if (GetUserRole() != "admin")
            {
                pnlListUsers.Size = new Size(776, 473);
                pnlListUsers.Location = new Point(5, 0);
                dgvListUsers.Size = pnlListUsers.Size;
                dgvListUsers.Location = new Point(3, 32);
                pnlForm.Visible = false;
            }
            else
            {
                pnlForm.Visible = true;
            }
        }

        private void SetFormMode(bool editMode)
        {
            isEditMode = editMode;

            if (!editMode)
            {
                // Add Mode
                btnAdd.Visible = true;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;
                btnCancel.Visible = false;
                lblMode.Text = "ADD NEW USER";
                lblMode.ForeColor = Color.Green;
                ClearFields();
                selectedUserId = 0;
            }
            else
            {
                // Edit Mode
                btnAdd.Visible = false;
                btnUpdate.Visible = true;
                btnDelete.Visible = true;
                btnCancel.Visible = true;
                lblMode.Text = "EDIT USER";
                lblMode.ForeColor = Color.Blue;
            }
        }

        private bool IsEmptyFields()
        {
            return string.IsNullOrWhiteSpace(txtUsername.Text) ||
                   string.IsNullOrWhiteSpace(txtPassword.Text) ||
                   cmbRole.SelectedItem == null ||
                   cmbStatus.SelectedItem == null;
        }

        private bool IsValidInput()
        {
            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Username must be at least 3 characters long.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtPassword.Text.Length < 6 && !isEditMode)
            {
                MessageBox.Show("Password must be at least 6 characters long.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

           // Check if passwords match(only if password is provided in edit mode)
            if (isEditMode && !string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            if (!isEditMode && txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        public string GetUserRole()
        {
            return "admin"; // This should come from your authentication system
        }

        // ADD BUTTON
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (IsEmptyFields())
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidInput())
            {
                return;
            }

            try
            {
                AdminAddUserData userData = new AdminAddUserData();
                bool success = userData.AddUser(
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    cmbRole.SelectedItem.ToString(),
                    cmbStatus.SelectedItem.ToString(),
                    selectedImagePath
                );

                if (success)
                {
                    MessageBox.Show("User added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayUsersData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UPDATE BUTTON
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedUserId == 0)
            {
                MessageBox.Show("Please select a user to update.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                cmbRole.SelectedItem == null ||
                cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidInput())
            {
                return;
            }

            try
            {
                AdminAddUserData userData = new AdminAddUserData();
                bool success;

                // Check if password should be updated
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    // Update with new password
                    success = userData.UpdateUserWithPassword(
                        selectedUserId,
                        txtUsername.Text.Trim(),
                        txtPassword.Text,
                        cmbRole.SelectedItem.ToString(),
                        cmbStatus.SelectedItem.ToString(),
                        selectedImagePath
                    );
                }
                else
                {
                    // Update without changing password
                    success = userData.UpdateUser(
                        selectedUserId,
                        txtUsername.Text.Trim(),
                        cmbRole.SelectedItem.ToString(),
                        cmbStatus.SelectedItem.ToString(),
                        selectedImagePath
                    );
                }

                if (success)
                {
                    MessageBox.Show("User updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayUsersData();
                    SetFormMode(false); // Return to Add mode
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DELETE BUTTON
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId == 0)
            {
                MessageBox.Show("Please select a user to delete.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Prevent self-deletion (optional)
            if (IsCurrentUser(selectedUserId))
            {
                MessageBox.Show("You cannot delete your own account.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete user '{txtUsername.Text}'?\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    AdminAddUserData userData = new AdminAddUserData();
                    bool success = userData.DeleteUser(selectedUserId);

                    if (success)
                    {
                        MessageBox.Show("User deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DisplayUsersData();
                        SetFormMode(false); // Return to Add mode
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsCurrentUser(int userId)
        {
            // Implement logic to check if the user being deleted is the current logged-in user
            // This is a placeholder - you'll need to implement based on your authentication system
            return false;
        }

        // CANCEL BUTTON
        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetFormMode(false);
        }

        // BROWSE IMAGE BUTTON
        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.Title = "Select Profile Picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;
                    txtImagePath.Text = Path.GetFileName(selectedImagePath);

                    // Display preview
                    try
                    {
                        picUserProfile.Image = Image.FromFile(selectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // CLEAR IMAGE BUTTON
        private void btnClearImage_Click(object sender, EventArgs e)
        {
            selectedImagePath = "";
            txtImagePath.Text = "";
            picUserProfile.Image = OOADCafeShopManagement.Properties.Resources.user_10x;
        }

        // DATA GRID SELECTION
        private void dgvListUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvListUsers.SelectedRows.Count > 0 && dgvListUsers.SelectedRows[0] != null)
            {
                DataGridViewRow selectedRow = dgvListUsers.SelectedRows[0];
                selectedUserId = (int)selectedRow.Cells["ID"].Value;

                // Load user data into form
                LoadUserData(selectedUserId);
                SetFormMode(true); // Switch to Edit mode
            }
        }

        private void LoadUserData(int userId)
        {
            try
            {
                AdminAddUserData userData = new AdminAddUserData();
                var user = userData.GetUserById(userId);

                if (user != null)
                {
                    txtUsername.Text = user.Username;
                    cmbRole.SelectedItem = user.Role;
                    cmbStatus.SelectedItem = user.Status;

                    // Load profile picture if exists
                    if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                    {
                        selectedImagePath = user.ProfilePicturePath;
                        txtImagePath.Text = Path.GetFileName(selectedImagePath);

                        try
                        {
                            if (File.Exists(selectedImagePath))
                            {
                                picUserProfile.Image = Image.FromFile(selectedImagePath);
                            }
                            else
                            {
                                picUserProfile.Image = OOADCafeShopManagement.Properties.Resources.user_10x;
                                // Don't show warning - just clear the image
                            }
                        }
                        catch
                        {
                            picUserProfile.Image = OOADCafeShopManagement.Properties.Resources.user_10x;
                        }
                    }
                    else
                    {
                        selectedImagePath = "";
                        txtImagePath.Text = "";
                        picUserProfile.Image = OOADCafeShopManagement.Properties.Resources.user_10x;
                    }

                    // Clear password fields for security
                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";

                    // Show hint for password
                    txtPassword.PlaceholderText = "Leave blank to keep current password";
                    txtConfirmPassword.PlaceholderText = "Leave blank to keep current password";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtImagePath.Text = "";
            cmbRole.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            selectedImagePath = "";
            picUserProfile.Image = OOADCafeShopManagement.Properties.Resources.user_10x;

            // Reset placeholders
            txtPassword.PlaceholderText = "Enter password";
            txtConfirmPassword.PlaceholderText = "Confirm password";
        }

        // REFRESH BUTTON
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DisplayUsersData();
            if (!isEditMode)
            {
                ClearFields();
            }
        }

        // SEARCH FUNCTIONALITY
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchUsers(txtSearch.Text);
        }

        private void SearchUsers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                DisplayUsersData();
                return;
            }

            try
            {
                AdminAddUserData userData = new AdminAddUserData();
                List<AdminAddUserData> allData = userData.UsersListData();

                var filteredData = allData.Where(u =>
                    u.Username.ToLower().Contains(searchTerm.ToLower()) ||
                    u.Role.ToLower().Contains(searchTerm.ToLower()) ||
                    u.Status.ToLower().Contains(searchTerm.ToLower())
                ).ToList();

                dgvListUsers.DataSource = filteredData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CLEAR SEARCH BUTTON
        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            DisplayUsersData();
        }

        private void dgvListUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}