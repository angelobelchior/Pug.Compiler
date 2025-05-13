using Pug.Compiler.Runtime;

namespace Pug.Compiler.CodeAnalysis;

public class SyntaxParser(Dictionary<string, ExpressionResult> expressionResults, List<Token> tokens)
{
    private Token CurrentToken => tokens[_currentPosition];
    private int _currentPosition;

    public ExpressionResult Parse()
    {
        var left = EvaluateExpressionWithPriority();

        while (CurrentToken.Type is TokenType.Plus or TokenType.Minus)
        {
            var @operator = NextIfTokenIs(CurrentToken.Type);
            var right = EvaluateExpressionWithPriority();

            left = left.DataType == DataTypes.String || right.DataType == DataTypes.String
                ? EvaluateStringOperation(left, right, @operator)
                : EvaluateNumberOperation(left, right, @operator);
        }

        return left;
    }

    private ExpressionResult EvaluateExpressionWithPriority()
    {
        var left = EvaluateToken();

        while (CurrentToken.Type is TokenType.Multiply or TokenType.Divide)
        {
            var @operator = NextIfTokenIs(CurrentToken.Type);
            var right = EvaluateToken();

            var value = @operator.Type == TokenType.Multiply
                ? left.AsDouble() * right.AsDouble()
                : left.AsDouble() / right.AsDouble();

            left = new ExpressionResult(DataTypes.Double, value);
        }

        return left;
    }

    private ExpressionResult EvaluateToken()
        => CurrentToken.Type switch
        {
            TokenType.DataType => EvaluateDataTypeToken(),
            TokenType.Identifier => EvaluateIdentifierToken(),
            TokenType.Function => EvaluateFunctionToken(),
            TokenType.Plus or TokenType.Minus => EvaluateUnaryOperation(),
            TokenType.Number => EvaluateNumberToken(),
            TokenType.String => EvaluateStringToken(),
            TokenType.Bool => EvaluateBoolToken(),
            TokenType.OpenParenthesis => EvaluateParenthesizedExpression(),
            _ => throw new Exception($"Unexpected token in EvaluateToken: {CurrentToken.Type}")
        };

    private ExpressionResult EvaluateDataTypeToken()
    {
        var dataTypeToken = NextIfTokenIs(TokenType.DataType);
        var variableName = NextIfTokenIs(TokenType.Identifier).Value;
        NextIfTokenIs(TokenType.Assign);

        var value = Parse();

        if (!ExpressionResult.ContainsDataType(dataTypeToken.Value))
            throw new Exception($"Unknown type: {dataTypeToken.Value}");

        var typedValue = value.Cast(dataTypeToken.Value);
        expressionResults[variableName] = typedValue;

        return typedValue;
    }

    private ExpressionResult EvaluateIdentifierToken()
    {
        if (CurrentToken.Type == TokenType.Identifier && Peek().Type == TokenType.Assign)
            return EvaluateAssignment();

        var token = NextIfTokenIs(TokenType.Identifier);

        if (!expressionResults.TryGetValue(token.Value, out var identifier))
            throw new Exception($"Unknown identifier: {token.Value}");

        return identifier;
    }

    private ExpressionResult EvaluateFunctionToken()
    {
        var token = NextIfTokenIs(TokenType.Function);
        NextIfTokenIs(TokenType.OpenParenthesis);

        var args = new List<ExpressionResult>();
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
        return BuiltInFunctions.Invoke(token, args);
    }

    private ExpressionResult EvaluateUnaryOperation()
    {
        var token = NextIfTokenIs(CurrentToken.Type);
        var result = EvaluateToken();
        var value = token.Type == TokenType.Minus ? -result.AsDouble() : result.AsDouble();
        return new ExpressionResult(DataTypes.Double, value);
    }

    private ExpressionResult EvaluateNumberToken()
    {
        var token = NextIfTokenIs(TokenType.Number);
        return ExpressionResult.FromToken(token);
    }

    private ExpressionResult EvaluateStringToken()
    {
        var token = NextIfTokenIs(TokenType.String);
        return new ExpressionResult(DataTypes.String, token.Value);
    }

    private ExpressionResult EvaluateBoolToken()
    {
        var token = NextIfTokenIs(TokenType.Bool);
        return new ExpressionResult(DataTypes.Bool, token.Value);
    }

    private ExpressionResult EvaluateParenthesizedExpression()
    {
        NextIfTokenIs(TokenType.OpenParenthesis);
        var result = Parse();
        NextIfTokenIs(TokenType.CloseParenthesis);
        return result;
    }

    private Token NextIfTokenIs(TokenType type)
    {
        if (CurrentToken.Type != type)
            throw new Exception($"Unexpected token: {CurrentToken.Type}, expected: {type}");

        var token = CurrentToken;
        _currentPosition++;
        return token;
    }

    private static ExpressionResult EvaluateStringOperation(ExpressionResult left, ExpressionResult right,
        Token @operator)
    {
        return @operator.Type == TokenType.Plus
            ? new ExpressionResult(DataTypes.String, left.AsString() + right.AsString())
            : new ExpressionResult(DataTypes.String, left.AsString().Replace(right.AsString(), string.Empty));
    }

    private static ExpressionResult EvaluateNumberOperation(ExpressionResult left, ExpressionResult right,
        Token @operator)
    {
        var value = @operator.Type == TokenType.Plus
            ? left.AsDouble() + right.AsDouble()
            : left.AsDouble() - right.AsDouble();

        return new ExpressionResult(DataTypes.Double, value);
    }

    private ExpressionResult EvaluateAssignment()
    {
        var variableName = NextIfTokenIs(TokenType.Identifier).Value;
        NextIfTokenIs(TokenType.Assign);
        var value = Parse();

        if (!expressionResults.ContainsKey(variableName))
            throw new Exception($"Unknown identifier: {variableName}");

        expressionResults[variableName] = value;
        return value;
    }

    private Token Peek()
    {
        var nextPosition = _currentPosition + 1;
        return nextPosition < tokens.Count ? tokens[nextPosition] : tokens[^1];
    }
}