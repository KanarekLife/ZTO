using System.Collections.Generic;
using BasicUtils;

namespace TDDLab.Core.InvoiceMgmt
{
    public class InvoiceProcessorImpl : IInvoiceProcessor
    {
        private readonly IDictionary<string,Money> products = new Dictionary<string,Money>();

        public IDictionary<string, Money> Products
        {
            get { return products; }
        }

        public ProcessingResult Process(Invoice invoice)
        {
            if (false == invoice.IsValid)
                return ProcessingResult.Failed();

            invoice.Lines.Each(line =>
            {
                if (!products.ContainsKey(line.ProductName))
                    products.Add(line.ProductName, line.Money);
                else
                    products[line.ProductName] += (line.Money - invoice.Discount);
            });

            return ProcessingResult.Succeeded();
        }
    }
}