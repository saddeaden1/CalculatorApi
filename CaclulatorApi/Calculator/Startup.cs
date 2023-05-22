using Calculator;
using Calculator.Domain;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Calculator;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IResultCalculator,ResultCalculator>();
        builder.Services.AddScoped<IValidator<string>, CalculationValidator>();
    }
}