using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator.Domain;

public class ResultCalculator : IResultCalculator
{
    public double Calculate(string expression)
    {
        List<string> parts = new List<string>(Regex.Split(expression, @"([*/+-])"));

        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i] == "*" || parts[i] == "/")
            {
                double left = double.Parse(parts[i - 1]);
                double right = double.Parse(parts[i + 1]);
                double result = ApplyOp(parts[i][0], right, left);

                parts[i - 1] = result.ToString(CultureInfo.InvariantCulture);
                parts.RemoveRange(i, 2);
                i -= 1;
            }
        }

        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i] == "+" || parts[i] == "-")
            {
                double left = double.Parse(parts[i - 1]);
                double right = double.Parse(parts[i + 1]);
                double result = ApplyOp(parts[i][0], right, left);

                parts[i - 1] = result.ToString(CultureInfo.InvariantCulture);
                parts.RemoveRange(i, 2);
                i -= 1;
            }
        }

        return double.Parse(parts[0]);
    }

    private double ApplyOp(char op, double b, double a)
    {
        switch (op)
        {
            case '+':
                return a + b;
            case '-':
                return a - b;
            case '*':
                return a * b;
            case '/':
                if (b == 0)
                    throw new CalculationException { Reason = "Invalid calculation due to division by zero" };
                return a / b;
        }
        return 0;
    }
}