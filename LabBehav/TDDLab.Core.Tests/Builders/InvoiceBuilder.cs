using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests.Builders
{
    public class InvoiceBuilder
    {
        private const string InvoiceNumber = "INV-1";
        private readonly Recipient _recipient = RecipientBuilder.BuildValid();
        private readonly Address _billTo = AddressBuilder.BuildValid();
        private List<InvoiceLine> _lines = [InvoiceLineBuilder.Valid().Build()];
        private Money _discount = MoneyBuilder.Valid().WithAmount(10).Build();

        public static InvoiceBuilder Valid() => new();
        public InvoiceBuilder WithLines(IEnumerable<InvoiceLine> lines) { _lines = new List<InvoiceLine>(lines); return this; }
        public InvoiceBuilder WithDiscount(Money discount) { _discount = discount; return this; }

        public Invoice Build() => new(InvoiceNumber, _recipient, _billTo, _lines, _discount);

        public Invoice BuildInvalidEmptyLines()
        {
            return new Invoice(InvoiceNumber, _recipient, _billTo, new List<InvoiceLine>(), _discount);
        }
    }
}
