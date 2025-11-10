using System.Reflection;
using BasicUtils;

namespace TDDLab.Core.InvoiceMgmt
{
    public class Recipient : ValidatedDomainObject
    {
        public Recipient(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        public string Name { get; private set; }

        public Address Address { get; private set; }

        public sealed class ValidationRules
        {
            public static IBusinessRule<Recipient> Name
            {
                get
                {
                    return new BusinessRule<Recipient>(MethodBase.GetCurrentMethod().Name, "Recipient name should be specified", recipient => recipient.Name.IsNotEmpty());
                }
            }
            public static IBusinessRule<Recipient> Address
            {
                get
                {
                    return new BusinessRule<Recipient>(MethodBase.GetCurrentMethod().Name, "Address should be valid", recipient => recipient!=null && recipient.Address.IsValid);
                }
            }
        }
        protected override IBusinessRuleSet Rules
        {
            get
            {
                return new BusinessRuleSet<Recipient>(
                    ValidationRules.Name,
                    ValidationRules.Address);
            }
        }
    }
}