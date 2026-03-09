using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using OOADCafeShopManagement.Strategy;
using OOADCafeShopManagement.Helper;


namespace OOADCafeShopManagement.Models
{
    class OrderManager
    {
        private Order _currentOrder;
        private BindingList<OrderDetail> _currentItems;
        private OrderDetail _selectedItem; // Single temporary item
        private IDiscountStrategy _discountStrategy; // Strategy Pattern
        private IPaymentStrategy _paymentStrategy;   // Strategy Pattern
        private IOrderTypeStrategy _orderTypeStrategy; // Order Type Strategy Pattern

        //Properties Get Only
        public Order CurrentOrder => _currentOrder;
        public BindingList<OrderDetail> CurrentItems => _currentItems;
        public OrderDetail SelectedItem => _selectedItem;
        public decimal GrandTotal => _currentOrder.TotalAmount + _currentOrder.ServiceCharge - _currentOrder.Discount;

        public OrderManager()
        {
            InitializeNewOrder();
            // Initialize default strategies
            _discountStrategy = new NoDiscountStrategy();
            _paymentStrategy = new CashPaymentStrategy();
            _orderTypeStrategy = new DineInOrderStrategy(); // Default to Dine-In
        }

        private void InitializeNewOrder()
        {
            _currentOrder = new Order
            {
                OrderDate = DateTime.Now,
                Status = "Pending",
                UserID = UserSession.UserId
            };
            _currentItems = new BindingList<OrderDetail>();
            _selectedItem = new OrderDetail(); // Initialize empty selected item
        }

        // Method to set the selected product (when clicking on grid)
        public void SetSelectedProduct(int productId, string productName, decimal unitPrice, decimal discount)
        {
            _selectedItem = new OrderDetail
            {
                ProductID = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = 1, // Default quantity
                Discount = discount  // Default discount
            };
        }

        // Method to update the selected item (when user changes quantity/discount)
        public void UpdateSelectedItem(decimal quantity, decimal discount)
        {
            if (_selectedItem != null)
            {
                _selectedItem.Quantity = quantity;
                _selectedItem.Discount = discount;
            }
        }

        // Add the selected item to the order list
        public void AddSelectedItemToOrder()
        {
            if (_selectedItem == null || _selectedItem.ProductID == 0)
            {
                throw new InvalidOperationException("No product selected. Please select a product first.");
            }

            if (_selectedItem.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be positive");
            }

            if (_selectedItem.Discount < 0)
            {
                throw new ArgumentException("Discount cannot be negative");
            }

            // Check if product already exists in order
            var existingItem = _currentItems.FirstOrDefault(item => item.ProductID == _selectedItem.ProductID);

            if (existingItem != null)
            {
                // Update existing item
                existingItem.Quantity += _selectedItem.Quantity;
                existingItem.Discount += _selectedItem.Discount;
            }
            else
            {
                // Create new order detail (clone the selected item)
                var newItem = new OrderDetail
                {
                    ProductID = _selectedItem.ProductID,
                    ProductName = _selectedItem.ProductName,
                    UnitPrice = _selectedItem.UnitPrice,
                    Quantity = _selectedItem.Quantity,
                    Discount = _selectedItem.Discount,
                };
                _currentItems.Add(newItem);
            }

            UpdateOrderTotals();

            // Reset selected item after adding to order
            _selectedItem = new OrderDetail();
        }

        public void RemoveOrderItem(int productId)
        {
            var itemToRemove = _currentItems.FirstOrDefault(item => item.ProductID == productId);
            if (itemToRemove != null)
            {
                _currentItems.Remove(itemToRemove);
                UpdateOrderTotals();
            }
        }

        private void UpdateOrderTotals()
        {
            _currentOrder.TotalAmount = _currentItems.Sum(item => item.UnitPrice * item.Quantity);
            _currentOrder.Discount = _currentItems.Sum(item => item.Discount);
        }

        public bool ProcessPayment(string note, string status)
        {
            try
            {
                // Set order details - create fresh values
                _currentOrder.Note = note;
                _currentOrder.Status = status;
                _currentOrder.OrderDate = DateTime.Now;

                // Save to database
                bool success = SaveOrderToDatabase();

                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Payment processing error: {ex.Message}");
                return false;
            }
        }

        public (decimal totalAmount, decimal discount, decimal grandTotal) GetOrderSummary()
        {

            return (_currentOrder.TotalAmount, _currentOrder.Discount, GrandTotal);
        }

        // ===== STRATEGY PATTERN METHODS =====

        /// <summary>
        /// Set discount strategy for the order
        /// </summary>
        public void SetDiscountStrategy(IDiscountStrategy strategy)
        {
            _discountStrategy = strategy ?? new NoDiscountStrategy();
        }

        /// <summary>
        /// Set payment strategy for the order
        /// </summary>
        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            _paymentStrategy = strategy ?? new CashPaymentStrategy();
        }

        /// <summary>
        /// Apply discount using current strategy
        /// </summary>
        public void ApplyDiscount()
        {
            if (_discountStrategy == null)
            {
                _discountStrategy = new NoDiscountStrategy();
            }

            decimal totalDiscount = 0;
            foreach (var item in _currentItems)
            {
                decimal itemDiscount = _discountStrategy.CalculateDiscount(item.UnitPrice, item.Quantity);
                item.Discount = itemDiscount;
                totalDiscount += itemDiscount;
            }

            _currentOrder.Discount = totalDiscount;
        }

        /// <summary>
        /// Process payment using current payment strategy
        /// </summary>
        public bool ProcessPaymentWithStrategy(string note = "")
        {
            if (_paymentStrategy == null)
            {
                _paymentStrategy = new CashPaymentStrategy();
            }

            // Calculate final amount
            decimal finalAmount = GrandTotal;

            // Process payment
            bool paymentSuccess = _paymentStrategy.ProcessPayment(finalAmount);

            if (paymentSuccess)
            {
                // Save order to database
                _currentOrder.Note = note;
                _currentOrder.Status = "Completed";
                return SaveOrderToDatabase();
            }

            return false;
        }

        /// <summary>
        /// Get payment receipt
        /// </summary>
        public string GetPaymentReceipt()
        {
            var context = new PaymentContext();
            context.SetDiscountStrategy(_discountStrategy);
            context.SetPaymentStrategy(_paymentStrategy);

            var receipt = context.GetPaymentReceipt(_currentOrder.TotalAmount, 1);
            return receipt.ToString();
        }

        /// <summary>
        /// Quick method to set discount from helper
        /// </summary>
        public void SetDiscountOption(DiscountOption option)
        {
            _discountStrategy = DiscountHelper.CreateDiscountStrategy(option);
            ApplyDiscount();
        }

        /// <summary>
        /// Quick method to set payment from helper
        /// </summary>
        public void SetPaymentOption(PaymentOption option, params string[] additionalParams)
        {
            _paymentStrategy = PaymentHelper.CreatePaymentStrategy(option, additionalParams);
        }

        // ===== ORDER TYPE STRATEGY PATTERN METHODS =====

        /// <summary>
        /// Set order type strategy (Dine-In, Takeaway, Delivery)
        /// </summary>
        public void SetOrderTypeStrategy(IOrderTypeStrategy strategy)
        {
            _orderTypeStrategy = strategy ?? new DineInOrderStrategy();
        }

        /// <summary>
        /// Apply order type calculations (service charges, packaging fees, etc.)
        /// </summary>
        public void ApplyOrderTypeCalculations()
        {
            if (_orderTypeStrategy == null)
            {
                _orderTypeStrategy = new DineInOrderStrategy();
            }

            int itemCount = _currentItems.Count;
            decimal totalAmount = _currentOrder.TotalAmount;

            // Calculate charges using strategy
            var (serviceCharge, packagingFee, tax) = _orderTypeStrategy.CalculateCharges(totalAmount, itemCount);

            // Store in order
            _currentOrder.ServiceCharge = serviceCharge + packagingFee;
            _currentOrder.Tax = tax;
        }

        /// <summary>
        /// Get order type name
        /// </summary>
        public string GetOrderTypeName()
        {
            return _orderTypeStrategy?.GetOrderTypeName() ?? "Dine-In";
        }

        // Clear current order for new transaction
        // In OrderManager class
        public void ClearOrder()
        {
            _currentItems.Clear(); // Clear all items from the list

            // Preserve the UserID when creating new order
            int currentUserId = _currentOrder.UserID; // Save current user ID

            _currentOrder = new Order // Create a brand new order object
            {
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0,
                Discount = 0,
                OrderID = 0, // Reset OrderID
                UserID = currentUserId // Preserve the UserID
            };
            _selectedItem = new OrderDetail(); // Reset selected item

            System.Diagnostics.Debug.WriteLine($"Order cleared. UserID preserved: {currentUserId}");
        }

        public void ClearSelectedItem()
        {
            _selectedItem = new OrderDetail();
        }

        private bool SaveOrderToDatabase()
        {
            using (var connection = DbConnection.Instance.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // STEP 1: Insert into Orders table first
                        string orderQuerySql = @"INSERT INTO Orders (status, total_amount, discount,user_id, note)
                                        VALUES (@Status, @TotalAmount, @Discount,@UserId, @Note)
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int newOrderId;
                        using (var command = new SqlCommand(orderQuerySql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Status", _currentOrder.Status);
                            command.Parameters.AddWithValue("@TotalAmount", _currentOrder.TotalAmount);
                            command.Parameters.AddWithValue("@Discount", _currentOrder.Discount);
                            command.Parameters.AddWithValue("@UserId", _currentOrder.UserID);
                            command.Parameters.AddWithValue("@Note", _currentOrder.Note ?? (object)DBNull.Value);
                            //command.Parameters.AddWithValue("@OrderDate", _currentOrder.OrderDate);

                            newOrderId = (int)command.ExecuteScalar();
                        }

                        if (newOrderId <= 0)
                        {
                            throw new Exception("Failed to retrieve new Order ID.");
                        }

                        // Store the new OrderID in the current order
                        _currentOrder.OrderID = newOrderId;

                        // STEP 2: Insert into Order_Details table using the new OrderID
                        string detailQuerySql = @"INSERT INTO order_details (order_id, product_id, quantity, unit_price, sub_total)
                                        VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @SubTotal)";

                        foreach (var item in _currentItems)
                        {
                            using (var command = new SqlCommand(detailQuerySql, connection, transaction))
                            {
                                decimal subTotal = (item.UnitPrice * item.Quantity) - item.Discount;

                                command.Parameters.AddWithValue("@OrderID", newOrderId); // Use the new OrderID
                                command.Parameters.AddWithValue("@ProductID", item.ProductID);
                                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                                command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                //command.Parameters.AddWithValue("@Discount", item.Discount);
                                command.Parameters.AddWithValue("@SubTotal", subTotal);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}