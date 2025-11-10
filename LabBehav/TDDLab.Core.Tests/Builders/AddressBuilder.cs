using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests.Builders
{
    public static class AddressBuilder
    {
        private const string ValidLine = "123 Main St";
        private const string ValidCity = "Springfield";
        private const string ValidState = "CA";
        private const string ValidZip = "90210";

        public static Address BuildValid() => new(ValidLine, ValidCity, ValidState, ValidZip);
    }
}
