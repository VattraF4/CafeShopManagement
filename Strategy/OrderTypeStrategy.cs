using System;

namespace OOADCafeShopManagement.Strategy
{
    /// <summary>
    /// Strategy Pattern for different Order Types
    /// Each order type has different behavior (fees, tax, packaging, etc.)
    /// </summary>
    public interface IOrderTypeStrategy
    {
        /// <summary>
        /// Get the order type name
        /// </summary>
        string GetOrderTypeName();

        /// <summary>
        /// Calculate service charge or delivery fee
        /// </summary>
        decimal CalculateServiceCharge(decimal totalAmount);

        /// <summary>
        /// Calculate packaging fee (for takeaway/delivery)
        /// </summary>
        decimal CalculatePackagingFee(int itemCount);

        /// <summary>
        /// Get tax rate (some types may have different tax)
        /// </summary>
        decimal GetTaxRate();

        /// <summary>
        /// Apply order type specific calculations
        /// </summary>
        (decimal serviceCharge, decimal packagingFee, decimal tax) CalculateCharges(decimal totalAmount, int itemCount);
    }

    /// <summary>
    /// Dine-In Order Strategy
    /// - No packaging fee
    /// - Table service charge
    /// - Standard tax
    /// </summary>
    public class DineInOrderStrategy : IOrderTypeStrategy
    {
        public string GetOrderTypeName() => "Dine-In";

        public decimal CalculateServiceCharge(decimal totalAmount)
        {
            // 10% service charge for dine-in
            return totalAmount * 0.10m;
        }

        public decimal CalculatePackagingFee(int itemCount)
        {
            // No packaging for dine-in
            return 0m;
        }

        public decimal GetTaxRate()
        {
            // Standard 7% tax
            return 0.07m;
        }

        public (decimal serviceCharge, decimal packagingFee, decimal tax) CalculateCharges(decimal totalAmount, int itemCount)
        {
            decimal serviceCharge = CalculateServiceCharge(totalAmount);
            decimal packagingFee = CalculatePackagingFee(itemCount);
            decimal tax = totalAmount * GetTaxRate();
            return (serviceCharge, packagingFee, tax);
        }
    }

    /// <summary>
    /// Takeaway Order Strategy
    /// - Packaging fee per item
    /// - No service charge
    /// - Standard tax
    /// </summary>
    public class TakeawayOrderStrategy : IOrderTypeStrategy
    {
        public string GetOrderTypeName() => "Takeaway";

        public decimal CalculateServiceCharge(decimal totalAmount)
        {
            // No service charge for takeaway
            return 0m;
        }

        public decimal CalculatePackagingFee(int itemCount)
        {
            // $0.50 per item for packaging
            return itemCount * 0.50m;
        }

        public decimal GetTaxRate()
        {
            // Standard 7% tax
            return 0.07m;
        }

        public (decimal serviceCharge, decimal packagingFee, decimal tax) CalculateCharges(decimal totalAmount, int itemCount)
        {
            decimal serviceCharge = CalculateServiceCharge(totalAmount);
            decimal packagingFee = CalculatePackagingFee(itemCount);
            decimal tax = totalAmount * GetTaxRate();
            return (serviceCharge, packagingFee, tax);
        }
    }

    /// <summary>
    /// Delivery Order Strategy
    /// - Delivery fee based on amount
    /// - Packaging fee per item
    /// - Standard tax
    /// </summary>
    public class DeliveryOrderStrategy : IOrderTypeStrategy
    {
        public string GetOrderTypeName() => "Delivery";

        public decimal CalculateServiceCharge(decimal totalAmount)
        {
            // Delivery fee: $5 flat or free if over $15
            if (totalAmount >= 15m)
                return 0m; // Free delivery
            else
                return 2.00m; // $2 delivery fee
        }

        public decimal CalculatePackagingFee(int itemCount)
        {
            // $0.75 per item for delivery packaging (more secure)
            return itemCount * 0.50m;
        }

        public decimal GetTaxRate()
        {
            // Standard 7% tax
            return 0.07m;
        }

        public (decimal serviceCharge, decimal packagingFee, decimal tax) CalculateCharges(decimal totalAmount, int itemCount)
        {
            decimal serviceCharge = CalculateServiceCharge(totalAmount);
            decimal packagingFee = CalculatePackagingFee(itemCount);
            decimal tax = totalAmount * GetTaxRate();
            return (serviceCharge, packagingFee, tax);
        }
    }
}
