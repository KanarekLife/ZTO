using TDDLab.Core.InvoiceMgmt;
using BasicUtils;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class InvoiceLineTests
{
    private Money _validMoney;

    [SetUp]
    public void SetUp()
    {
        _validMoney = new Money(100, "USD");
    }

    [Test]
    public void InvoiceLine_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        const string productName = "Widget";

        // Act
        var line = new InvoiceLine(productName, _validMoney);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(line.ProductName, Is.EqualTo(productName));
            Assert.That(line.Money, Is.EqualTo(_validMoney));
            Assert.That(line.Invoice, Is.Null);
        }
    }

    [Test]
    public void InvoiceLine_Invoice_CanBeSetAndRetrieved()
    {
        // Arrange
        var line = new InvoiceLine("Widget", _validMoney);
        var invoice = new Invoice();

        // Act
        line.Invoice = invoice;

        // Assert
        Assert.That(line.Invoice, Is.EqualTo(invoice));
    }

    [Test]
    public void InvoiceLine_IsValid_WithValidProductNameAndMoney_ReturnsTrue()
    {
        // Arrange
        var line = new InvoiceLine("Widget", _validMoney);

        // Act & Assert
        Assert.That(line.IsValid, Is.True);
    }

    [Theory]
    [TestCase("", TestName = "Empty ProductName")]
    [TestCase(null, TestName = "Null ProductName")]
    public void InvoiceLine_IsValid_WithInvalidProductName_ReturnsFalse(string? productName)
    {
        // Arrange
        var line = new InvoiceLine(productName, _validMoney);

        // Act & Assert
        Assert.That(line.IsValid, Is.False);
    }

    [Test]
    public void InvoiceLine_IsValid_WithInvalidMoney_ReturnsFalse()
    {
        // Arrange
        var invalidMoney = new Money(100, "");
        var line = new InvoiceLine("Widget", invalidMoney);

        // Act & Assert
        Assert.That(line.IsValid, Is.False);
    }

    [Test]
    public void InvoiceLine_IsValid_WithNullMoney_ReturnsFalse()
    {
        // Arrange
        var line = new InvoiceLine("Widget", null);

        // Act & Assert
        Assert.That(line.IsValid, Is.False);
    }

    [Test]
    public void InvoiceLine_Validate_WithValidLine_ReturnsNoErrors()
    {
        // Arrange
        var line = new InvoiceLine("Widget", _validMoney);

        // Act
        var errors = line.Validate();

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Theory]
    [TestCase("", "get_ProductName", TestName = "Invalid ProductName")]
    [TestCase("Widget", "get_Money", true, TestName = "Invalid Money")]
    [TestCase("Widget", "get_Money", false, TestName = "Null Money")]
    public void InvoiceLine_Validate_WithInvalidField_ReturnsExpectedError(string? productName, string expectedErrorName, bool useInvalidMoney = false)
    {
        // Arrange
        var money = useInvalidMoney ? new Money(100, "") : productName == "Widget" && expectedErrorName == "get_Money" ? null : _validMoney;
        var line = new InvoiceLine(productName, money);

        // Act
        var errors = line.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors.Any(e => e.Name == expectedErrorName), Is.True);
        }
    }

    [Test]
    public void InvoiceLine_Validate_WithMultipleInvalidFields_ReturnsMultipleErrors()
    {
        // Arrange
        var line = new InvoiceLine("", null);

        // Act
        var errors = line.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(2));
            Assert.That(errors.Any(e => e.Name == "get_ProductName"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Money"), Is.True);
        }
    }

    [Test]
    public void InvoiceLine_ToString_ReturnsFormattedString()
    {
        // Arrange
        var line = new InvoiceLine("Widget", _validMoney);

        // Act
        var result = line.ToString();

        // Assert
        Assert.That(result, Is.EqualTo("Widget for 100USD"));
    }

    [Theory]
    [TestCase("ProductName", "get_ProductName", "Product name should be specified")]
    [TestCase("Money", "get_Money", "Money should be valid")]
    public void InvoiceLine_ValidationRules_HasCorrectNameAndDescription(string ruleName, string expectedName, string expectedDescription)
    {
        // Act
        var rule = ruleName switch
        {
            "ProductName" => InvoiceLine.ValidationRules.ProductName,
            "Money" => InvoiceLine.ValidationRules.Money,
            _ => throw new ArgumentException($"Unknown rule name: {ruleName}")
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rule.Name, Is.EqualTo(expectedName));
            Assert.That(rule.Description, Is.EqualTo(expectedDescription));
        }
    }

    [Theory]
    [TestCase("ProductName", "Widget", true, TestName = "ProductName rule with valid name")]
    [TestCase("ProductName", "", false, TestName = "ProductName rule with empty name")]
    [TestCase("Money", "Widget", true, TestName = "Money rule with valid money")]
    [TestCase("Money", "Widget", false, TestName = "Money rule with invalid money")]
    public void InvoiceLine_ValidationRules_IsSatisfiedBy_ReturnsExpectedResult(string ruleName, string productName, bool useValidMoney)
    {
        // Arrange
        var money = useValidMoney ? _validMoney : new Money(100, "");
        var line = new InvoiceLine(productName, money);
        var rule = ruleName switch
        {
            "ProductName" => InvoiceLine.ValidationRules.ProductName,
            "Money" => InvoiceLine.ValidationRules.Money,
            _ => throw new ArgumentException($"Unknown rule name: {ruleName}")
        };

        // Act
        var isSatisfied = rule.IsSatisfiedBy(line);

        // Assert
        Assert.That(isSatisfied, Is.EqualTo(useValidMoney && !string.IsNullOrEmpty(productName)));
    }
}