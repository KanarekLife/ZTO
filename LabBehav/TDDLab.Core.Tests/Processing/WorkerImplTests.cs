using Moq;
using TDDLab.Core.Infrastructure;
using TDDLab.Core.InvoiceMgmt;
using TDDLab.Core.Tests.Builders;

namespace TDDLab.Core.Tests.Processing
{
    public class WorkerImplTests
    {
        [Fact]
        public void Start_Should_InitializeChannels_When_Called()
        {
            // Arrange
            var config = new Mock<IConfigurationSettings>();
            var messaging = new Mock<IMessagingFacility<Invoice, ProcessingResult>>();
            var exceptionHandler = new Mock<IExceptionHandler>();
            var processor = new Mock<IInvoiceProcessor>();

            config.Setup(c => c.GetSettingsByKey("inputQueue")).Returns("in-q");
            config.Setup(c => c.GetSettingsByKey("outputQueue")).Returns("out-q");

            var sut = new WorkerImpl(config.Object, messaging.Object, exceptionHandler.Object, processor.Object);

            // Act
            sut.Start();

            // Assert
            messaging.Verify(m => m.InitializeInputChannel("in-q"), Times.Once);
            messaging.Verify(m => m.InitializeOutputChannel("out-q"), Times.Once);
        }

        [Fact]
        public void Stop_Should_DisposeMessagingFacility_When_Called()
        {
            // Arrange
            var config = new Mock<IConfigurationSettings>();
            var messaging = new Mock<IMessagingFacility<Invoice, ProcessingResult>>();
            var exceptionHandler = new Mock<IExceptionHandler>();
            var processor = new Mock<IInvoiceProcessor>();

            var sut = new WorkerImpl(config.Object, messaging.Object, exceptionHandler.Object, processor.Object);

            // Act
            sut.Stop();

            // Assert
            messaging.Verify(m => m.Dispose(), Times.Once);
        }

        [Fact]
        public void DoJob_Should_ReadProcessAndWriteMessage_When_ProcessingSucceeds()
        {
            // Arrange
            var config = new Mock<IConfigurationSettings>();
            var messaging = new Mock<IMessagingFacility<Invoice, ProcessingResult>>();
            var exceptionHandler = new Mock<IExceptionHandler>();
            var processor = new Mock<IInvoiceProcessor>();

            var invoice = InvoiceBuilder.Valid().Build();
            var metadata = new Metadata().FromString("meta");
            var inputMsg = new Message<Invoice> { Data = invoice, Metadata = metadata };

            messaging.Setup(m => m.ReadMessage()).Returns(inputMsg);

            var processingResult = ProcessingResult.Succeeded();
            processor.Setup(p => p.Process(invoice)).Returns(processingResult);

            var sut = new WorkerImpl(config.Object, messaging.Object, exceptionHandler.Object, processor.Object);

            // Act
            sut.DoJob();

            // Assert
            messaging.Verify(m => m.ReadMessage(), Times.Once);
            processor.Verify(p => p.Process(invoice), Times.Once);
            messaging.Verify(m => m.WriteMessage(It.Is<Message<ProcessingResult>>(msg => ReferenceEquals(msg.Metadata, metadata) && Equals(msg.Data, processingResult))), Times.Once);
        }
        
        [Fact]
        public void DoJob_Should_ReadProcessAndWriteMessage_When_ProcessingFails()
        {
            // Arrange
            var config = new Mock<IConfigurationSettings>();
            var messaging = new Mock<IMessagingFacility<Invoice, ProcessingResult>>();
            var exceptionHandler = new Mock<IExceptionHandler>();
            var processor = new Mock<IInvoiceProcessor>();

            var invoice = InvoiceBuilder.Valid().Build();
            var metadata = new Metadata().FromString("meta");
            var inputMsg = new Message<Invoice> { Data = invoice, Metadata = metadata };

            messaging.Setup(m => m.ReadMessage()).Returns(inputMsg);

            var processingResult = ProcessingResult.Failed();
            processor.Setup(p => p.Process(invoice)).Returns(processingResult);

            var sut = new WorkerImpl(config.Object, messaging.Object, exceptionHandler.Object, processor.Object);

            // Act
            sut.DoJob();

            // Assert
            messaging.Verify(m => m.ReadMessage(), Times.Once);
            processor.Verify(p => p.Process(invoice), Times.Once);
            messaging.Verify(m => m.WriteMessage(It.Is<Message<ProcessingResult>>(msg => ReferenceEquals(msg.Metadata, metadata) && Equals(msg.Data, processingResult))), Times.Once);
        }

        [Fact]
        public void DoJob_Should_UseExceptionHandler_When_ReadMessageThrows()
        {
            // Arrange
            var config = new Mock<IConfigurationSettings>();
            var messaging = new Mock<IMessagingFacility<Invoice, ProcessingResult>>();
            var exceptionHandler = new Mock<IExceptionHandler>();
            var processor = new Mock<IInvoiceProcessor>();

            var ex = new InvalidOperationException("boom");
            messaging.Setup(m => m.ReadMessage()).Throws(ex);

            var sut = new WorkerImpl(config.Object, messaging.Object, exceptionHandler.Object, processor.Object);

            // Act
            sut.DoJob();

            // Assert
            exceptionHandler.Verify(h => h.HandleException(ex), Times.Once);
            // Ensure no write happened
            messaging.Verify(m => m.WriteMessage(It.IsAny<Message<ProcessingResult>>()), Times.Never);
        }

        [Fact]
        public void DoJob_Should_UseExceptionHandler_When_ProcessorThrows()
        {
            // Arrange
            var config = new Mock<IConfigurationSettings>();
            var messaging = new Mock<IMessagingFacility<Invoice, ProcessingResult>>();
            var exceptionHandler = new Mock<IExceptionHandler>();
            var processor = new Mock<IInvoiceProcessor>();

            var invoice = InvoiceBuilder.Valid().Build();
            var metadata = new Metadata().FromString("meta");
            var inputMsg = new Message<Invoice> { Data = invoice, Metadata = metadata };

            messaging.Setup(m => m.ReadMessage()).Returns(inputMsg);

            var ex = new ApplicationException("fail processing");
            processor.Setup(p => p.Process(invoice)).Throws(ex);

            var sut = new WorkerImpl(config.Object, messaging.Object, exceptionHandler.Object, processor.Object);

            // Act
            sut.DoJob();

            // Assert
            exceptionHandler.Verify(h => h.HandleException(ex), Times.Once);
            messaging.Verify(m => m.WriteMessage(It.IsAny<Message<ProcessingResult>>()), Times.Never);
        }
    }
}
