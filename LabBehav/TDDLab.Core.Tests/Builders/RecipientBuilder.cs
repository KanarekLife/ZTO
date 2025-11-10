using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests.Builders
{
    public static class RecipientBuilder
    {
        private const string Name = "John Doe";

        public static Recipient BuildValid() => new(Name, AddressBuilder.BuildValid());
    }
}
