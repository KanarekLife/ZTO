using TDDLab.Core.InvoiceMgmt;
using BasicUtils;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class RecipientTests
{
    private Address _validAddress;

    [SetUp]
    public void SetUp()
    {
        _validAddress = new Address("123 Main St", "New York", "NY", "10001");
    }

    [Test]
    public void Recipient_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        const string name = "John Doe";

        // Act
        var recipient = new Recipient(name, _validAddress);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(recipient.Name, Is.EqualTo(name));
            Assert.That(recipient.Address, Is.EqualTo(_validAddress));
        }
    }

    [Test]
    public void Recipient_IsValid_WithValidNameAndAddress_ReturnsTrue()
    {
        // Arrange
        var recipient = new Recipient("John Doe", _validAddress);

        // Act & Assert
        Assert.That(recipient.IsValid, Is.True);
    }

    [Theory]
    [TestCase("", TestName = "Empty Name")]
    [TestCase(null, TestName = "Null Name")]
    public void Recipient_IsValid_WithInvalidName_ReturnsFalse(string? name)
    {
        // Arrange
        var recipient = new Recipient(name, _validAddress);

        // Act & Assert
        Assert.That(recipient.IsValid, Is.False);
    }

    [Test]
    public void Recipient_IsValid_WithInvalidAddress_ReturnsFalse()
    {
        // Arrange
        var invalidAddress = new Address("", "", "", "");
        var recipient = new Recipient("John Doe", invalidAddress);

        // Act & Assert
        Assert.That(recipient.IsValid, Is.False);
    }

    [Test]
    public void Recipient_IsValid_WithNullAddress_ReturnsFalse()
    {
        // Arrange
        var recipient = new Recipient("John Doe", null);

        // Act & Assert
        Assert.That(recipient.IsValid, Is.False);
    }

    [Test]
    public void Recipient_Validate_WithValidRecipient_ReturnsNoErrors()
    {
        // Arrange
        var recipient = new Recipient("John Doe", _validAddress);

        // Act
        var errors = recipient.Validate();

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Theory]
    [TestCase("", "get_Name", TestName = "Invalid Name")]
    [TestCase("John Doe", "get_Address", true, TestName = "Invalid Address")]
    [TestCase("John Doe", "get_Address", false, TestName = "Null Address")]
    public void Recipient_Validate_WithInvalidField_ReturnsExpectedError(string? name, string expectedErrorName, bool useInvalidAddress = false)
    {
        // Arrange
        Address? address = useInvalidAddress ? new Address("", "", "", "") : name == "John Doe" && expectedErrorName == "get_Address" ? null : _validAddress;
        var recipient = new Recipient(name, address);

        // Act
        var errors = recipient.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors.Any(e => e.Name == expectedErrorName), Is.True);
        }
    }

    [Test]
    public void Recipient_Validate_WithMultipleInvalidFields_ReturnsMultipleErrors()
    {
        // Arrange
        var invalidAddress = new Address("", "", "", "");
        var recipient = new Recipient("", invalidAddress);

        // Act
        var errors = recipient.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(2));
            Assert.That(errors.Any(e => e.Name == "get_Name"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Address"), Is.True);
        }
    }

    [Theory]
    [TestCase("Name", "get_Name", "Recipient name should be specified")]
    [TestCase("Address", "get_Address", "Address should be valid")]
    public void Recipient_ValidationRules_HasCorrectNameAndDescription(string ruleName, string expectedName, string expectedDescription)
    {
        // Act
        var rule = ruleName switch
        {
            "Name" => Recipient.ValidationRules.Name,
            "Address" => Recipient.ValidationRules.Address,
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
    [TestCase("Name", "John Doe", true, TestName = "Name rule with valid name")]
    [TestCase("Name", "", false, TestName = "Name rule with empty name")]
    [TestCase("Address", "John Doe", true, TestName = "Address rule with valid address")]
    [TestCase("Address", "John Doe", false, TestName = "Address rule with invalid address")]
    public void Recipient_ValidationRules_IsSatisfiedBy_ReturnsExpectedResult(string ruleName, string name, bool useValidAddress)
    {
        // Arrange
        var address = useValidAddress ? _validAddress : new Address("", "", "", "");
        var recipient = new Recipient(name, address);
        var rule = ruleName switch
        {
            "Name" => Recipient.ValidationRules.Name,
            "Address" => Recipient.ValidationRules.Address,
            _ => throw new ArgumentException($"Unknown rule name: {ruleName}")
        };

        // Act
        var isSatisfied = rule.IsSatisfiedBy(recipient);

        // Assert
        Assert.That(isSatisfied, Is.EqualTo(useValidAddress && !string.IsNullOrEmpty(name)));
    }
}