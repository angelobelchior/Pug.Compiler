namespace Pug.Compiler.CodeAnalysis;

public class SyntaxParser(List<Token> tokens)
{
    private Token CurrentToken => tokens[_currentPosition];

    private int _currentPosition;

    public double Parse()
    {
        var result = EvaluateExpressionWithPriority();

        while (CurrentToken.Type is TokenType.Plus or TokenType.Minus)
        {
            if (CurrentToken.Type == TokenType.Plus)
            {
                NextIfTokenIs(TokenType.Plus);
                result += EvaluateExpressionWithPriority();
            }
            else if (CurrentToken.Type == TokenType.Minus)
            {
                NextIfTokenIs(TokenType.Minus);
                result -= EvaluateExpressionWithPriority();
            }
        }

        return result;
    }

    private double EvaluateExpressionWithPriority()
    {
        var result = EvaluateToken();

        while (CurrentToken.Type is TokenType.Multiply or TokenType.Divide)
        {
            if (CurrentToken.Type == TokenType.Multiply)
            {
                NextIfTokenIs(TokenType.Multiply);
                result *= EvaluateToken();
            }
            else if (CurrentToken.Type == TokenType.Divide)
            {
                NextIfTokenIs(TokenType.Divide);
                result /= EvaluateToken();
            }
        }

        return result;
    }

    private double EvaluateToken()
    {
        var token = CurrentToken;
        
        if (token.Type == TokenType.Function)
        {
            NextIfTokenIs(TokenType.Function);
            NextIfTokenIs(TokenType.OpenParenthesis);

            var args = new List<double>();

            if (CurrentToken.Type != TokenType.CloseParenthesis)
            {
                args.Add(Parse());

                while (CurrentToken.Type == TokenType.Comma)
                {
                    NextIfTokenIs(TokenType.Comma);
                    args.Add(Parse());
                }
            }

            NextIfTokenIs(TokenType.CloseParenthesis);

            return InvokeMethod(token, args);
        }

        if (token.Type == TokenType.Plus)
        {
            NextIfTokenIs(TokenType.Plus);
            return EvaluateToken();
        }

        if (token.Type == TokenType.Minus)
        {
            NextIfTokenIs(TokenType.Minus);
            return -EvaluateToken();
        }

        if (token.Type == TokenType.Number)
        {
            NextIfTokenIs(TokenType.Number);
            return double.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        if (token.Type == TokenType.OpenParenthesis)
        {
            NextIfTokenIs(TokenType.OpenParenthesis);
            var result = Parse();
            NextIfTokenIs(TokenType.CloseParenthesis);
            return result;
        }

        throw new Exception($"Unexpected token in Evaluate Token: {token.Type}");
    }

    private void NextIfTokenIs(TokenType type)
    {
        if (CurrentToken.Type == type)
            _currentPosition++;
        else
            throw new Exception($"Unexpected token: {CurrentToken.Type}, expected: {type}");
    }

    private static double InvokeMethod(Token token, List<double> args)
    => token.Value.ToLower() switch
    {
        "sqrt" => args.Count == 1
            ? Math.Sqrt(args[0])
            : throw new Exception("Invalid number of arguments for sqrt"),

        "pow" => args.Count switch
        {
            1 => Math.Pow(args[0], 2),
            2 => Math.Pow(args[0], args[1]),
            _ => throw new Exception("Invalid number of arguments for round")
        },

        "min" => args.Count == 2
            ? Math.Min(args[0], args[1])
            : throw new Exception("Invalid number of arguments for min"),
        
        "max" => args.Count == 2
            ? Math.Max(args[0], args[1])
            : throw new Exception("Invalid number of arguments for max"),

        "round" => args.Count switch
        {
            1 => Math.Round(args[0]),
            2 => Math.Round(args[0], Convert.ToInt32(args[1])),
            _ => throw new Exception("Invalid number of arguments for round")
        },

        "random" => args.Count switch
        {
            0 => Random.Shared.NextDouble(),
            1 => Random.Shared.Next(Convert.ToInt32(args[0])),
            2 => Random.Shared.Next(Convert.ToInt32(args[0]), Convert.ToInt32(args[1])),
            _ => throw new Exception("Invalid number of arguments for random")
        },

        _ => throw new Exception($"Unknown function: {token.Value}")
    };
}