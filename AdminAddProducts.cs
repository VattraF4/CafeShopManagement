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
            ProductListDataGridView();
            SetupComboBoxes();
        }
        public void ProductListDataGridView()
        {
            ProductHandlers products = new ProductHandlers();
            List<ProductHandlers> productsList = products.ListData(); //access to ListData method (match return type)
            dgvListProducts.DataSource = productsList;

            // Hide unwanted columns
            dgvListProducts.Columns["CategoryID"].Visible = false;
            dgvListProducts.Columns["SupplierID"].Visible = false;

            // Custom Header Name
            dgvListProducts.Columns[6].HeaderText = "Categories";
            dgvListProducts.Columns[7].HeaderText = "Supplier";

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ProductHandlers products = new ProductHandlers();
                bool success = products.AddProduct(
                    txtProductName.Text,
                    (int)cmbCategory.SelectedValue,
                    (int)cmbSupplier.SelectedValue,
                    decimal.Parse(txtProductPrice.Text),
                    decimal.Parse(txtDiscount.Text)
                );
                if (success)
                {
                    MessageBox.Show("User added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message);
            }
        }
    }
}
