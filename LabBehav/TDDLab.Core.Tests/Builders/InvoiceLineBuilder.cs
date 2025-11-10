using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests.Builders
{
    public class InvoiceLineBuilder
    {
        private string _productName = "P1";
        private Money _money = MoneyBuilder.Valid().Build();

        public static InvoiceLineBuilder Valid() => new();

        public InvoiceLineBuilder WithProduct(string productName) { _productName = productName; return this; }
        public InvoiceLineBuilder WithMoney(Money money) { _money = money; return this; }

        public InvoiceLine Build() => new(_productName, _money);
    }
}
