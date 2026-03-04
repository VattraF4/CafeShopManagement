using OOADCafeShopManagement.Models;
using OOADCafeShopManagement.Services;
using OOADCafeShopManagement.Repositories;
using OOADCafeShopManagement.Factory;
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
    public partial class POSForm : UserControl
    {
        private OrderManager _orderManager;
        private ProductService _productService;
        private int rowIndex = 0;

        public POSForm()
        {
            InitializeComponent();
            _orderManager = new OrderManager(); // Initialize OrderManager
            // Use ServiceFactory to create ProductService (Factory Pattern)
            _productService = ServiceFactory.Instance.CreateProductService();
            LoadForm();
        }

        // Loading Form
        private void LoadForm()
        {

            ListMenu();
            InitializeOrderDisplay();
            ClearSelectedItemDisplay();
        }

        // Initialize order display
        private void InitializeOrderDisplay()
        {
            // Bind current order items to DataGridView
            dgvCurrentOrder.DataSource = _orderManager.CurrentItems;

            // Format the order display grid
            if (dgvCurrentOrder.Columns.Count > 0)
            {
                //Hide Column
                dgvCurrentOrder.Columns["ProductID"].Visible = false;
                dgvCurrentOrder.Columns["OrderID"].Visible = false;
                dgvCurrentOrder.Columns["OrderDetailID"].Visible = false;
                dgvCurrentOrder.Columns["Discount"].Visible = false;
                // Custom Header Name
                //dgvCurrentOrder.Columns["ProductID"].HeaderText = "Product ID";
                dgvCurrentOrder.Columns["ProductName"].HeaderText = "Name";
                dgvCurrentOrder.Columns["UnitPrice"].HeaderText = "Price";
                dgvCurrentOrder.Columns["Quantity"].HeaderText = "Qty";
                dgvCurrentOrder.Columns["Total"].HeaderText = "Amount";



            }

            UpdateOrderSummary();
        }


        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            List<Products> productsList = _productService.SearchProducts(txtInputName.Text.Trim());
            dgvListMenu.DataSource = productsList;
            int productID = dgvCurrentOrder.Rows[rowIndex].Cells["ProductID"].Value != null ?
                Convert.ToInt32(dgvCurrentOrder.Rows[rowIndex].Cells["ProductID"].Value) : 0;
            string productName = dgvCurrentOrder.Rows[rowIndex].Cells["ProductName"].Value != null ?
                dgvCurrentOrder.Rows[rowIndex].Cells["ProductName"].Value.ToString() : "";
            decimal unitPrice = dgvCurrentOrder.Rows[rowIndex].Cells["UnitPrice"].Value != null ?
                Convert.ToDecimal(dgvCurrentOrder.Rows[rowIndex].Cells["UnitPrice"].Value) : 0;
            decimal discount = 0;
            _orderManager.SetSelectedProduct(productID, productName, unitPrice, discount);

        }

        // Helper Method to List Orders (if needed for order history)
        public void PerformAddButtonClick()
        {
            btnOrderAdd.PerformClick();
        }

        private void ListMenu()
        {
            List<Products> productsList = _productService.GetActiveProducts();
            dgvListMenu.DataSource = productsList;

            // Hide unwanted columns
            dgvListMenu.Columns["CategoryID"].Visible = false;
            dgvListMenu.Columns["SupplierID"].Visible = false;

            // Custom Header Name
            dgvListMenu.Columns[6].HeaderText = "Categories";
            dgvListMenu.Columns[7].HeaderText = "Supplier";
        }

        // STEP 1: When user clicks on a product in the menu (select product)
        private void dgvListMenu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvListMenu.Rows[e.RowIndex];

                // Get product details from the selected row
                int productId = Convert.ToInt32(row.Cells["ID"].Value);
                string productName = row.Cells["Name"].Value.ToString();
                decimal unitPrice = Convert.ToDecimal(row.Cells["Price"].Value); // Adjust column name if needed
                decimal discount = Convert.ToDecimal(row.Cells["Discount"].Value);

                // Store in selected item (single temporary item)
                _orderManager.SetSelectedProduct(productId, productName, unitPrice, discount);

                // Display the selected item in the input fields
                UpdateSelectedItemDisplay();
            }
        }

        // Display the selected item in input fields
        private void UpdateSelectedItemDisplay()
        {
            if (_orderManager.SelectedItem != null && _orderManager.SelectedItem.ProductID > 0)
            {
                //txtInputName.Text = _orderManager.SelectedItem.ProductName;
                lblProductName.Text = _orderManager.SelectedItem.ProductName;
                lblUnitPrice.Text = _orderManager.SelectedItem.UnitPrice.ToString("C2");
                nudQuantity.Text = _orderManager.SelectedItem.Quantity.ToString();
                txtItemDiscount.Text = _orderManager.SelectedItem.Discount.ToString();

                CalculateSelectedItemAmount();
            }
        }

        // Clear the selected item display
        private void ClearSelectedItemDisplay()
        {
            txtInputName.Clear();
            nudQuantity.Text = "1";
            txtItemDiscount.Text = "0";
            lblUnitPrice.Text = "";
            lblItemAmount.Text = "";
            lblProductName.Text = "";
        }

        // Calculate amount for selected item
        private void CalculateSelectedItemAmount()
        {
            if (_orderManager.SelectedItem != null)
            {
                decimal amount = (_orderManager.SelectedItem.UnitPrice * _orderManager.SelectedItem.Quantity)
                               - _orderManager.SelectedItem.Discount;
                lblItemAmount.Text = amount.ToString("C2");
            }
        }


        // Quantity text changed - update selected item
        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            if (_orderManager.SelectedItem != null)
            {
                decimal quantity = nudQuantity.Value;
                _orderManager.UpdateSelectedItem(quantity, _orderManager.SelectedItem.Discount);
                CalculateSelectedItemAmount();
            }
        }


        // Discount text changed - update selected item
        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtItemDiscount.Text, out decimal discount) && _orderManager.SelectedItem != null)
            {
                _orderManager.UpdateSelectedItem(_orderManager.SelectedItem.Quantity, discount);
                CalculateSelectedItemAmount();
            }
        }
        private void txtTotalDiscount_TextChanged(object sender, EventArgs e)
        {
            ApplyCustomTotalDiscount();
        }
        private void ApplyCustomTotalDiscount()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTotalDiscount.Text))
                {
                    // If empty, reset to calculated discount
                    UpdateOrderSummary();
                    return;
                }

                // Parse the custom discount value
                string discountText = txtTotalDiscount.Text.Replace("$", "").Replace(",", "").Trim();

                if (decimal.TryParse(discountText, out decimal customDiscount))
                {
                    // Validate discount doesn't exceed total amount
                    decimal maxDiscount = _orderManager.CurrentOrder.TotalAmount;

                    if (customDiscount > maxDiscount)
                    {
                        MessageBox.Show($"Discount cannot exceed total amount of {maxDiscount:C2}",
                            "Invalid Discount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTotalDiscount.Text = maxDiscount.ToString("F2");
                        customDiscount = maxDiscount;
                    }

                    if (customDiscount < 0)
                    {
                        MessageBox.Show("Discount cannot be negative",
                            "Invalid Discount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTotalDiscount.Text = "0";
                        customDiscount = 0;
                    }

                    // Apply the custom discount
                    _orderManager.CurrentOrder.Discount = customDiscount;

                    // Update display
                    UpdateOrderSummaryWithCustomDiscount(customDiscount);
                }
                else
                {
                    // If invalid input, revert to calculated discount
                    UpdateOrderSummary();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying custom discount: {ex.Message}");
                UpdateOrderSummary(); // Revert to calculated discount
            }
        }
        private void UpdateOrderSummaryWithCustomDiscount(decimal customDiscount)
        {
            decimal totalAmount = _orderManager.CurrentOrder.TotalAmount;
            decimal grandTotal = totalAmount - customDiscount;

            // Calculate total quantity (sum of all item quantities)
            int totalQuantity = _orderManager.CurrentItems.Sum(item => (int)item.Quantity);

            // Count distinct products
            int productCount = _orderManager.CurrentItems.Count;

            //lblItemAmount.Text = totalAmount.ToString("C2");
            lblGrandTotal.Text = grandTotal.ToString("C2");

            // Fixed: Show total quantity and product count
            lblItemCount.Text = $"{totalQuantity} items / {productCount} products";

            // Don't update txtTotalDiscount here to avoid recursive events
            dgvCurrentOrder.Refresh();
        }
        // STEP 2: Add button click (Add to order list)
        private void btnOrderAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Add the selected item to the order list
                _orderManager.AddSelectedItemToOrder();

                // Refresh displays
                UpdateOrderSummary();
                ClearSelectedItemDisplay();


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item to order: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Update order summary display
        private void UpdateOrderSummary()
        {
            var summary = _orderManager.GetOrderSummary();

            // Calculate total quantity and product count
            int totalQuantity = _orderManager.CurrentItems.Sum(item => (int)item.Quantity);
            int productCount = _orderManager.CurrentItems.Count;

            lblItemAmount.Text = summary.totalAmount.ToString("C2");
            txtTotalDiscount.Text = summary.discount.ToString("C2");
            lblGrandTotal.Text = summary.grandTotal.ToString("C2");
            lblItemCount.Text = $"{totalQuantity} items / {productCount} products";

            dgvCurrentOrder.Refresh();
        }

        // Remove selected item from order
        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvCurrentOrder.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dgvCurrentOrder.SelectedRows[0].Cells["ProductID"].Value);
                _orderManager.RemoveOrderItem(productId);
                UpdateOrderSummary();
            }
        }

        // Clear entire order


        // Process payment
        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                if (_orderManager.CurrentItems.Count == 0)
                {
                    MessageBox.Show("Please add items to order first!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get customer info
                string note = txtNote.Text;
                string status = "Completed";

                //Print Receipt before update

                // Process payment
                bool success = _orderManager.ProcessPayment(note, status);

                if (success)
                {
                    MessageBox.Show("Payment processed successfully! Order saved.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //Print receipt after insert to database
                    PrintReceipt();

                    // Clear form for next order
                    _orderManager.ClearOrder();

                    ClearForm();
                    UpdateOrderSummary();

                    dgvCurrentOrder.DataSource = null;
                    InitializeOrderDisplay();
                }
                else
                {
                    MessageBox.Show("Failed to process payment. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clear form inputs
        private void ClearForm()
        {
            txtNote.Clear();
            ClearSelectedItemDisplay();
        }

        // Clear selected item only (not the entire order)

        private void btnOrderClear_Click(object sender, EventArgs e)
        {
            _orderManager.ClearSelectedItem();
            ClearSelectedItemDisplay();
        }
        private void btnClearOrder_Click(object sender, EventArgs e)
        {
            _orderManager.ClearOrder();

            // Refresh DataGridView by resetting DataSource
            dgvCurrentOrder.DataSource = null;
            InitializeOrderDisplay();

            UpdateOrderSummary();
            ClearSelectedItemDisplay();
            MessageBox.Show("Order cleared!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Create receipt layout for printing
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10);
            Font smallFont = new Font("Arial", 8);

            float yPos = 20;
            float leftMargin = 20;

            // Header
            g.DrawString("CAFE SHOP RECEIPT", titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Order info
            g.DrawString($"Order #: {_orderManager.CurrentOrder.OrderID}", normalFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;
            g.DrawString($"Date: {_orderManager.CurrentOrder.OrderDate:dd/MM/yyyy HH:mm}", normalFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;
            //g.DrawString($"Status: Paid", normalFont, Brushes.Black, leftMargin, yPos);
            g.DrawString($"Status: {_orderManager.CurrentOrder.Status}", normalFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;
            g.DrawString($"Sale Person: {UserSession.Username}", smallFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;

            // Items header
            g.DrawString("Items:", headerFont, Brushes.Black, leftMargin, yPos);
            yPos += 25;

            // Draw line
            g.DrawLine(Pens.Black, leftMargin, yPos, 300, yPos);
            yPos += 10;

            // Items
            foreach (var item in _orderManager.CurrentItems)
            {
                string itemLine = $"{item.ProductName} x{item.Quantity}";
                string priceLine = $"{(item.UnitPrice * item.Quantity - item.Discount):C2}";

                g.DrawString(itemLine, normalFont, Brushes.Black, leftMargin, yPos);
                g.DrawString(priceLine, normalFont, Brushes.Black, 200, yPos);
                yPos += 20;

                if (item.Discount > 0)
                {
                    g.DrawString($"  Discount: -{item.Discount:C2}", smallFont, Brushes.Black, leftMargin + 10, yPos);
                    yPos += 15;
                }
            }

            yPos += 10;
            g.DrawLine(Pens.Black, leftMargin, yPos, 300, yPos);
            yPos += 20;

            // Totals
            var summary = _orderManager.GetOrderSummary();
            g.DrawString($"Subtotal: {summary.totalAmount:C2}", normalFont, Brushes.Black, 130, yPos);
            yPos += 20;
            g.DrawString($"Discount: -{summary.discount:C2}", normalFont, Brushes.Black, 130, yPos);
            yPos += 20;

            // Draw line
            g.DrawLine(Pens.Black, leftMargin, yPos, 300, yPos);
            yPos += 10;
            g.DrawString($"Grand Total: {summary.grandTotal:C2}", headerFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Note
            if (!string.IsNullOrEmpty(_orderManager.CurrentOrder.Note))
            {
                g.DrawString($"Note: {_orderManager.CurrentOrder.Note}", smallFont, Brushes.Black, leftMargin, yPos);
                yPos += 20;
            }


            // Footer
            g.DrawString("Thank you for your business!", normalFont, Brushes.Black, leftMargin, yPos);
        }
        private void PrintReceipt()
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = printDocument1;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();
                }
            }
        }

        public void ReloadProducts()
        {
            LoadForm();
        }
    }
}