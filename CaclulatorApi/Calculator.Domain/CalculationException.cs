namespace Calculator.Domain;

public class CalculationException : Exception
{
    public string Reason { get; set; }

    public override string Message => Reason;
}