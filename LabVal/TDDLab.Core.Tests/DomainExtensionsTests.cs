using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class DomainExtensionsTests
{
    [Test]
    public void ToCurrency_SameCurrency_ReturnsEquivalentMoney()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money.ToCurrency("USD");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Amount, Is.EqualTo(100UL));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }
    }

    [Test]
    public void ToCurrency_DifferentCurrency_CallsCurrencyConverter()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money.ToCurrency("EUR");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Currency, Is.EqualTo("EUR"));
            Assert.That(result.Amount, Is.EqualTo(100UL));
        }
    }

    [Theory]
    [TestCase(null, TestName = "Null target currency")]
    [TestCase("", TestName = "Empty target currency")]
    public void ToCurrency_WithInvalidTargetCurrency_CreatesMoneyWithSpecifiedCurrency(string? targetCurrency)
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money.ToCurrency(targetCurrency);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Amount, Is.EqualTo(100UL));
            Assert.That(result.Currency, Is.EqualTo(targetCurrency));
        }
    }

    [Theory]
    [TestCase(0UL, TestName = "Zero amount")]
    [TestCase(ulong.MaxValue, TestName = "Maximum amount")]
    public void ToCurrency_WithEdgeCaseAmounts_HandlesCorrectly(ulong amount)
    {
        // Arrange
        var money = new Money(amount, "USD");

        // Act
        var result = money.ToCurrency("EUR");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Amount, Is.EqualTo(amount));
            Assert.That(result.Currency, Is.EqualTo("EUR"));
        }
    }
}

[TestFixture]
public class CurrencyConverterTests
{
    [Test]
    public void Convert_SameCurrency_ReturnsOriginalAmount()
    {
        // Arrange
        const ulong amount = 100;

        // Act
        var result = CurrencyConverter.Convert("USD", "USD", amount);

        // Assert
        Assert.That(result, Is.EqualTo(amount));
    }

    [Test]
    public void Convert_DifferentCurrency_ReturnsOriginalAmount()
    {
        // Arrange
        const ulong amount = 100;

        // Act
        var result = CurrencyConverter.Convert("USD", "EUR", amount);

        // Assert
        Assert.That(result, Is.EqualTo(amount));
    }

    [Test]
    public void Convert_WithZeroAmount_ReturnsZero()
    {
        // Arrange
        const ulong amount = 0;

        // Act
        var result = CurrencyConverter.Convert("USD", "EUR", amount);

        // Assert
        Assert.That(result, Is.EqualTo(0UL));
    }

    [Test]
    public void Convert_WithMaxAmount_HandlesCorrectly()
    {
        // Arrange
        const ulong amount = ulong.MaxValue;

        // Act
        var result = CurrencyConverter.Convert("USD", "EUR", amount);

        // Assert
        Assert.That(result, Is.EqualTo(ulong.MaxValue));
    }
}