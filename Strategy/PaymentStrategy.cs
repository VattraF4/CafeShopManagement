using System;

namespace OOADCafeShopManagement.Strategy
{
    /// <summary>
    /// Strategy Pattern for payment processing
    /// Allows different payment methods
    /// </summary>
    public interface IPaymentStrategy
    {
        bool ProcessPayment(decimal amount);
        string GetPaymentMethod();
        string GetTransactionId();
    }

    /// <summary>
    /// Cash payment strategy
    /// </summary>
    public class CashPaymentStrategy : IPaymentStrategy
    {
        private string _transactionId;

        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            // Simulate cash payment processing
            _transactionId = $"CASH-{DateTime.Now:yyyyMMddHHmmss}";
            
            // Cash payments are always successful (assuming correct amount given)
            return true;
        }

        public string GetPaymentMethod()
        {
            return "Cash";
        }

        public string GetTransactionId()
        {
            return _transactionId ?? "N/A";
        }
    }

    /// <summary>
    /// Credit/Debit Card payment strategy
    /// </summary>
    public class CardPaymentStrategy : IPaymentStrategy
    {
        private readonly string _cardNumber;
        private readonly string _cardHolderName;
        private string _transactionId;

        public CardPaymentStrategy(string cardNumber, string cardHolderName)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new ArgumentException("Card number is required.", nameof(cardNumber));
            
            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new ArgumentException("Card holder name is required.", nameof(cardHolderName));
            
            _cardNumber = cardNumber;
            _cardHolderName = cardHolderName;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            // Simulate card payment processing
            // In real system, this would call payment gateway API
            
            // Validate card (simple check for demo)
            if (_cardNumber.Length < 13 || _cardNumber.Length > 19)
            {
                return false; // Invalid card
            }

            _transactionId = $"CARD-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
            
            // Simulate successful payment (90% success rate for demo)
            return new Random().Next(100) < 90;
        }

        public string GetPaymentMethod()
        {
            // Mask card number (show last 4 digits)
            string maskedCard = "****-****-****-" + _cardNumber.Substring(_cardNumber.Length - 4);
            return $"Card ({maskedCard})";
        }

        public string GetTransactionId()
        {
            return _transactionId ?? "N/A";
        }
    }

    /// <summary>
    /// E-Wallet payment strategy (e.g., PayPal, Apple Pay, Google Pay)
    /// </summary>
    public class EWalletPaymentStrategy : IPaymentStrategy
    {
        private readonly string _walletType;
        private readonly string _accountId;
        private string _transactionId;

        public EWalletPaymentStrategy(string walletType, string accountId)
        {
            if (string.IsNullOrWhiteSpace(walletType))
                throw new ArgumentException("Wallet type is required.", nameof(walletType));
            
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Account ID is required.", nameof(accountId));
            
            _walletType = walletType;
            _accountId = accountId;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            // Simulate e-wallet payment processing
            // In real system, this would call e-wallet API
            
            _transactionId = $"{_walletType.ToUpper()}-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            // Simulate successful payment (95% success rate for demo)
            return new Random().Next(100) < 95;
        }

        public string GetPaymentMethod()
        {
            return $"{_walletType} ({_accountId})";
        }

        public string GetTransactionId()
        {
            return _transactionId ?? "N/A";
        }
    }

    /// <summary>
    /// Mobile payment strategy (QR code based)
    /// </summary>
    public class MobilePaymentStrategy : IPaymentStrategy
    {
        private readonly string _phoneNumber;
        private string _transactionId;

        public MobilePaymentStrategy(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
            
            _phoneNumber = phoneNumber;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            // Simulate mobile payment processing (QR code scan)
            _transactionId = $"MOBILE-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(10000, 99999)}";
            
            // Simulate successful payment
            return true;
        }

        public string GetPaymentMethod()
        {
            // Mask phone number
            string masked = _phoneNumber.Substring(0, 3) + "****" + _phoneNumber.Substring(_phoneNumber.Length - 2);
            return $"Mobile Pay ({masked})";
        }

        public string GetTransactionId()
        {
            return _transactionId ?? "N/A";
        }
    }

    /// <summary>
    /// Gift Card payment strategy
    /// </summary>
    public class GiftCardPaymentStrategy : IPaymentStrategy
    {
        private readonly string _giftCardNumber;
        private decimal _balance;
        private string _transactionId;

        public GiftCardPaymentStrategy(string giftCardNumber, decimal balance)
        {
            if (string.IsNullOrWhiteSpace(giftCardNumber))
                throw new ArgumentException("Gift card number is required.", nameof(giftCardNumber));
            
            if (balance < 0)
                throw new ArgumentException("Balance cannot be negative.", nameof(balance));
            
            _giftCardNumber = giftCardNumber;
            _balance = balance;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            // Check if sufficient balance
            if (_balance < amount)
            {
                return false; // Insufficient balance
            }

            // Deduct amount from balance
            _balance -= amount;
            _transactionId = $"GIFTCARD-{DateTime.Now:yyyyMMddHHmmss}";
            
            return true;
        }

        public string GetPaymentMethod()
        {
            // Mask gift card number
            string masked = "****-" + _giftCardNumber.Substring(_giftCardNumber.Length - 4);
            return $"Gift Card ({masked}) - Balance: ${_balance:F2}";
        }

        public string GetTransactionId()
        {
            return _transactionId ?? "N/A";
        }

        public decimal GetRemainingBalance()
        {
            return _balance;
        }
    }

    /// <summary>
    /// Points/Loyalty payment strategy
    /// </summary>
    public class PointsPaymentStrategy : IPaymentStrategy
    {
        private int _points;
        private readonly decimal _pointsToMoneyRatio; // e.g., 100 points = $1
        private string _transactionId;

        public PointsPaymentStrategy(int points, decimal pointsToMoneyRatio = 100)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative.", nameof(points));
            
            if (pointsToMoneyRatio <= 0)
                throw new ArgumentException("Points ratio must be positive.", nameof(pointsToMoneyRatio));
            
            _points = points;
            _pointsToMoneyRatio = pointsToMoneyRatio;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            // Calculate points needed
            int pointsNeeded = (int)(amount * _pointsToMoneyRatio);
            
            if (_points < pointsNeeded)
            {
                return false; // Insufficient points
            }

            // Deduct points
            _points -= pointsNeeded;
            _transactionId = $"POINTS-{DateTime.Now:yyyyMMddHHmmss}";
            
            return true;
        }

        public string GetPaymentMethod()
        {
            return $"Loyalty Points (Available: {_points})";
        }

        public string GetTransactionId()
        {
            return _transactionId ?? "N/A";
        }

        public int GetRemainingPoints()
        {
            return _points;
        }
    }
}
