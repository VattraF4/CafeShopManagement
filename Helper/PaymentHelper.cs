using System;
using System.Collections.Generic;
using OOADCafeShopManagement.Strategy;

namespace OOADCafeShopManagement.Helper
{
    /// <summary>
    /// Helper class to easily integrate Payment Strategy pattern with UI
    /// Provides simple methods for payment method selection
    /// </summary>
    public static class PaymentHelper
    {
        /// <summary>
        /// Get list of available payment methods for ComboBox/DropDown
        /// </summary>
        public static List<PaymentOption> GetPaymentOptions()
        {
            return new List<PaymentOption>
            {
                new PaymentOption { Id = 1, Name = "Cash", Type = "Cash" },
                new PaymentOption { Id = 2, Name = "Credit/Debit Card", Type = "Card" },
                new PaymentOption { Id = 3, Name = "E-Wallet (PayPal/Apple Pay)", Type = "EWallet" },
                new PaymentOption { Id = 4, Name = "Mobile Payment (QR)", Type = "Mobile" },
                new PaymentOption { Id = 5, Name = "Gift Card", Type = "GiftCard" },
                new PaymentOption { Id = 6, Name = "Loyalty Points", Type = "Points" }
            };
        }

        /// <summary>
        /// Create payment strategy based on selection
        /// </summary>
        public static IPaymentStrategy CreatePaymentStrategy(PaymentOption option, params string[] additionalParams)
        {
            if (option == null)
                return new CashPaymentStrategy();

            switch (option.Type)
            {
                case "Cash":
                    return new CashPaymentStrategy();
                
                case "Card":
                    // additionalParams[0] = cardNumber, additionalParams[1] = cardHolderName
                    string cardNumber = additionalParams.Length > 0 ? additionalParams[0] : "0000000000000000";
                    string cardHolder = additionalParams.Length > 1 ? additionalParams[1] : "Customer";
                    return new CardPaymentStrategy(cardNumber, cardHolder);
                
                case "EWallet":
                    // additionalParams[0] = walletType, additionalParams[1] = accountId
                    string walletType = additionalParams.Length > 0 ? additionalParams[0] : "PayPal";
                    string accountId = additionalParams.Length > 1 ? additionalParams[1] : "customer@email.com";
                    return new EWalletPaymentStrategy(walletType, accountId);
                
                case "Mobile":
                    // additionalParams[0] = phoneNumber
                    string phone = additionalParams.Length > 0 ? additionalParams[0] : "000-000-0000";
                    return new MobilePaymentStrategy(phone);
                
                case "GiftCard":
                    // additionalParams[0] = giftCardNumber, additionalParams[1] = balance
                    string gcNumber = additionalParams.Length > 0 ? additionalParams[0] : "GC000000";
                    decimal balance = additionalParams.Length > 1 ? decimal.Parse(additionalParams[1]) : 100.0m;
                    return new GiftCardPaymentStrategy(gcNumber, balance);
                
                case "Points":
                    // additionalParams[0] = points, additionalParams[1] = pointsRatio
                    int points = additionalParams.Length > 0 ? int.Parse(additionalParams[0]) : 1000;
                    decimal ratio = additionalParams.Length > 1 ? decimal.Parse(additionalParams[1]) : 100;
                    return new PointsPaymentStrategy(points, ratio);
                
                default:
                    return new CashPaymentStrategy();
            }
        }

        /// <summary>
        /// Create simple cash payment
        /// </summary>
        public static IPaymentStrategy CreateCashPayment()
        {
            return new CashPaymentStrategy();
        }

        /// <summary>
        /// Create card payment
        /// </summary>
        public static IPaymentStrategy CreateCardPayment(string cardNumber, string cardHolderName)
        {
            return new CardPaymentStrategy(cardNumber, cardHolderName);
        }

        /// <summary>
        /// Create e-wallet payment
        /// </summary>
        public static IPaymentStrategy CreateEWalletPayment(string walletType, string accountId)
        {
            return new EWalletPaymentStrategy(walletType, accountId);
        }

        /// <summary>
        /// Process payment with default cash method
        /// </summary>
        public static bool ProcessCashPayment(decimal amount)
        {
            var context = new PaymentContext();
            context.SetPaymentStrategy(new CashPaymentStrategy());
            context.SetDiscountStrategy(new NoDiscountStrategy());
            return context.ProcessPayment(amount);
        }

        /// <summary>
        /// Process payment with discount
        /// </summary>
        public static PaymentReceipt ProcessPaymentWithDiscount(
            decimal amount, 
            decimal quantity,
            PaymentOption paymentOption,
            DiscountOption discountOption,
            params string[] paymentParams)
        {
            var context = new PaymentContext();
            
            // Set payment strategy
            var paymentStrategy = CreatePaymentStrategy(paymentOption, paymentParams);
            context.SetPaymentStrategy(paymentStrategy);
            
            // Set discount strategy
            var discountStrategy = DiscountHelper.CreateDiscountStrategy(discountOption);
            context.SetDiscountStrategy(discountStrategy);
            
            // Process payment
            bool success = context.ProcessPayment(amount, quantity);
            
            // Return receipt
            return context.GetPaymentReceipt(amount, quantity);
        }
    }

    /// <summary>
    /// Payment option for UI selection
    /// </summary>
    public class PaymentOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
