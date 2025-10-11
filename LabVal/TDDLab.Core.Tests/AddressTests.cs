using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class AddressTests
{
    [Test]
    public void Address_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        const string addressLine1 = "123 Main St";
        const string city = "New York";
        const string state = "NY";
        const string zip = "10001";

        // Act
        var address = new Address(addressLine1, city, state, zip);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(address.AddressLine1, Is.EqualTo(addressLine1));
            Assert.That(address.City, Is.EqualTo(city));
            Assert.That(address.State, Is.EqualTo(state));
            Assert.That(address.Zip, Is.EqualTo(zip));
        }
    }

    [Test]
    public void Address_IsValid_WithAllValidFields_ReturnsTrue()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "NY", "10001");

        // Act & Assert
        Assert.That(address.IsValid, Is.True);
    }

    [Theory]
    [TestCase("", "New York", "NY", "10001", TestName = "Empty AddressLine1")]
    [TestCase(null, "New York", "NY", "10001", TestName = "Null AddressLine1")]
    [TestCase("123 Main St", "", "NY", "10001", TestName = "Empty City")]
    [TestCase("123 Main St", null, "NY", "10001", TestName = "Null City")]
    [TestCase("123 Main St", "New York", "", "10001", TestName = "Empty State")]
    [TestCase("123 Main St", "New York", null, "10001", TestName = "Null State")]
    [TestCase("123 Main St", "New York", "NY", "", TestName = "Empty Zip")]
    [TestCase("123 Main St", "New York", "NY", null, TestName = "Null Zip")]
    public void Address_IsValid_WithInvalidFields_ReturnsFalse(string? addressLine1, string? city, string? state, string? zip)
    {
        // Arrange
        var address = new Address(addressLine1, city, state, zip);

        // Act & Assert
        Assert.That(address.IsValid, Is.False);
    }

    [Test]
    public void Address_Validate_WithValidAddress_ReturnsNoErrors()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "NY", "10001");

        // Act
        var errors = address.Validate();

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Theory]
    [TestCase("", "New York", "NY", "10001", "get_AddressLine1", TestName = "Invalid AddressLine1")]
    [TestCase("123 Main St", "", "NY", "10001", "get_City", TestName = "Invalid City")]
    [TestCase("123 Main St", "New York", "", "10001", "get_State", TestName = "Invalid State")]
    [TestCase("123 Main St", "New York", "NY", "", "get_Zip", TestName = "Invalid Zip")]
    public void Address_Validate_WithInvalidField_ReturnsExpectedError(string? addressLine1, string? city, string? state, string? zip, string expectedErrorName)
    {
        // Arrange
        var address = new Address(addressLine1, city, state, zip);

        // Act
        var errors = address.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors.Any(e => e.Name == expectedErrorName), Is.True);
        }
    }

    [Test]
    public void Address_Validate_WithMultipleInvalidFields_ReturnsMultipleErrors()
    {
        // Arrange
        var address = new Address("", "", "", "");

        // Act
        var errors = address.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(4));
            Assert.That(errors.Any(e => e.Name == "get_AddressLine1"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_City"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_State"), Is.True);
            Assert.That(errors.Any(e => e.Name == "get_Zip"), Is.True);
        }
    }

    [Theory]
    [TestCase("AddressLine1", "get_AddressLine1", "AddressLine1 should be specified")]
    [TestCase("City", "get_City", "City should be specified")]
    [TestCase("State", "get_State", "State should be properly specified")]
    [TestCase("Zip", "get_Zip", "Zip code should be specified")]
    public void Address_ValidationRules_HasCorrectNameAndDescription(string ruleName, string expectedName, string expectedDescription)
    {
        // Act
        var rule = ruleName switch
        {
            "AddressLine1" => Address.ValidationRules.AddressLine1,
            "City" => Address.ValidationRules.City,
            "State" => Address.ValidationRules.State,
            "Zip" => Address.ValidationRules.Zip,
            _ => throw new ArgumentException($"Unknown rule name: {ruleName}")
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rule.Name, Is.EqualTo(expectedName));
            Assert.That(rule.Description, Is.EqualTo(expectedDescription));
        }
    }
}