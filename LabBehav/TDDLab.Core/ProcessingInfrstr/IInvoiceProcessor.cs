namespace TDDLab.Core.InvoiceMgmt
{
    public interface IInvoiceProcessor
    {
        ProcessingResult Process(Invoice invoice);
    }
}