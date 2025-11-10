using System.Reflection;
using IBusinessRuleSet = BasicUtils.IBusinessRuleSet;
using BasicUtils;


namespace TDDLab.Core.InvoiceMgmt
{
    public class InvoiceLine : ValidatedDomainObject
    {
        private Invoice invoice;

        public InvoiceLine(string productName, Money money)
        {
            ProductName = productName;
            Money = money;
        }

        public Money Money { get; private set; }

        public string ProductName { get; private set; }

        public Invoice Invoice
        {
            set { invoice = value; }
            get { return invoice; }
        }

        public sealed class ValidationRules
        {
            public static IBusinessRule<InvoiceLine> ProductName
            {
                get
                {
                    return new BusinessRule<InvoiceLine>(MethodBase.GetCurrentMethod().Name, "Product name should be specified", line => line.ProductName.IsNotEmpty());
                }
            }
            public static IBusinessRule<InvoiceLine> Money
            {
                get
                {
                    return new BusinessRule<InvoiceLine>(MethodBase.GetCurrentMethod().Name, "Money should be valid", line => line.Money!=null && line.Money.IsValid);
                }
            }
        }
        protected override IBusinessRuleSet Rules
        {
            get
            {
                return new BusinessRuleSet<InvoiceLine>(
                    ValidationRules.ProductName,
                    ValidationRules.Money);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} for {1}", ProductName, Money);
        }
    }
}