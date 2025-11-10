using System.Reflection;
using BasicUtils;

namespace TDDLab.Core.InvoiceMgmt
{
    public class CurrencyConverter
    {
        public static ulong Convert(string from, string to, ulong amount)
        {
            return amount;
        }
    }
    public class Money : ValidatedDomainObject
    {
        public Money(ulong amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Money(ulong amount) : this(amount,DefaultCurrency){}

        public const string DefaultCurrency = "USD";

        public ulong Amount { get; private set; }

        public string Currency { get; private set; }

        public static Money ZERO
        {
            get { return new Money(0); } 
        }
        
        public static Money operator +(Money left,Money right)
        {
            return new Money(left.Amount +right.ToCurrency(left.Currency).Amount,left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            var rightAmount = right.ToCurrency(left.Currency).Amount;
            ulong result = 0;
            if (left.Amount > rightAmount)
                result = left.Amount - rightAmount;
            return new Money(result, left.Currency); ;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Amount, Currency);
        }

        public sealed class ValidationRules
        {
            public static IBusinessRule<Money> Currency
            {
                get
                {
                    return new BusinessRule<Money>(MethodBase.GetCurrentMethod().Name, "Currency should be specified", money => money.Currency.IsNotEmpty());
                }
            }
        }
        protected override IBusinessRuleSet Rules
        {
            get
            {
                return new BusinessRuleSet<Money>(
                    ValidationRules.Currency);
            }
        }

        #region equality

        public bool Equals(Money obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Amount == Amount && Equals(obj.Currency, Currency);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Money)) return false;
            return Equals((Money) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Amount.GetHashCode()*397) ^ (Currency != null ? Currency.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Money left, Money right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Money left, Money right)
        {
            return !Equals(left, right);
        }

        #endregion

    }
}