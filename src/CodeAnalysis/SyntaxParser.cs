using Pug.Compiler.Runtime;

namespace Pug.Compiler.CodeAnalysis;

public class SyntaxParser(Dictionary<string, Identifier> identifiers, List<Token> tokens)
{
    private Token CurrentToken => tokens[_currentPosition];
    private int _currentPosition;

    public Identifier Parse()
        => EvaluateExpression();

    private Identifier EvaluateExpression()
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

    private Identifier EvaluateExpressionWithPriority()
    {
        var left = EvaluateToken();

        while (CurrentToken.Type is TokenType.Multiply or TokenType.Divide)
        {
            var @operator = NextIfTokenIs(CurrentToken.Type);
            var right = EvaluateToken();

            var value = @operator.Type == TokenType.Multiply
                ? left.AsDouble() * right.AsDouble()
                : left.AsDouble() / right.AsDouble();

            left = new Identifier(DataTypes.Double, value);
        }

        return left;
    }

    private Identifier EvaluateToken()
        => CurrentToken.Type switch
        {
            TokenType.DataType => EvaluateDataTypeToken(),
            TokenType.Identifier => EvaluateIdentifierToken(),
            TokenType.Function => EvaluateFunctionToken(),
            TokenType.Plus or TokenType.Minus => EvaluateOperation(),
            TokenType.Number => EvaluateNumberToken(),
            TokenType.String => EvaluateStringToken(),
            TokenType.Bool => EvaluateBoolToken(),
            TokenType.OpenParenthesis => EvaluateParenthesizedExpression(),
            _ => throw new Exception($"Unexpected token in EvaluateToken: {CurrentToken.Type}")
        };

    private Identifier EvaluateDataTypeToken()
    {
        var dataTypeToken = NextIfTokenIs(TokenType.DataType);
        if (!Identifier.ContainsDataType(dataTypeToken.Value))
            throw new Exception($"Unknown type: {dataTypeToken.Value}");

        var checkNext = Peek();

        var name = NextIfTokenIs(TokenType.Identifier).Value;
        var identifier = Identifier.Default(dataTypeToken.Value);
        identifiers[name] = identifier;

        if (checkNext.Type != TokenType.Assign)
            return identifier;

        NextIfTokenIs(TokenType.Assign);
        var value = EvaluateExpression();
        identifier = value.Cast(dataTypeToken.Value);

        identifiers[name] = identifier;
        return identifier;
    }

    private Identifier EvaluateIdentifierToken()
    {
        var checkNext = Peek();
        if (CurrentToken.Type == TokenType.Identifier && checkNext.Type == TokenType.Assign)
            return EvaluateAssignment();

        var token = NextIfTokenIs(TokenType.Identifier);

        if (!identifiers.TryGetValue(token.Value, out var identifier))
            throw new Exception($"Unknown identifier: {token.Value}");

        return identifier;
    }

    private Identifier EvaluateFunctionToken()
    {
        var token = NextIfTokenIs(TokenType.Function);
        NextIfTokenIs(TokenType.OpenParenthesis);

        var args = new List<Identifier>();
        if (CurrentToken.Type != TokenType.CloseParenthesis)
        {
            args.Add(EvaluateExpression());
            while (CurrentToken.Type == TokenType.Comma)
            {
                NextIfTokenIs(TokenType.Comma);
                args.Add(EvaluateExpression());
            }
        }

        NextIfTokenIs(TokenType.CloseParenthesis);
        return BuiltInFunctions.Invoke(token, args);
    }

    private Identifier EvaluateOperation()
    {
        var token = NextIfTokenIs(CurrentToken.Type);
        var result = EvaluateToken();
        var value = token.Type == TokenType.Minus ? -result.AsDouble() : result.AsDouble();
        return new Identifier(DataTypes.Double, value);
    }

    private Identifier EvaluateNumberToken()
    {
        var token = NextIfTokenIs(TokenType.Number);
        return Identifier.FromToken(token);
    }

    private Identifier EvaluateStringToken()
    {
        var token = NextIfTokenIs(TokenType.String);
        return new Identifier(DataTypes.String, token.Value);
    }

    private Identifier EvaluateBoolToken()
    {
        var token = NextIfTokenIs(TokenType.Bool);
        return new Identifier(DataTypes.Bool, token.Value);
    }

    private Identifier EvaluateParenthesizedExpression()
    {
        NextIfTokenIs(TokenType.OpenParenthesis);
        var result = EvaluateExpression();
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

    private static Identifier EvaluateStringOperation(Identifier left, Identifier right,
        Token @operator)
    {
        return @operator.Type == TokenType.Plus
            ? new Identifier(DataTypes.String, left.AsString() + right.AsString())
            : new Identifier(DataTypes.String, left.AsString().Replace(right.AsString(), string.Empty));
    }

    private static Identifier EvaluateNumberOperation(Identifier left, Identifier right,
        Token @operator)
    {
        var value = @operator.Type == TokenType.Plus
            ? left.AsDouble() + right.AsDouble()
            : left.AsDouble() - right.AsDouble();

        return new Identifier(DataTypes.Double, value);
    }

    private Identifier EvaluateAssignment()
    {
        var variableName = NextIfTokenIs(TokenType.Identifier).Value;
        NextIfTokenIs(TokenType.Assign);
        var value = EvaluateExpression();

        if (!identifiers.ContainsKey(variableName))
            throw new Exception($"Unknown identifier: {variableName}");

        identifiers[variableName] = value;
        return value;
    }

    private Token Peek()
    {
        var nextPosition = _currentPosition + 1;
        return nextPosition < tokens.Count ? tokens[nextPosition] : tokens[^1];
    }
}