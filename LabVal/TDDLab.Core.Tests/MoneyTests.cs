using TDDLab.Core.InvoiceMgmt;
using BasicUtils;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class MoneyTests
{
    [Test]
    public void Money_Constructor_WithAmountAndCurrency_SetsPropertiesCorrectly()
    {
        // Arrange
        const ulong amount = 100;
        const string currency = "EUR";

        // Act
        var money = new Money(amount, currency);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(money.Amount, Is.EqualTo(amount));
            Assert.That(money.Currency, Is.EqualTo(currency));
        }
    }

    [Test]
    public void Money_Constructor_WithAmountOnly_UsesDefaultCurrency()
    {
        // Arrange
        const ulong amount = 100;

        // Act
        var money = new Money(amount);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(money.Amount, Is.EqualTo(amount));
            Assert.That(money.Currency, Is.EqualTo(Money.DefaultCurrency));
        }
    }

    [Test]
    public void Money_ZERO_ReturnsZeroAmount()
    {
        // Act
        var zero = Money.ZERO;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(zero.Amount, Is.EqualTo(0UL));
            Assert.That(zero.Currency, Is.EqualTo(Money.DefaultCurrency));
        }
    }

    [Test]
    public void Money_Addition_SameCurrency_ReturnsCorrectSum()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "USD");

        // Act
        var result = money1 + money2;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Amount, Is.EqualTo(150UL));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }
    }

    [Test]
    public void Money_Subtraction_ValidSubtraction_ReturnsCorrectDifference()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "USD");

        // Act
        var result = money1 - money2;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Amount, Is.EqualTo(50UL));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }
    }

    [Test]
    public void Money_Subtraction_NegativeResult_ReturnsZero()
    {
        // Arrange
        var money1 = new Money(50, "USD");
        var money2 = new Money(100, "USD");

        // Act
        var result = money1 - money2;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Amount, Is.EqualTo(0UL));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }
    }

    [Test]
    public void Money_ToString_ReturnsFormattedString()
    {
        // Arrange
        var money = new Money(100, "EUR");

        // Act
        var result = money.ToString();

        // Assert
        Assert.That(result, Is.EqualTo("100EUR"));
    }

    [Test]
    public void Money_Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");

        // Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(money1.Equals(money2), Is.True);
            Assert.That(money1 == money2, Is.True);
            Assert.That(money1 != money2, Is.False);
        }
    }

    [Test]
    public void Money_Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "USD");

        // Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(money1.Equals(money2), Is.False);
            Assert.That(money1 == money2, Is.False);
            Assert.That(money1 != money2, Is.True);
        }
    }

    [Test]
    public void Money_GetHashCode_SameValues_ReturnsSameHashCode()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");

        // Act & Assert
        Assert.That(money1.GetHashCode(), Is.EqualTo(money2.GetHashCode()));
    }

    [Test]
    public void Money_IsValid_WithValidCurrency_ReturnsTrue()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act & Assert
        Assert.That(money.IsValid, Is.True);
    }

    [Theory]
    [TestCase("", TestName = "Empty Currency")]
    [TestCase(null, TestName = "Null Currency")]
    public void Money_IsValid_WithInvalidCurrency_ReturnsFalse(string? currency)
    {
        // Arrange
        var money = new Money(100, currency);

        // Act & Assert
        Assert.That(money.IsValid, Is.False);
    }

    [Test]
    public void Money_Validate_WithValidCurrency_ReturnsNoErrors()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var errors = money.Validate();

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void Money_Validate_WithInvalidCurrency_ReturnsCurrencyError()
    {
        // Arrange
        var money = new Money(100, "");

        // Act
        var errors = money.Validate().ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors.Any(e => e.Name == "get_Currency"), Is.True);
        }
    }
}