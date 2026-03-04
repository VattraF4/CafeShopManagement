using System;

namespace OOADCafeShopManagement.Strategy
{
    /// <summary>
    /// Strategy Pattern for calculating discounts
    /// Allows different discount calculation algorithms
    /// </summary>
    public interface IDiscountStrategy
    {
        decimal CalculateDiscount(decimal amount, decimal quantity = 1);
        string GetDescription();
    }

    /// <summary>
    /// No discount strategy
    /// </summary>
    public class NoDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            return 0;
        }

        public string GetDescription()
        {
            return "No Discount";
        }
    }

    /// <summary>
    /// Percentage-based discount (e.g., 10% off, 25% off)
    /// </summary>
    public class PercentageDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _percentage;

        public PercentageDiscountStrategy(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100.", nameof(percentage));
            
            _percentage = percentage;
        }

        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            return (amount * quantity) * (_percentage / 100);
        }

        public string GetDescription()
        {
            return $"{_percentage}% Off";
        }
    }

    /// <summary>
    /// Fixed amount discount (e.g., $5 off, $10 off)
    /// </summary>
    public class FixedAmountDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _discountAmount;

        public FixedAmountDiscountStrategy(decimal discountAmount)
        {
            if (discountAmount < 0)
                throw new ArgumentException("Discount amount cannot be negative.", nameof(discountAmount));
            
            _discountAmount = discountAmount;
        }

        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            decimal totalAmount = amount * quantity;
            
            // Don't apply discount if it exceeds total
            if (_discountAmount > totalAmount)
                return totalAmount;
            
            return _discountAmount;
        }

        public string GetDescription()
        {
            return $"${_discountAmount:F2} Off";
        }
    }

    /// <summary>
    /// Buy One Get One (BOGO) discount
    /// </summary>
    public class BuyOneGetOneStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            // For every 2 items, you get 1 free (50% off when buying 2+)
            int freeItems = (int)(quantity / 2);
            return amount * freeItems;
        }

        public string GetDescription()
        {
            return "Buy One Get One Free";
        }
    }

    /// <summary>
    /// Buy X Get Y Free discount (e.g., Buy 2 Get 1 Free)
    /// </summary>
    public class BuyXGetYFreeStrategy : IDiscountStrategy
    {
        private readonly int _buyQuantity;
        private readonly int _freeQuantity;

        public BuyXGetYFreeStrategy(int buyQuantity, int freeQuantity = 1)
        {
            if (buyQuantity <= 0)
                throw new ArgumentException("Buy quantity must be positive.", nameof(buyQuantity));
            
            if (freeQuantity <= 0)
                throw new ArgumentException("Free quantity must be positive.", nameof(freeQuantity));
            
            _buyQuantity = buyQuantity;
            _freeQuantity = freeQuantity;
        }

        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            int groupSize = _buyQuantity + _freeQuantity;
            int completeGroups = (int)(quantity / groupSize);
            int freeItems = completeGroups * _freeQuantity;
            
            return amount * freeItems;
        }

        public string GetDescription()
        {
            return $"Buy {_buyQuantity} Get {_freeQuantity} Free";
        }
    }

    /// <summary>
    /// Seasonal discount (higher discount during specific period)
    /// </summary>
    public class SeasonalDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _percentage;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly string _seasonName;

        public SeasonalDiscountStrategy(decimal percentage, DateTime startDate, DateTime endDate, string seasonName = "Seasonal")
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100.", nameof(percentage));
            
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date.");
            
            _percentage = percentage;
            _startDate = startDate;
            _endDate = endDate;
            _seasonName = seasonName;
        }

        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            DateTime now = DateTime.Now;
            
            // Check if current date is within season
            if (now >= _startDate && now <= _endDate)
            {
                return (amount * quantity) * (_percentage / 100);
            }
            
            return 0; // No discount outside season
        }

        public string GetDescription()
        {
            return $"{_seasonName} Special - {_percentage}% Off";
        }
    }

    /// <summary>
    /// Loyalty discount (based on customer tier)
    /// </summary>
    public class LoyaltyDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _percentage;
        private readonly string _tierName;

        public LoyaltyDiscountStrategy(string tierName, decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100.", nameof(percentage));
            
            _percentage = percentage;
            _tierName = tierName;
        }

        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            return (amount * quantity) * (_percentage / 100);
        }

        public string GetDescription()
        {
            return $"{_tierName} Member - {_percentage}% Off";
        }
    }

    /// <summary>
    /// Volume discount (discount increases with quantity)
    /// </summary>
    public class VolumeDiscountStrategy : IDiscountStrategy
    {
        private readonly int _minQuantity;
        private readonly decimal _percentage;

        public VolumeDiscountStrategy(int minQuantity, decimal percentage)
        {
            if (minQuantity <= 0)
                throw new ArgumentException("Minimum quantity must be positive.", nameof(minQuantity));
            
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100.", nameof(percentage));
            
            _minQuantity = minQuantity;
            _percentage = percentage;
        }

        public decimal CalculateDiscount(decimal amount, decimal quantity = 1)
        {
            if (quantity >= _minQuantity)
            {
                return (amount * quantity) * (_percentage / 100);
            }
            
            return 0;
        }

        public string GetDescription()
        {
            return $"Buy {_minQuantity}+ Get {_percentage}% Off";
        }
    }
}
