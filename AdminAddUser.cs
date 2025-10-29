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
    public partial class AdminAddUser : UserControl
    {
        public AdminAddUser()
        {
            InitializeComponent();
            DisplayUsersData();
        }
        public void DisplayUsersData()
        {
            AdminAddUserData userData = new AdminAddUserData();
            List<AdminAddUserData> listData = userData.UsersListData();

            dgvListUsers.DataSource = listData; // this will show all fields
            // Hide the columns I don't want to show
            dgvListUsers.Columns["Password"].Visible = false;
            dgvListUsers.Columns["ProfilePicturePath"].Visible = false;

            // Set specific column widths (in pixels)
            dgvListUsers.Columns["ID"].Width = 60;           // Small for ID
            dgvListUsers.Columns["Username"].Width = 150;    // Medium for username
            dgvListUsers.Columns["Role"].Width = 120;        // Medium for role
            dgvListUsers.Columns["Status"].Width = 100;      // Small for status

            // Optional: Improve appearance
            //dgvListUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            //dgvListUsers.RowHeadersVisible = false; // Hide the row headers
            //dgvListUsers.BorderStyle = BorderStyle.None;
            //dgvListUsers.BackgroundColor = SystemColors.Window;
            int i=0;
            foreach (var data in listData)
            {
                Console.WriteLine(data);
                i++;
            }
        }
    }
}
