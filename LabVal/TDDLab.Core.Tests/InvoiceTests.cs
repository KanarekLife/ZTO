using TDDLab.Core.InvoiceMgmt;
using BasicUtils;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class InvoiceTests
{
    private Address _validAddress;
    private Recipient _validRecipient;
    private InvoiceLine _validLine1;
    private InvoiceLine _validLine2;
    private Money _validDiscount;

    [SetUp]
    public void SetUp()
    {
        _validAddress = new Address("123 Main St", "New York", "NY", "10001");
        _validRecipient = new Recipient("John Doe", _validAddress);
        _validLine1 = new InvoiceLine("Widget A", new Money(100, "USD"));
        _validLine2 = new InvoiceLine("Widget B", new Money(200, "USD"));
        _validDiscount = new Money(10, "USD");
    }

    [Test]
    public void Invoice_DefaultConstructor_InitializesEmptyLines()
    {
        // Act
        var invoice = new Invoice();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.Lines, Is.Not.Null);
            Assert.That(invoice.Lines, Is.Empty);
            Assert.That(invoice.InvoiceNumber, Is.Null);
            Assert.That(invoice.Recipient, Is.Null);
            Assert.That(invoice.BillToAddress, Is.Null);
            Assert.That(invoice.Discount, Is.Null);
        }
    }

    [Test]
    public void Invoice_FullConstructor_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var invoiceNumber = "INV-001";
        var lines = new[] { _validLine1, _validLine2 };

        // Act
        var invoice = new Invoice(invoiceNumber, _validRecipient, _validAddress, lines, _validDiscount);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.InvoiceNumber, Is.EqualTo(invoiceNumber));
            Assert.That(invoice.Recipient, Is.EqualTo(_validRecipient));
            Assert.That(invoice.BillToAddress, Is.EqualTo(_validAddress));
            Assert.That(invoice.Discount, Is.EqualTo(_validDiscount));
            Assert.That(invoice.Lines, Has.Count.EqualTo(2));
            Assert.That(invoice.Lines, Contains.Item(_validLine1));
            Assert.That(invoice.Lines, Contains.Item(_validLine2));
        }
    }

    [Test]
    public void Invoice_ConstructorWithoutDiscount_SetsDiscountToNull()
    {
        // Arrange
        var invoiceNumber = "INV-001";
        var lines = new[] { _validLine1 };

        // Act
        var invoice = new Invoice(invoiceNumber, _validRecipient, _validAddress, lines);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.InvoiceNumber, Is.EqualTo(invoiceNumber));
            Assert.That(invoice.Recipient, Is.EqualTo(_validRecipient));
            Assert.That(invoice.BillToAddress, Is.EqualTo(_validAddress));
            Assert.That(invoice.Discount, Is.Null);
            Assert.That(invoice.Lines, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Invoice_AttachInvoiceLine_AddsLineToCollectionAndSetsInvoiceReference()
    {
        // Arrange
        var invoice = new Invoice();
        var line = new InvoiceLine("Widget", new Money(100, "USD"));

        // Act
        invoice.AttachInvoiceLine(line);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.Lines, Has.Count.EqualTo(1));
            Assert.That(invoice.Lines, Contains.Item(line));
            Assert.That(line.Invoice, Is.EqualTo(invoice));
        }
    }

    [Test]
    public void Invoice_IsValid_WithAllValidFields_ReturnsTrue()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.True);
    }

    [Theory]
    [TestCase("", TestName = "Empty InvoiceNumber")]
    [TestCase(null, TestName = "Null InvoiceNumber")]
    public void Invoice_IsValid_WithInvalidInvoiceNumber_ReturnsFalse(string? invoiceNumber)
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice(invoiceNumber, _validRecipient, _validAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithInvalidRecipient_ReturnsFalse()
    {
        // Arrange
        var invalidRecipient = new Recipient("", _validAddress);
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", invalidRecipient, _validAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithNullRecipient_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", null, _validAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithInvalidBillToAddress_ReturnsFalse()
    {
        // Arrange
        var invalidAddress = new Address("", "", "", "");
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, invalidAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithNullBillToAddress_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, null, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithInvalidDiscount_ReturnsFalse()
    {
        // Arrange
        var invalidDiscount = new Money(100, "");
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, invalidDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithNullDiscount_ReturnsTrue()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, null);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.True);
    }

    [Test]
    public void Invoice_IsValid_WithEmptyLines_ReturnsFalse()
    {
        // Arrange
        var lines = new InvoiceLine[] { };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_IsValid_WithInvalidLines_ReturnsFalse()
    {
        // Arrange
        var invalidLine = new InvoiceLine("", new Money(100, "USD"));
        var lines = new[] { invalidLine };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, _validDiscount);

        // Act & Assert
        Assert.That(invoice.IsValid, Is.False);
    }

    [Test]
    public void Invoice_Validate_WithValidInvoice_ReturnsNoErrors()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, _validDiscount);

        // Act
        var errors = invoice.Validate();

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void Invoice_Validate_WithInvalidInvoiceNumber_ReturnsInvoiceNumberError()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("", _validRecipient, _validAddress, lines, _validDiscount);

        // Act
        var errors = invoice.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors.Any(e => e.Name == "get_InvoiceNumber"), Is.True);
        }
    }

    [Test]
    public void Invoice_Validate_WithMultipleInvalidFields_ReturnsMultipleErrors()
    {
        // Arrange
        var invalidRecipient = new Recipient("", _validAddress);
        var invalidAddress = new Address("", "", "", "");
        var invalidDiscount = new Money(100, "");
        var lines = new InvoiceLine[] { };
        var invoice = new Invoice("", invalidRecipient, invalidAddress, lines, invalidDiscount);

        // Act
        var errors = invoice.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(5));
            Assert.That(errors.Any(e => e.Name == "get_InvoiceNumber"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_BillingAddress"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Recipient"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Discount"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Lines"), Is.True);
        }
    }

    [Theory]
    [TestCase("InvoiceNumber", "get_InvoiceNumber", "Invoice number should be specified")]
    [TestCase("BillingAddress", "get_BillingAddress", "Billing address should be valid")]
    [TestCase("Recipient", "get_Recipient", "Recipient should be valid")]
    [TestCase("Discount", "get_Discount", "Discount should be valid")]
    [TestCase("Lines", "get_Lines", "Invoice lines should all be valid")]
    public void Invoice_ValidationRules_HasCorrectNameAndDescription(string ruleName, string expectedName, string expectedDescription)
    {
        // Act
        var rule = ruleName switch
        {
            "InvoiceNumber" => Invoice.ValidationRules.InvoiceNumber,
            "BillingAddress" => Invoice.ValidationRules.BillingAddress,
            "Recipient" => Invoice.ValidationRules.Recipient,
            "Discount" => Invoice.ValidationRules.Discount,
            "Lines" => Invoice.ValidationRules.Lines,
            _ => throw new ArgumentException($"Unknown rule name: {ruleName}")
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rule.Name, Is.EqualTo(expectedName));
            Assert.That(rule.Description, Is.EqualTo(expectedDescription));
        }
    }

    [Test]
    public void Invoice_ValidationRules_Contains_ReturnsCorrectResult()
    {
        // Arrange
        var lines = new[] { _validLine1 };
        var invoice = new Invoice("INV-001", _validRecipient, _validAddress, lines, _validDiscount);
        var errors = invoice.Validate().ToArray();

        // Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.InvoiceNumber));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.BillingAddress));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.Recipient));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.Discount));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.Lines));
        }
    }

    [Test]
    public void Invoice_ValidationRules_Contains_WithInvalidInvoice_ReturnsCorrectResult()
    {
        // Arrange
        var invoice = new Invoice("", null, null, [], new Money(100, ""));
        var errors = invoice.Validate().ToArray();

        // Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Does.Contain(Invoice.ValidationRules.InvoiceNumber));
            Assert.That(errors, Does.Contain(Invoice.ValidationRules.BillingAddress));
            Assert.That(errors, Does.Contain(Invoice.ValidationRules.Recipient));
            Assert.That(errors, Does.Contain(Invoice.ValidationRules.Discount));
            Assert.That(errors, Does.Contain(Invoice.ValidationRules.Lines));
        }
    }
}