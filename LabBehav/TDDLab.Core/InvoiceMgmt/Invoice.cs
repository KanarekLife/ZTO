using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BasicUtils;

namespace TDDLab.Core.InvoiceMgmt
{
    public class Invoice : ValidatedDomainObject
    {
        public Invoice()
        {
            Lines = new List<InvoiceLine>();
        }

        public Invoice(string invoiceNumber, Recipient recipient, Address billToAddress, IEnumerable<InvoiceLine> lines, Money discount)
        {
            InvoiceNumber = invoiceNumber;
            Lines = new List<InvoiceLine>();
            Recipient = recipient;
            BillToAddress = billToAddress;
            Discount = discount;
            lines.Each(line => Lines.Add(line));
        }

        public Invoice(string invoiceNumber, Recipient recipient, Address billToAddress, IEnumerable<InvoiceLine> lines)
            : this(invoiceNumber, recipient, billToAddress, lines, null)
        {
        }

        public Money Discount { get; private set; }

        public Recipient Recipient { get; private set; }

        public IList<InvoiceLine> Lines { get; private set; }

        public string InvoiceNumber { get; private set; }

        public Address BillToAddress { get; private set; }

        public void AttachInvoiceLine(InvoiceLine line)
        {
            Lines.Add(line);
            line.Invoice = this;
        }

        public sealed class ValidationRules
        {
            public static IBusinessRule<Invoice> InvoiceNumber
            {
                get
                {
                    return new BusinessRule<Invoice>(MethodBase.GetCurrentMethod().Name, "Invoice number should be specified", invoice => invoice.InvoiceNumber.IsNotEmpty());
                }
            }
            public static IBusinessRule<Invoice> BillingAddress
            {
                get
                {
                    return new BusinessRule<Invoice>(MethodBase.GetCurrentMethod().Name, "Billing address should be valid", invoice => invoice.BillToAddress != null && invoice.BillToAddress.IsValid);
                }
            }
            public static IBusinessRule<Invoice> Recipient
            {
                get
                {
                    return new BusinessRule<Invoice>(MethodBase.GetCurrentMethod().Name, "Recipient should be valid", invoice => invoice.Recipient != null && invoice.Recipient.IsValid);
                }
            }
            public static IBusinessRule<Invoice> Discount
            {
                get
                {
                    return new BusinessRule<Invoice>(MethodBase.GetCurrentMethod().Name, "Discount should be valid", invoice => invoice.Discount == null || invoice.Discount.IsValid);
                }
            }
            public static IBusinessRule<Invoice> Lines
            {
                get
                {
                    return new BusinessRule<Invoice>(MethodBase.GetCurrentMethod().Name, "Invoice lines should all be valid", invoice => invoice.Lines.IsNotEmpty() && invoice.Lines.Where<InvoiceLine>(item=>false==item.IsValid).IsEmpty());
                }
            }
        }
        protected override IBusinessRuleSet Rules
        {
            get
            {
                return new BusinessRuleSet<Invoice>(
                    ValidationRules.InvoiceNumber,
                    ValidationRules.BillingAddress,
                    ValidationRules.Recipient,
                    ValidationRules.Discount,
                    ValidationRules.Lines);
            }
        }

    }
}