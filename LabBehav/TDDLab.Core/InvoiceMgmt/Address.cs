using System.Reflection;
using BasicUtils;

namespace TDDLab.Core.InvoiceMgmt
{
    public class Address : ValidatedDomainObject, IValidatedObject
    {
        public Address(string addresLine1, string city, string state, string zip)
        {
            AddressLine1 = addresLine1;
            City = city;
            State = state;
            Zip = zip;
        }

        public string AddressLine1 { get; private set; }

        public string City { get; private set; }

        public string State { get; private set; }

        public string Zip { get; private set; }

        public sealed class ValidationRules
        {
            public static IBusinessRule<Address> AddressLine1
            {
                get
                {
                    return new BusinessRule<Address>(MethodBase.GetCurrentMethod().Name, "AddressLine1 should be specified", address => address.AddressLine1.IsNotEmpty());
                }
            }
            public static IBusinessRule<Address> City
            {
                get
                {
                    return new BusinessRule<Address>(MethodBase.GetCurrentMethod().Name, "City should be specified", address => address.City.IsNotEmpty());
                }
            }
            public static IBusinessRule<Address> Zip
            {
                get
                {
                    return new BusinessRule<Address>(MethodBase.GetCurrentMethod().Name, "Zip code should be specified", address => address.Zip.IsNotEmpty());
                }
            }
            public static IBusinessRule<Address> State
            {
                get
                {
                    return new BusinessRule<Address>(MethodBase.GetCurrentMethod().Name, "State should be properly specified", address => address.State.IsNotEmpty());
                }
            }
        }
        protected override IBusinessRuleSet Rules
        {
            get
            {
                return new BusinessRuleSet<Address>(
                    ValidationRules.AddressLine1,
                    ValidationRules.City,
                    ValidationRules.Zip,
                    ValidationRules.State);
            }
        }
    }
}