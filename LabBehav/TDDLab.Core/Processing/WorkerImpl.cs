using System;
using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Infrastructure
{
    public class WorkerImpl : IWorker
    {
        readonly IConfigurationSettings configurationSettings;
        readonly IMessagingFacility<Invoice, ProcessingResult> messagingFacitliy;
        readonly IExceptionHandler exceptionHandler;
        readonly IInvoiceProcessor _invoiceProcessor;


        public WorkerImpl(
            IConfigurationSettings configurationSettings,
            IMessagingFacility<Invoice, ProcessingResult> messagingFacitliy, 
            IExceptionHandler exceptionHandler,
            IInvoiceProcessor _invoiceProcessor)
        {
            this.configurationSettings = configurationSettings;
            this._invoiceProcessor = _invoiceProcessor;
            this.exceptionHandler = exceptionHandler;
            this.messagingFacitliy = messagingFacitliy;
        }


        public void Start()
        {
            messagingFacitliy.InitializeInputChannel(
                configurationSettings.GetSettingsByKey("inputQueue"));

            messagingFacitliy.InitializeOutputChannel(
                configurationSettings.GetSettingsByKey("outputQueue"));
        }
        public void Stop()
        {
            messagingFacitliy.Dispose();
        }

        public void DoJob()
        {
            try
            {
                var inputMessage = messagingFacitliy.ReadMessage();

                var processingResult = _invoiceProcessor.Process(inputMessage.Data);

                messagingFacitliy.WriteMessage(new Message<ProcessingResult>()
                                                   {
                                                       Data = processingResult,
                                                       Metadata = inputMessage.Metadata
                                                   });

            }
            catch (Exception ex)
            {
                exceptionHandler.HandleException(ex);
            }
        }
    }
}