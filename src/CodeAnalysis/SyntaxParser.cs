using System.Text;
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

            left = EvaluateOperation(left, right, @operator);
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
            left = EvaluateOperation(left, right, @operator);
        }

        return left;
    }

    private Identifier EvaluateToken()
        => CurrentToken.Type switch
        {
            TokenType.DataType => EvaluateDataTypeToken(),
            TokenType.Identifier => EvaluateIdentifierToken(),
            TokenType.Function => EvaluateFunctionToken(),
            TokenType.Plus or TokenType.Minus => EvaluateSignal(),
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

    private Identifier EvaluateSignal()
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
    
    private static Identifier EvaluateOperation(Identifier left, Identifier right, Token @operator)
    {
        switch (left.DataType)
        {
            case DataTypes.String when right.DataType == DataTypes.String:
            case DataTypes.String when right.DataType == DataTypes.Int && @operator.Type == TokenType.Multiply:
            case DataTypes.String when right.DataType == DataTypes.Double && @operator.Type == TokenType.Multiply:
                return EvaluateStringOperation(left, right, @operator);
            case DataTypes.Int when right.DataType == DataTypes.String  && @operator.Type == TokenType.Multiply:
            case DataTypes.Double when right.DataType == DataTypes.String  && @operator.Type == TokenType.Multiply:
                return EvaluateStringOperation(right, left, @operator);
        }

        if (left.DataType == DataTypes.Double ||
            right.DataType == DataTypes.Double ||
            left.DataType == DataTypes.Int ||
            right.DataType == DataTypes.Int)
            return EvaluateNumberOperation(left, right, @operator);

        throw new Exception($"Unexpected token: {@operator.Type}");
    }

    private static Identifier EvaluateStringOperation(Identifier left, Identifier right,
        Token @operator)
    {
        var value = @operator.Type switch
        {
            TokenType.Plus => left.AsString() + right.AsString(),
            TokenType.Minus => left.AsString().Replace(right.AsString(), string.Empty),
            TokenType.Multiply => Multiply(left.AsString(), right.AsInt()),
            _ => throw new Exception($"Unexpected token: {@operator.Type}")
        };

        return new Identifier(DataTypes.String, value);

        static string Multiply(string @string, int count)
        {
            var result = new StringBuilder();
            for (var i = 0; i < count; i++)
                result.Append(@string);
            return result.ToString();
        }
    }

    private static Identifier EvaluateNumberOperation(Identifier left, Identifier right,
        Token @operator)
    {
        var value = @operator.Type switch
        {
            TokenType.Plus => left.AsDouble() + right.AsDouble(),
            TokenType.Minus => left.AsDouble() - right.AsDouble(),
            TokenType.Multiply => left.AsDouble() * right.AsDouble(),
            TokenType.Divide => left.AsDouble() / right.AsDouble(),
            _ => throw new Exception($"Unexpected token: {@operator.Type}")
        };

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
        return nextPosition < tokens.Count ? tokens[nextPosition] : tokens.Last();
    }
}