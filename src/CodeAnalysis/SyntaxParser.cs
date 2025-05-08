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
                CheckToken(TokenType.Plus);
                result += EvaluateExpressionWithPriority();
            }
            else if (CurrentToken.Type == TokenType.Minus)
            {
                CheckToken(TokenType.Minus);
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
                CheckToken(TokenType.Multiply);
                result *= EvaluateToken();
            }
            else if (CurrentToken.Type == TokenType.Divide)
            {
                CheckToken(TokenType.Divide);
                result /= EvaluateToken();
            }
        }

        return result;
    }

    private double EvaluateToken()
    {
        var token = CurrentToken;
        
        if (token.Type == TokenType.Plus)
        {
            CheckToken(TokenType.Plus);
            return EvaluateToken();
        }

        if (token.Type == TokenType.Minus)
        {
            CheckToken(TokenType.Minus);
            return -EvaluateToken();
        }

        if (token.Type == TokenType.Number)
        {
            CheckToken(TokenType.Number);
            return double.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        if (token.Type == TokenType.OpenParenthesis)
        {
            CheckToken(TokenType.OpenParenthesis);
            var result = Parse();
            CheckToken(TokenType.CloseParenthesis);
            return result;
        }

        throw new Exception($"Unexpected token in Evaluate Token: {token.Type}");
    }

    private void CheckToken(TokenType type)
    {
        if (CurrentToken.Type == type)
            _currentPosition++;
        else
            throw new Exception($"Unexpected token: {CurrentToken.Type}, expected: {type}");
    }
}
