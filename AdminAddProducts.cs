using OOADCafeShopManagement.Models;
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
    public partial class AdminAddProducts : UserControl
    {
        public AdminAddProducts()
        {
            InitializeComponent();
            LoadData();
        }
        public void LoadData()
        {
            ProductHandlers products = new ProductHandlers();
            List<ProductHandlers> productsList = products.ListData(); //access to ListData method (match return type)
            dgvListProducts.DataSource = productsList;
            SetupComboBoxes();
        }
        private void SetupComboBoxes()
        {
            LoadCategories();
            LoadSuppliers();
        }
        public void LoadCategories()
        {
            CategoriesHandler categoriesHandler = new CategoriesHandler();

            //get categories from database
            List<CategoriesHandler> categoriesList = categoriesHandler.ListData();

            // Bind to ComboBox
            cmbCategory.DataSource = categoriesList;
            cmbCategory.DisplayMember = "Name"; // What the user sees
            cmbCategory.ValueMember = "ID";     // The actual value
        }
        public void LoadSuppliers()
        {
            //Step to get Data from database to store on class list

            //1 Instance OBJ
            SupplierHandlers supplierHandler = new SupplierHandlers();
            //2 Call ListData method to get list of suppliers
            List<SupplierHandlers> suppliersList = supplierHandler.ListData();

            //3 Bind to ComboBox
            cmbSupplier.DataSource = suppliersList; // DataSource property to bind list
            cmbSupplier.DisplayMember = "Name"; // What the user sees
            cmbSupplier.ValueMember = "ID";     // The actual value
        }

    }
}
