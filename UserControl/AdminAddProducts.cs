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
using System.Xml.Linq;

namespace OOADCafeShopManagement
{
    public partial class AdminAddProducts : UserControl
    {
        //Private Fields


        public AdminAddProducts()
        {
            InitializeComponent();
            LoadData();
        }
        // Load Data Method
        public void LoadData()
        {
            ProductListDataGridView();
            ApplyRoleBasedAccess();
            SetupComboBoxes();
            ClearInputs();
            this.txtProductID.Visible = false;
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
            dgvListProducts.Columns[6].HeaderText = "Status";
            dgvListProducts.Columns[7].HeaderText = "Categories";
            dgvListProducts.Columns[8].HeaderText = "Supplier";

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
        public void LoadStatus()
        {
            var statusOptions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("active", "Active"),
                new KeyValuePair<string, string>("inactive", "Inactive"),
                new KeyValuePair<string, string>("out_of_stock", "Out of Stock"),
                new KeyValuePair<string, string>("discontinued", "Discontinued")
            };

            cmbStatus.DataSource = statusOptions;
            cmbStatus.DisplayMember = "Value";  // Show "Active", "Inactive"
            cmbStatus.ValueMember = "Key";      // Store "active", "inactive"
            cmbStatus.SelectedIndex = 0;
        }


        //Helper Method
        private void SetupComboBoxes()
        {
            LoadCategories();
            LoadSuppliers();
            LoadStatus();
        }
        public void ClearInputs()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtDiscount.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbSupplier.SelectedIndex = -1;
        }
        private void ApplyRoleBasedAccess()
        {
            if (UserSession.Role != "admin")
            {
                pnlAction.Visible = false;
                pnlListProducts.Size = new Size(759, 402);
                dgvListProducts.Size = new Size(759, 346);
            }

        }



        //Action Method
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // avoid error when clicking header
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvListProducts.Rows[e.RowIndex];
                txtProductID.Text = row.Cells["id"].Value.ToString();
                // or row.Cells[0].Value.ToString(); if ID is first column
            }
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
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the selected value correctly
                string selectedStatus = cmbStatus.SelectedValue?.ToString();
                ProductHandlers productHandlers = new ProductHandlers();
                bool success = productHandlers.UpdateProduct(
                    int.Parse(txtProductID.Text),
                    txtProductName.Text,
                    (int)cmbCategory.SelectedValue,
                    (int)cmbSupplier.SelectedValue,
                    decimal.Parse(txtProductPrice.Text),
                    decimal.Parse(txtDiscount.Text),
                    selectedStatus
                );
                if (success)
                {
                    MessageBox.Show("Product updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                    LoadData();
                }
                else
                {
                    MessageBox.Show("No changes were made to the product.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message);
            }
        }
        private void txtSearchId_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtProductID.Text, out int productId)) // Check if input is a valid integer
            {
                ProductHandlers productHandlers = new ProductHandlers();
                var product = productHandlers.SearchProductById(productId);
                if (product != null)
                {
                    txtProductName.Text = product.Name;
                    cmbCategory.SelectedValue = product.CategoryID;
                    cmbSupplier.SelectedValue = product.SupplierID;
                    // Set by Value - this should work now
                    cmbStatus.SelectedValue = product.Status?.ToLower(); // Ensure case matches
                    txtProductPrice.Text = product.Price.ToString();
                    txtDiscount.Text = product.Discount.ToString();
                }
                else
                {
                    // Clear inputs when no match found
                    txtProductName.Clear();
                    txtProductPrice.Clear();
                    txtDiscount.Clear();
                    cmbCategory.SelectedIndex = -1;
                    cmbStatus.SelectedIndex = -1;
                    cmbSupplier.SelectedIndex = -1;

                }
            }
        }
        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            ProductHandlers productHandlers = new ProductHandlers();
            List<ProductHandlers> filteredProducts = productHandlers.SearchProductByName(txtSearchProduct.Text);
            dgvListProducts.DataSource = filteredProducts;
            // Hide unwanted columns
            dgvListProducts.Columns["CategoryID"].Visible = false;
            dgvListProducts.Columns["SupplierID"].Visible = false;

            // Custom Header Name
            dgvListProducts.Columns[6].HeaderText = "Status";
            dgvListProducts.Columns[7].HeaderText = "Categories";
            dgvListProducts.Columns[8].HeaderText = "Supplier";
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ProductHandlers productHandlers = new ProductHandlers();
                bool success = productHandlers.DeleteProduct(int.Parse(txtProductID.Text));
                if (success)
                {
                    MessageBox.Show("Product deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch
            {
                MessageBox.Show("Error deleting product.");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }
    }
}
