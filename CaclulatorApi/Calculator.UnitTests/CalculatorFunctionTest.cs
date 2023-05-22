using System.Text;
using System.Web.Http;
using AutoFixture;
using Calculator.Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Calculator.UnitTests;

[TestFixture]
public class CalculatorFunctionTest
{
    private Mock<IResultCalculator> _calculatorMock;
    private Mock<IValidator<string>> _validatorMock;
    private DefaultHttpContext _context;
    private IFixture _fixture;
    private Calculator _sut;

    [SetUp]
    public void Setup()
    {
        _calculatorMock = new Mock<IResultCalculator>();
        _validatorMock = new Mock<IValidator<string>>();
        _context = new DefaultHttpContext();
        _fixture = new Fixture();

        _validatorMock.Setup(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _sut = new Calculator(_calculatorMock.Object, _validatorMock.Object);
    }

    [Test]
    public async Task Run_WhenCalledWithValidExpression_ReturnsOkObjectResult()
    {
        //arrange
        var inputString = _fixture.Create<string>();
        var request = SetupRequestBody(inputString);
        var value = _fixture.Create<int>();

        _calculatorMock.Setup(c => c.Calculate(It.IsAny<string>()))
            .Returns(value);

        //act
        var result = await _sut.RunAsync(request, null);

        //assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>().Which.Value.Should().Be(value);
        _calculatorMock.Verify(c => c.Calculate(inputString), Times.Once());
        _validatorMock.Verify(c => c.ValidateAsync(inputString, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task Run_WhenExpressionFailsValidation_ReturnsBadRequestObjectResult()
    {
        //arrange
        var request = SetupFixturedRequestBody();

        var validationFailure = _fixture.Create<ValidationFailure>();
        _validatorMock.Setup(v => 
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { validationFailure }));

        //act
        var result = await _sut.RunAsync(request, null);

        //assert
        result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be($"Invalid expression: {validationFailure.ErrorMessage}");

        _calculatorMock.Verify(c => c.Calculate(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Run_WhenValidatorThrowsException_ReturnsInternalServerErrorResult()
    {
        //arrange
        var request = SetupFixturedRequestBody();

        _validatorMock.Setup(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>();

        //act
        var result = await _sut.RunAsync(request, null);

        //assert
        result.Should().NotBeNull().And.BeOfType<InternalServerErrorResult>();
        _calculatorMock.Verify(c => c.Calculate(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Run_WhenCalculatorThrowsCalculationException_ReturnsBadRequestObjectResult()
    {
        //arrange
        var request = SetupFixturedRequestBody();

        var calculationException = _fixture.Create<CalculationException>();

        _calculatorMock.Setup(c => c.Calculate(It.IsAny<string>()))
            .Throws(() => calculationException);

        //act
        var result = await _sut.RunAsync(request, null);

        //assert
        result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be($"Failed to evaluate the expression : {calculationException.Reason}");
    }

    [Test]
    public async Task Run_WhenCalculatorThrowCalculateException_ReturnsBadRequestResult()
    {
        //arrange
        var request = SetupFixturedRequestBody();

        var calculationException = _fixture.Create<CalculationException>();

        _calculatorMock.Setup(c => c.Calculate(It.IsAny<string>()))
            .Throws(() => calculationException);

        //act
        var result = await _sut.RunAsync(request, null);

        //assert
        result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be($"Failed to evaluate the expression : {calculationException.Reason}");
    }

    [Test]
    public async Task Run_WhenCalculatorThrowException_ReturnsInternalServerErrorResult()
    {
        //arrange
        var request = SetupFixturedRequestBody();

        _calculatorMock.Setup(c => c.Calculate(It.IsAny<string>()))
            .Throws<Exception>();

        //act
        var result = await _sut.RunAsync(request, null);

        //assert
        result.Should().NotBeNull().And.BeOfType<InternalServerErrorResult>();
    }

    private HttpRequest SetupFixturedRequestBody()
    {
        return SetupRequestBody(_fixture.Create<string>());
    }

    private HttpRequest SetupRequestBody(string body)
    {
        var request = _context.Request;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        request.Body = stream;
        return request;
    }
}
