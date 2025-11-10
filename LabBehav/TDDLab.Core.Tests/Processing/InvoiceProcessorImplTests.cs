using TDDLab.Core.InvoiceMgmt;
using TDDLab.Core.Tests.Builders;

namespace TDDLab.Core.Tests.Processing
{
    public class InvoiceProcessorImplTests
    {
        [Fact]
        public void Process_Should_ReturnFailed_When_InvoiceIsInvalid()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var invalidInvoice = new InvoiceBuilder().BuildInvalidEmptyLines();

            // Act
            var result = sut.Process(invalidInvoice);

            // Assert
            Assert.Equal(ProcessingResult.Failed(), result);
        }

        [Fact]
        public void Process_Should_Succeed_When_InvoiceIsValid()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var invoice = InvoiceBuilder.Valid().Build();

            // Act
            var result = sut.Process(invoice);

            // Assert
            Assert.Equal(ProcessingResult.Succeeded(), result);
        }

        [Fact]
        public void Process_Should_AddNewProduct_When_ProductNotSeenBefore()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var line = InvoiceLineBuilder.Valid().WithProduct("Widget").WithMoney(new Money(50)).Build();
            var invoice = InvoiceBuilder.Valid().WithLines(new[] { line }).Build();

            // Act
            var res = sut.Process(invoice);

            // Assert
            Assert.Equal(ProcessingResult.Succeeded(), res);
            Assert.True(sut.Products.ContainsKey("Widget"));
            Assert.Equal(new Money(50), sut.Products["Widget"]);
        }

        [Fact]
        public void Process_Should_AccumulateAmountMinusDiscount_When_ProductAlreadyExists()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var discount = new Money(10);
            var line1 = new InvoiceLine("Widget", new Money(50));
            var line2 = new InvoiceLine("Widget", new Money(40));
            var invoice = InvoiceBuilder.Valid()
                .WithLines(new[] { line1, line2 })
                .WithDiscount(discount)
                .Build();

            // Act
            var res = sut.Process(invoice);

            // Assert
            Assert.Equal(ProcessingResult.Succeeded(), res);
            // first time: add 50; second time: + (40 - 10) = +30 => 80
            Assert.Equal(new Money(80), sut.Products["Widget"]);
        }

        [Fact]
        public void Process_Should_Throw_When_DiscountIsNull()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var line1 = new InvoiceLine("Widget", new Money(50));
            var line2 = new InvoiceLine("Widget", new Money(40));
            var invoice = InvoiceBuilder.Valid()
                .WithLines([line1, line2])
                .WithDiscount(null!)
                .Build();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => sut.Process(invoice));
        }

        [Fact]
        public void Process_Should_TrackProductsSeparately_When_MultipleDifferentProducts()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var line1 = new InvoiceLine("Widget", new Money(50));
            var line2 = new InvoiceLine("Gadget", new Money(40));
            var invoice = InvoiceBuilder.Valid().WithLines([line1, line2]).Build();

            // Act
            var res = sut.Process(invoice);

            // Assert
            Assert.Equal(ProcessingResult.Succeeded(), res);
            Assert.Equal(new Money(50), sut.Products["Widget"]);
            Assert.Equal(new Money(40), sut.Products["Gadget"]);
        }

        [Fact]
        public void Process_Should_NotReduceBelowZero_When_DiscountExceedsLineAmount()
        {
            // Arrange
            var sut = new InvoiceProcessorImpl();
            var discount = new Money(1000);
            var line1 = new InvoiceLine("Widget", new Money(50));
            var line2 = new InvoiceLine("Widget", new Money(40));
            var invoice = InvoiceBuilder.Valid()
                .WithLines([line1, line2])
                .WithDiscount(discount)
                .Build();

            // Act
            var res = sut.Process(invoice);

            // Assert
            Assert.Equal(ProcessingResult.Succeeded(), res);
            Assert.Equal(new Money(50), sut.Products["Widget"]);
        }
    }
}
