using FluentAssertions;
using FluentValidation.TestHelper;

namespace Calculator.UnitTests;


public class CalculationValidatorTests
{
    private CalculationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CalculationValidator();
    }

    [Test]
    public void Validate_EmptyExpression_ValidationError()
    {
        var result = _validator.TestValidate("");

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(expression => expression)
            .WithErrorMessage("Expression is empty.");
    }

    [Test]
    public void Validate_ExpressionHasAdjacentOperators_ValidationError()
    {
        var result = _validator.TestValidate("2++3");

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(expression => expression)
            .WithErrorMessage("Expression contains adjacent operators.");
    }

    [Test]
    public void Validate_ExpressionHasInvalidCharacters_ValidationError()
    {
        var result = _validator.TestValidate("2a+3");

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(expression => expression)
            .WithErrorMessage("Expression contains invalid characters.");
    }

    [TestCase("2+3")]
    [TestCase("2-3*4")]
    [TestCase("2/3+4-5")]
    [TestCase("20+30-40*50/60")]
    public void Validate_ValidExpression_NoValidationError(string validExpression)
    {
        var result = _validator.TestValidate(validExpression);

        result.IsValid.Should().BeTrue();
    }

    [TestCase("+2+3")]
    [TestCase("2+3+")]
    [TestCase("+2+3*4-")]
    public void Validate_ExpressionStartsOrEndsWithOperator_ValidationError(string expression)
    {
        var result = _validator.TestValidate(expression);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(expr => expr)
            .WithErrorMessage("Expression is invalid.");
    }

    [TestCase("+2+3")]
    [TestCase("2+3+")]
    public void Validate_ExpressionNotStartEndWithNumber_ValidationError(string expression)
    {
        var result = _validator.TestValidate(expression);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(expr => expr)
            .WithErrorMessage("Expression is invalid.");
    }
}
