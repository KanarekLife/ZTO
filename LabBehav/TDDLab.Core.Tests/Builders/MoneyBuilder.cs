using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests.Builders
{
    public class MoneyBuilder
    {
        private ulong _amount = 100;
        private const string Currency = Money.DefaultCurrency;

        public static MoneyBuilder Valid() => new();

        public MoneyBuilder WithAmount(ulong amount)
        {
            _amount = amount;
            return this;
        }

        public Money Build() => new(_amount, Currency);
    }
}
