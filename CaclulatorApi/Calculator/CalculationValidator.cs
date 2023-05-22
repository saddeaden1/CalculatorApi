using FluentValidation;
using System.Text.RegularExpressions;

namespace Calculator;

public class CalculationValidator : AbstractValidator<string>
{
    public CalculationValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Expression is empty.")
            .Must(StartAndEndWithNumber).WithMessage("Expression is invalid.")
            .Must(NotHaveAdjacentOperators).WithMessage("Expression contains adjacent operators.")
            .Matches(@"^[0-9\+\-\*\/\s]+$").WithMessage("Expression contains invalid characters.");
    }

    private bool StartAndEndWithNumber(string expression)
    {
        // Making sure the expression starts and ends with a number
        return Regex.IsMatch(expression, @"^[0-9].*[0-9]$");
    }

    private bool NotHaveAdjacentOperators(string expression)
    {
        // Making sure there are no adjacent operators
        return !Regex.IsMatch(expression, @"\D{2,}");
    }
}