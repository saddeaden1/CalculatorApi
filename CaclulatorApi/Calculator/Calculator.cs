using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Calculator.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Calculator;

public  class Calculator
{
    private readonly IResultCalculator _resultCalculator;
    private readonly IValidator<string> _validator;

    public Calculator(IResultCalculator resultCalculator, IValidator<string> validator)
    {
        _resultCalculator = resultCalculator;
        _validator = validator;
    }

    [FunctionName("calculate")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] 
        HttpRequest req, ILogger log)
    {
        try
        {
            var expression = await new StreamReader(req.Body).ReadToEndAsync();

            var validationResult = await _validator.ValidateAsync(expression);

            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult($"Invalid expression: {validationResult}");
            }

            var result = _resultCalculator.Calculate(expression);
            return new OkObjectResult(result);
        }
        catch (CalculationException e)
        {
            return new BadRequestObjectResult($"Failed to evaluate the expression : {e.Reason}");
        }
        catch
        {
            return new InternalServerErrorResult();
        }
    }
}