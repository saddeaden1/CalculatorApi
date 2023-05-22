using FluentAssertions;

namespace Calculator.Domain.UnitTests;

[TestFixture]
public class ResultCalculatorTests
{
    private ResultCalculator _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new ResultCalculator();
    }

    [TestCase("1+2", 3)]
    [TestCase("10+20", 30)]
    [TestCase("3+5", 8)]
    public void Calculate_AdditionExpression_ShouldReturnExpectedResult(string expression, double expected)
    {
        // Act
        var result = _sut.Calculate(expression);

        // Assert
        result.Should().Be(expected);
    }

    [TestCase("1-2", -1)]
    [TestCase("10-5", 5)]
    [TestCase("15-5", 10)]
    public void Calculate_SubtractionExpression_ShouldReturnExpectedResult(string expression, double expected)
    {
        // Act
        var result = _sut.Calculate(expression);

        // Assert
        result.Should().Be(expected);
    }

    [TestCase("2*3", 6)]
    [TestCase("5*5", 25)]
    [TestCase("10*3", 30)]
    public void Calculate_MultiplicationExpression_ReturnsExpectedResult(string expression, double expected)
    {
        // Act
        var result = _sut.Calculate(expression);

        // Assert
        result.Should().Be(expected);
    }

    [TestCase("6/2", 3)]
    [TestCase("10/2", 5)]
    [TestCase("25/5", 5)]
    public void Calculate_DivisionExpression_ReturnsExpectedResult(string expression, double expected)
    {
        // Act
        var result = _sut.Calculate(expression);

        // Assert
        result.Should().Be(expected);
    }

    [TestCase("2+3*2", 8)]
    [TestCase("4+5/2-1", 5.5)]
    [TestCase("10/2*3", 15)]
    [TestCase("10+2*3-4/2", 14)]
    [TestCase("100-25*3+50/2", 50)]
    public void Calculate_MixedOperationsExpression_ReturnsExpectedResult(string expression, double expected)
    {
        // Act
        var result = _sut.Calculate(expression);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void Calculate_DivisionByZero_ThrowsCalculationException()
    {
        // Arrange
        var expression = "1/0";

        // Act
        Action act = () => _sut.Calculate(expression);

        // Assert
        act.Should().Throw<CalculationException>()
            .WithMessage("Invalid calculation due to division by zero");
    }
}