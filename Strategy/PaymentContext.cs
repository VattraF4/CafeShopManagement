using System;

namespace OOADCafeShopManagement.Strategy
{
    /// <summary>
    /// Context class for payment processing using Strategy Pattern
    /// Allows runtime selection of payment method
    /// </summary>
    public class PaymentContext
    {
        private IPaymentStrategy _paymentStrategy;
        private IDiscountStrategy _discountStrategy;

        public PaymentContext()
        {
            // Default strategies
            _paymentStrategy = new CashPaymentStrategy();
            _discountStrategy = new NoDiscountStrategy();
        }

        /// <summary>
        /// Set the payment method to use
        /// </summary>
        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            
            _paymentStrategy = strategy;
        }

        /// <summary>
        /// Set the discount method to use
        /// </summary>
        public void SetDiscountStrategy(IDiscountStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            
            _discountStrategy = strategy;
        }

        /// <summary>
        /// Calculate discount for given amount and quantity
        /// </summary>
        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            return _discountStrategy.CalculateDiscount(amount, quantity);
        }

        /// <summary>
        /// Process payment with current payment strategy
        /// </summary>
        public bool ProcessPayment(decimal originalAmount, decimal quantity = 1)
        {
            // Calculate total after discount
            decimal discount = CalculateDiscount(originalAmount, quantity);
            decimal finalAmount = (originalAmount * quantity) - discount;

            if (finalAmount < 0)
                finalAmount = 0;

            // Process payment
            return _paymentStrategy.ProcessPayment(finalAmount);
        }

        /// <summary>
        /// Get payment receipt information
        /// </summary>
        public PaymentReceipt GetPaymentReceipt(decimal originalAmount, decimal quantity = 1)
        {
            decimal subtotal = originalAmount * quantity;
            decimal discount = CalculateDiscount(originalAmount, quantity);
            decimal finalAmount = subtotal - discount;

            if (finalAmount < 0)
                finalAmount = 0;

            return new PaymentReceipt
            {
                Subtotal = subtotal,
                Discount = discount,
                FinalAmount = finalAmount,
                PaymentMethod = _paymentStrategy.GetPaymentMethod(),
                DiscountDescription = _discountStrategy.GetDescription(),
                TransactionId = _paymentStrategy.GetTransactionId(),
                TransactionDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get current payment method
        /// </summary>
        public string GetPaymentMethod()
        {
            return _paymentStrategy.GetPaymentMethod();
        }

        /// <summary>
        /// Get current discount description
        /// </summary>
        public string GetDiscountDescription()
        {
            return _discountStrategy.GetDescription();
        }
    }

    /// <summary>
    /// Payment receipt information
    /// </summary>
    public class PaymentReceipt
    {
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string DiscountDescription { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }

        public override string ToString()
        {
            return $@"
═══════════════════════════════════
           PAYMENT RECEIPT
═══════════════════════════════════
Date: {TransactionDate:yyyy-MM-dd HH:mm:ss}
Transaction ID: {TransactionId}

Subtotal:        ${Subtotal:F2}
Discount:        ${Discount:F2} ({DiscountDescription})
───────────────────────────────────
TOTAL:           ${FinalAmount:F2}
═══════════════════════════════════
Payment Method: {PaymentMethod}
═══════════════════════════════════
        Thank you for your purchase!
═══════════════════════════════════";
        }
    }
}
