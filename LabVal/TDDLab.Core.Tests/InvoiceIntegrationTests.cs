using TDDLab.Core.InvoiceMgmt;
using BasicUtils;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class InvoiceIntegrationTests
{
    [Test]
    public void CreateCompleteInvoice_WithValidData_ShouldBeValid()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "NY", "10001");
        var recipient = new Recipient("John Doe", address);
        var billToAddress = new Address("456 Oak Ave", "Los Angeles", "CA", "90210");
        
        var line1 = new InvoiceLine("Widget A", new Money(100, "USD"));
        var line2 = new InvoiceLine("Widget B", new Money(200, "USD"));
        var lines = new[] { line1, line2 };
        
        var discount = new Money(10, "USD");

        // Act
        var invoice = new Invoice("INV-001", recipient, billToAddress, lines, discount);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.IsValid, Is.True);
            Assert.That(invoice.Validate(), Is.Empty);
            Assert.That(invoice.InvoiceNumber, Is.EqualTo("INV-001"));
            Assert.That(invoice.Recipient, Is.EqualTo(recipient));
            Assert.That(invoice.BillToAddress, Is.EqualTo(billToAddress));
            Assert.That(invoice.Lines, Has.Count.EqualTo(2));
            Assert.That(invoice.Discount, Is.EqualTo(discount));
        }
    }

    [Test]
    public void CreateInvoice_AttachLines_ShouldSetInvoiceReference()
    {
        // Arrange
        var invoice = new Invoice();
        var line1 = new InvoiceLine("Widget A", new Money(100, "USD"));
        var line2 = new InvoiceLine("Widget B", new Money(200, "USD"));

        // Act
        invoice.AttachInvoiceLine(line1);
        invoice.AttachInvoiceLine(line2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.Lines, Has.Count.EqualTo(2));
            Assert.That(line1.Invoice, Is.EqualTo(invoice));
            Assert.That(line2.Invoice, Is.EqualTo(invoice));
        }
    }

    [Test]
    public void MoneyOperations_WithDifferentCurrencies_ShouldWorkCorrectly()
    {
        // Arrange
        var usdMoney = new Money(100, "USD");
        var eurMoney = new Money(50, "EUR");

        // Act
        var convertedEurMoney = eurMoney.ToCurrency("USD");
        var sum = usdMoney + convertedEurMoney;
        var difference = usdMoney - convertedEurMoney;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(convertedEurMoney.Currency, Is.EqualTo("USD"));
            Assert.That(sum.Currency, Is.EqualTo("USD"));
            Assert.That(sum.Amount, Is.EqualTo(150UL));
            Assert.That(difference.Currency, Is.EqualTo("USD"));
            Assert.That(difference.Amount, Is.EqualTo(50UL));
        }
    }

    [Test]
    public void Invoice_WithInvalidComponents_ShouldFailValidation()
    {
        // Arrange
        var invalidAddress = new Address("", "", "", "");
        var invalidRecipient = new Recipient("", invalidAddress);
        var invalidLine = new InvoiceLine("", new Money(100, ""));
        var invalidDiscount = new Money(10, "");

        // Act
        var invoice = new Invoice("", invalidRecipient, invalidAddress, [invalidLine], invalidDiscount);

        // Assert
        Assert.That(invoice.IsValid, Is.False);
        
        var errors = invoice.Validate().ToList();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.Name == "get_InvoiceNumber"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Recipient"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_BillingAddress"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Lines"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Discount"), Is.True);
        }
    }

    [Test]
    public void ValidationRules_ShouldHaveConsistentEqualityBehavior()
    {
        // Arrange
        var rule1 = Invoice.ValidationRules.InvoiceNumber;
        var rule2 = Invoice.ValidationRules.InvoiceNumber;

        // Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rule1.Equals(rule2), Is.True);
            Assert.That(rule1.GetHashCode(), Is.EqualTo(rule2.GetHashCode()));
            Assert.That(rule1.Name, Is.EqualTo(rule2.Name));
        }
    }

    [Test]
    public void BusinessRuleSet_ShouldContainCorrectRules()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "NY", "10001");
        var recipient = new Recipient("John Doe", address);
        var billToAddress = new Address("456 Oak Ave", "Los Angeles", "CA", "90210");
        var line = new InvoiceLine("Widget", new Money(100, "USD"));
        var discount = new Money(10, "USD");
        var invoice = new Invoice("INV-001", recipient, billToAddress, [line], discount);

        // Act
        var errors = invoice.Validate().ToArray();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.InvoiceNumber));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.Recipient));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.BillingAddress));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.Lines));
            Assert.That(errors, Does.Not.Contain(Invoice.ValidationRules.Discount));
        }
    }

    [Test]
    public void ComplexValidationScenario_MixedValidAndInvalidObjects()
    {
        // Arrange
        var validAddress = new Address("123 Main St", "New York", "NY", "10001");
        var invalidAddress = new Address("", "", "", "");
        var validRecipient = new Recipient("John Doe", validAddress);
        var validLine = new InvoiceLine("Valid Widget", new Money(100, "USD"));
        var invalidLine = new InvoiceLine("", new Money(50, ""));
        var validDiscount = new Money(10, "USD");

        // Act
        var invoice = new Invoice("INV-001", validRecipient, invalidAddress, [validLine, invalidLine], validDiscount);

        // Assert
        Assert.That(invoice.IsValid, Is.False);
        
        var errors = invoice.Validate().ToList();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors.Any(e => e.Name == "get_BillingAddress"), Is.True, "Should fail due to invalid billing address");
            Assert.That(errors.Any(e => e.Name == "get_Lines"), Is.True, "Should fail due to invalid line");
            Assert.That(errors.Any(e => e.Name == "get_InvoiceNumber"), Is.False, "Should pass invoice number validation");
            Assert.That(errors.Any(e => e.Name == "get_Recipient"), Is.False, "Should pass recipient validation");
            Assert.That(errors.Any(e => e.Name == "get_Discount"), Is.False, "Should pass discount validation");
        }
    }

    [Test]
    public void MoneyEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");
        var money3 = new Money(200, "USD");
        var money4 = new Money(100, "EUR");

        // Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(money1, Is.EqualTo(money2));
            Assert.That(money1, Is.EqualTo(money2));
            Assert.That(money1, Is.Not.EqualTo(money3));
            Assert.That(money1, Is.Not.EqualTo(money4));
            Assert.That(money1.GetHashCode(), Is.EqualTo(money2.GetHashCode()));
        }
    }

    [Test]
    public void ToStringMethods_ShouldReturnFormattedOutput()
    {
        // Arrange
        var money = new Money(100, "USD");
        var line = new InvoiceLine("Test Widget", money);

        // Act
        var moneyString = money.ToString();
        var lineString = line.ToString();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(moneyString, Is.EqualTo("100USD"));
            Assert.That(lineString, Is.EqualTo("Test Widget for 100USD"));
        }
    }
}