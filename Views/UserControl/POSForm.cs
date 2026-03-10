using OOADCafeShopManagement.Models;
using OOADCafeShopManagement.Services;
using OOADCafeShopManagement.Repositories;
using OOADCafeShopManagement.Factory;
using OOADCafeShopManagement.Strategy;
using OOADCafeShopManagement.State;
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

        // STATE PATTERN - Order State Management
        private OrderContext _orderContext;
        private Label _lblCurrentState;
        private Button _btnProcessOrder;
        private Button _btnCompleteOrder;
        private Button _btnCancelOrder;
        private Button _btnRefundOrder;

        public POSForm()
        {
            InitializeComponent();
            _orderManager = new OrderManager();
            _productService = ServiceFactory.Instance.CreateProductService();

            // Initialize Order Type ComboBox (Strategy Pattern)
            InitializeOrderTypeComboBox();

            // Initialize State Pattern UI
            CreateStateManagementPanel();

            LoadForm();
        }

        // Initialize Order Type ComboBox with values
        private void InitializeOrderTypeComboBox()
        {
            try
            {
                // Check if cmbOrderType exists (created in Designer)
                if (cmbOrderType != null)
                {
                    // Clear existing items
                    cmbOrderType.Items.Clear();

                    // Add order types
                    cmbOrderType.Items.Add("Dine-In");
                    cmbOrderType.Items.Add("Takeaway");
                    cmbOrderType.Items.Add("Delivery");

                    // Set default to Dine-In
                    cmbOrderType.SelectedIndex = 0;

                    // Wire up event handler
                    cmbOrderType.SelectedIndexChanged -= CmbOrderType_SelectedIndexChanged; // Remove if exists
                    cmbOrderType.SelectedIndexChanged += CmbOrderType_SelectedIndexChanged; // Add

                    // Set initial strategy
                    _orderManager.SetOrderTypeStrategy(new DineInOrderStrategy());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing order type: {ex.Message}");
            }
        }

        // ========================================
        // STATE PATTERN UI - Safe for Bunifu
        // ========================================

        private void CreateStateManagementPanel()
        {
            try
            {
                // Wait for form to fully load
                this.Load += (s, e) => InitializeStateControls();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void InitializeStateControls()
        {
            try
            {
                if (bunifuPanel1 == null || bunifuPanel1.IsDisposed) return;

                // Calculate positions
                int panelWidth = bunifuPanel1.Width;
                int centerX = Math.Max(0, (panelWidth - 150) / 1);
                int topY = 0;

                // State label
                _lblCurrentState = new Label
                {
                    Name = "lblStatePattern",
                    Text = "No Order",
                    Location = new Point(centerX, topY),
                    Size = new Size(120, 14),
                    Font = new Font("Arial", 7, FontStyle.Bold),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent,
                    AutoSize = false
                };

                // Buttons
                int btnY = topY + 16;
                int btnW = 28;

                _btnProcessOrder = CreateStateButton("►", centerX, btnY, btnW, Color.LightGreen);
                _btnProcessOrder.Click += BtnProcessOrder_Click;

                _btnCompleteOrder = CreateStateButton("✓", centerX + btnW + 2, btnY, btnW, Color.LightBlue);
                _btnCompleteOrder.Click += BtnCompleteOrder_Click;

                _btnCancelOrder = CreateStateButton("✕", centerX + (btnW + 2) * 2, btnY, btnW, Color.LightCoral);
                _btnCancelOrder.Click += BtnCancelOrder_Click;

                _btnRefundOrder = CreateStateButton("↩", centerX + (btnW + 2) * 3, btnY, btnW, Color.LightGoldenrodYellow);
                _btnRefundOrder.Click += BtnRefundOrder_Click;

                // Add to panel safely
                bunifuPanel1.Controls.Add(_lblCurrentState);
                bunifuPanel1.Controls.Add(_btnProcessOrder);
                bunifuPanel1.Controls.Add(_btnCompleteOrder);
                bunifuPanel1.Controls.Add(_btnCancelOrder);
                bunifuPanel1.Controls.Add(_btnRefundOrder);

                _lblCurrentState.BringToFront();
                _btnProcessOrder.BringToFront();
                _btnCompleteOrder.BringToFront();
                _btnCancelOrder.BringToFront();
                _btnRefundOrder.BringToFront();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"State UI Error: {ex.Message}");
            }
        }

        private Button CreateStateButton(string text, int x, int y, int w, Color bgColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 18),
                BackColor = bgColor,
                Enabled = false,
                Font = new Font("Arial", 7, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ========================================
        // STATE PATTERN - Event Handlers
        // ========================================

        private void BtnProcessOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (_orderContext == null) return;
                _orderContext.Process();
                UpdateStateDisplay();
                MessageBox.Show("Processing", "State", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCompleteOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (_orderContext == null) return;
                _orderContext.Complete();
                UpdateStateDisplay();
                MessageBox.Show("Completed!", "State", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (_orderContext == null) return;
                if (MessageBox.Show("Cancel order?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _orderContext.Cancel();
                    UpdateStateDisplay();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefundOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (_orderContext == null) return;
                if (MessageBox.Show("Refund order?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _orderContext.Refund();
                    UpdateStateDisplay();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Update state display - simplified
        private void UpdateStateDisplay()
        {
            if (_orderContext == null)
            {
                _lblCurrentState.Text = "No Order";
                _lblCurrentState.ForeColor = Color.Gray;
                _btnProcessOrder.Enabled = false;
                _btnCompleteOrder.Enabled = false;
                _btnCancelOrder.Enabled = false;
                _btnRefundOrder.Enabled = false;
                return;
            }

            string state = _orderContext.GetCurrentStateName();
            _lblCurrentState.Text = $"State: {state}";

            switch (state)
            {
                case "Pending":
                    _lblCurrentState.ForeColor = Color.Orange;
                    _btnProcessOrder.Enabled = true;
                    _btnCompleteOrder.Enabled = false;
                    _btnCancelOrder.Enabled = true;
                    _btnRefundOrder.Enabled = false;
                    break;
                case "Processing":
                    _lblCurrentState.ForeColor = Color.Blue;
                    _btnProcessOrder.Enabled = false;
                    _btnCompleteOrder.Enabled = true;
                    _btnCancelOrder.Enabled = true;
                    _btnRefundOrder.Enabled = false;
                    break;
                case "Completed":
                    _lblCurrentState.ForeColor = Color.Green;
                    _btnProcessOrder.Enabled = false;
                    _btnCompleteOrder.Enabled = false;
                    _btnCancelOrder.Enabled = false;
                    _btnRefundOrder.Enabled = true;
                    break;
                default:
                    _lblCurrentState.ForeColor = Color.Red;
                    _btnProcessOrder.Enabled = false;
                    _btnCompleteOrder.Enabled = false;
                    _btnCancelOrder.Enabled = false;
                    _btnRefundOrder.Enabled = false;
                    break;
            }
        }



        private void CmbOrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbOrderType == null || _orderManager == null) return;

                string selectedType = cmbOrderType.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedType)) return;

                // Create appropriate order type strategy
                IOrderTypeStrategy orderTypeStrategy = null;
                switch (selectedType)
                {
                    case "Dine-In":
                        orderTypeStrategy = new DineInOrderStrategy();
                        break;
                    case "Takeaway":
                        orderTypeStrategy = new TakeawayOrderStrategy();
                        break;
                    case "Delivery":
                        orderTypeStrategy = new DeliveryOrderStrategy();
                        break;
                }

                if (orderTypeStrategy != null)
                {
                    _orderManager.SetOrderTypeStrategy(orderTypeStrategy);

                    // Recalculate if there are items
                    if (_orderManager.CurrentItems.Count > 0)
                    {
                        _orderManager.ApplyOrderTypeCalculations();
                        UpdateOrderSummary();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error changing order type: {ex.Message}");
            }
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

                string note = txtNote.Text;
                string orderType = cmbOrderType?.SelectedItem?.ToString() ?? "Dine-In";

                // Apply order type calculations before payment
                _orderManager.ApplyOrderTypeCalculations();

                // Process payment
                bool success = _orderManager.ProcessPayment(note, "Pending"); // Start in Pending state

                if (success)
                {
                    // ========================================
                    // STATE PATTERN - Initialize Order State
                    // ========================================
                    _orderContext = new OrderContext(_orderManager.CurrentOrder);
                    UpdateStateDisplay();

                    var summary = _orderManager.GetOrderSummary();
                    MessageBox.Show($"✅ Payment Successful!\n\n" +
                                  $"Order #{_orderManager.CurrentOrder.OrderID} created\n" +
                                  $"Initial State: {_orderContext.GetCurrentStateName()}\n\n" +
                                  $"Order Type: {orderType}\n" +
                                  $"Subtotal: {summary.totalAmount:C2}\n" +
                                  $"Service/Fees: {_orderManager.CurrentOrder.ServiceCharge:C2}\n" +
                                  $"Discount: {summary.discount:C2}\n" +
                                  $"Total: {summary.grandTotal:C2}\n\n" +
                                  $"🔄 Use State buttons to manage order status!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    PrintReceipt();

                    // Don't clear order yet - allow state management
                    // _orderManager.ClearOrder();
                    ClearForm();
                    // UpdateOrderSummary();

                    // dgvCurrentOrder.DataSource = null;
                    // InitializeOrderDisplay();

                    // Reset to Dine-In
                    if (cmbOrderType != null) cmbOrderType.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Payment failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void cmbOrderType1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}