namespace Calculator.Domain;

public interface IResultCalculator
{
    double Calculate(string expression);
}