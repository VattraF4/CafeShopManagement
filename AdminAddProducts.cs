using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace OOADCafeShopManagement
{
    public partial class AdminAddProducts : UserControl
    {

        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=cafe;Integrated Security=True;TrustServerCertificate=True";
        public AdminAddProducts()
        {
            InitializeComponent();
        }
       

        

        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        
    }
}
    

