namespace TDDLab.Core.InvoiceMgmt
{
    public static class DomainExtensions
    {
        public static Money ToCurrency(this Money money, string currency)
        {
            return new Money(CurrencyConverter.Convert(money.Currency, currency, money.Amount), currency);
        }
    }
}