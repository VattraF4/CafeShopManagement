using System;
using System.Collections.Generic;
using OOADCafeShopManagement.Strategy;

namespace OOADCafeShopManagement.Helper
{
    /// <summary>
    /// Helper class to easily integrate Discount Strategy pattern with UI
    /// Provides simple methods for discount selection
    /// </summary>
    public static class DiscountHelper
    {
        /// <summary>
        /// Get list of available discount types for ComboBox/DropDown
        /// </summary>
        public static List<DiscountOption> GetDiscountOptions()
        {
            return new List<DiscountOption>
            {
                new DiscountOption { Id = 0, Name = "No Discount", Type = "None" },
                new DiscountOption { Id = 1, Name = "5% Off", Type = "Percentage", Value = 5.0m },
                new DiscountOption { Id = 2, Name = "10% Off", Type = "Percentage", Value = 10.0m },
                new DiscountOption { Id = 3, Name = "15% Off", Type = "Percentage", Value = 15.0m },
                new DiscountOption { Id = 4, Name = "20% Off", Type = "Percentage", Value = 20.0m },
                new DiscountOption { Id = 5, Name = "25% Off", Type = "Percentage", Value = 25.0m },
                new DiscountOption { Id = 6, Name = "$1 Off", Type = "Fixed", Value = 1.0m },
                new DiscountOption { Id = 7, Name = "$2 Off", Type = "Fixed", Value = 2.0m },
                new DiscountOption { Id = 8, Name = "$5 Off", Type = "Fixed", Value = 5.0m },
                new DiscountOption { Id = 9, Name = "$10 Off", Type = "Fixed", Value = 10.0m },
                new DiscountOption { Id = 10, Name = "Buy 1 Get 1 Free", Type = "BOGO", Value = 0 },
                new DiscountOption { Id = 11, Name = "Buy 2 Get 1 Free", Type = "BuyXGetY", Value = 2 }
            };
        }

        /// <summary>
        /// Create discount strategy based on option selected
        /// </summary>
        public static IDiscountStrategy CreateDiscountStrategy(DiscountOption option)
        {
            if (option == null || option.Type == "None")
                return new NoDiscountStrategy();

            switch (option.Type)
            {
                case "Percentage":
                    return new PercentageDiscountStrategy(option.Value);
                
                case "Fixed":
                    return new FixedAmountDiscountStrategy(option.Value);
                
                case "BOGO":
                    return new BuyOneGetOneStrategy();
                
                case "BuyXGetY":
                    return new BuyXGetYFreeStrategy((int)option.Value, 1);
                
                default:
                    return new NoDiscountStrategy();
            }
        }

        /// <summary>
        /// Calculate discount amount
        /// </summary>
        public static decimal CalculateDiscount(DiscountOption option, decimal amount, decimal quantity = 1)
        {
            var strategy = CreateDiscountStrategy(option);
            return strategy.CalculateDiscount(amount, quantity);
        }

        /// <summary>
        /// Calculate discount from custom percentage
        /// </summary>
        public static decimal CalculatePercentageDiscount(decimal percentage, decimal amount, decimal quantity = 1)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100");

            var strategy = new PercentageDiscountStrategy(percentage);
            return strategy.CalculateDiscount(amount, quantity);
        }

        /// <summary>
        /// Calculate discount from fixed amount
        /// </summary>
        public static decimal CalculateFixedDiscount(decimal fixedAmount, decimal totalAmount)
        {
            if (fixedAmount < 0)
                throw new ArgumentException("Discount amount cannot be negative");

            var strategy = new FixedAmountDiscountStrategy(fixedAmount);
            return strategy.CalculateDiscount(totalAmount);
        }
    }

    /// <summary>
    /// Discount option for UI selection
    /// </summary>
    public class DiscountOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
