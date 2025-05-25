using System.Text;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.CodeAnalysis;

public class SyntaxParser(Dictionary<string, Identifier> identifiers, List<Token> tokens)
{
    private Token CurrentToken => tokens[_currentPosition];
    private int _currentPosition;

    public List<Identifier> Evaluate()
    {
        var results = new List<Identifier>();
        while (_currentPosition < tokens.Count && CurrentToken.Type != TokenType.EndOfFile)
        {
            var identifier = EvaluateExpression();
            results.Add(identifier);
        }

        return results;
    }

    private Identifier EvaluateExpression()
    {
        return EvaluateLogicalOr();
    }

    private Identifier EvaluateLogicalOr()
    {
        var left = EvaluateLogicalAnd();
        if (CurrentToken.Type != TokenType.Or) return left;
        var or = NextIfTokenIs(TokenType.Or);
        var right = EvaluateLogicalAnd();
        left = EvaluateLogicalOperation(left, right, or);
        return left;
    }

    private Identifier EvaluateLogicalAnd()
    {
        var left = EvaluateComparison();
        if (CurrentToken.Type != TokenType.And) return left;
        var and = NextIfTokenIs(TokenType.And);
        var right = EvaluateComparison();
        left = EvaluateLogicalOperation(left, right, and);
        return left;
    }

    private Identifier EvaluateComparison()
    {
        var left = EvaluatePlusOrMinus();
        if (!Token.IsMathOperatorType(CurrentToken.Type)) return left;
        var @operator = NextIfTokenIs(CurrentToken.Type);
        var right = EvaluatePlusOrMinus();
        left = EvaluateComparison(left, right, @operator);
        return left;
    }

    private Identifier EvaluatePlusOrMinus()
    {
        var left = EvaluateMultiplyOrDivide();
        if (CurrentToken.Type is not (TokenType.Plus or TokenType.Minus)) return left;
        var @operator = NextIfTokenIs(CurrentToken.Type);
        var right = EvaluateMultiplyOrDivide();
        left = EvaluateOperation(left, right, @operator);
        return left;
    }

    private Identifier EvaluateMultiplyOrDivide()
    {
        var left = EvaluateToken();
        while (CurrentToken.Type is TokenType.Multiply or TokenType.Divide)
        {
            var op = NextIfTokenIs(CurrentToken.Type);
            var right = EvaluateToken();
            left = EvaluateOperation(left, right, op);
        }

        return left;
    }

    private Identifier EvaluateToken()
        => CurrentToken.Type switch
        {
            TokenType.DataType => EvaluateDataType(),
            TokenType.Identifier => EvaluateIdentifier(),
            TokenType.Function => EvaluateFunction(),
            TokenType.Plus or TokenType.Minus => EvaluateSignal(),
            TokenType.Number => EvaluateNumber(),
            TokenType.String => EvaluateString(),
            TokenType.Bool => EvaluateBool(),
            TokenType.OpenParenthesis => EvaluateParenthesis(),
            _ => throw new Exception($"Unexpected token {CurrentToken.Type} at position {CurrentToken.Position}")
        };

    private Identifier EvaluateDataType()
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

    private Identifier EvaluateIdentifier()
    {
        var checkNext = Peek();
        if (CurrentToken.Type == TokenType.Identifier && checkNext.Type == TokenType.Assign)
            return EvaluateAssignment();

        var token = NextIfTokenIs(TokenType.Identifier);

        if (!identifiers.TryGetValue(token.Value, out var identifier))
            throw new Exception($"Unknown identifier: {token.Value}");

        return identifier;
    }

    private Identifier EvaluateFunction()
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
        return BuiltInFunctions.Invoke(token.Value, args);
    }

    private Identifier EvaluateSignal()
    {
        var token = NextIfTokenIs(CurrentToken.Type);
        var result = EvaluateToken();
        var value = token.Type == TokenType.Minus
            ? -result.ToDouble()
            : result.ToDouble();

        return new Identifier(result.DataType, value);
    }

    private Identifier EvaluateNumber()
    {
        var token = NextIfTokenIs(TokenType.Number);
        return Identifier.FromToken(token);
    }

    private Identifier EvaluateString()
    {
        var token = NextIfTokenIs(TokenType.String);
        return new Identifier(DataTypes.String, token.Value);
    }

    private Identifier EvaluateBool()
    {
        var token = NextIfTokenIs(TokenType.Bool);
        return new Identifier(DataTypes.Bool, token.Value);
    }

    private Identifier EvaluateParenthesis()
    {
        NextIfTokenIs(TokenType.OpenParenthesis);
        var result = EvaluateExpression();
        NextIfTokenIs(TokenType.CloseParenthesis);
        return result;
    }

    private static Identifier EvaluateOperation(Identifier left, Identifier right, Token @operator)
    {
        if (left.DataType == DataTypes.String || right.DataType == DataTypes.String)
            return EvaluateStringOperation(left, right, @operator);

        if (Identifier.AllAreNumberTypes(left.DataType, right.DataType))
            return EvaluateNumberOperation(left, right, @operator);

        throw new Exception($"Unexpected token: {@operator.Type}");
    }

    private static Identifier EvaluateStringOperation(Identifier left, Identifier right, Token @operator)
    {
        var value = @operator.Type switch
        {
            TokenType.Plus => left + right.ToString(),
            TokenType.Minus => Minus(left.ToString(), right.ToString()),
            TokenType.Multiply => Multiply(),
            _ => throw new Exception($"Unexpected token: {@operator.Type}")
        };

        return new Identifier(DataTypes.String, value);

        static string Minus(string @string, string value)
            => @string.Replace(value, string.Empty);

        string Multiply()
        {
            string @string;
            int count;

            if (left.DataType == DataTypes.String)
            {
                @string = left.ToString();
                count = right.ToInt();
            }
            else
            {
                @string = right.ToString();
                count = left.ToInt();
            }

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
            TokenType.Plus => left.ToDouble() + right.ToDouble(),
            TokenType.Minus => left.ToDouble() - right.ToDouble(),
            TokenType.Multiply => left.ToDouble() * right.ToDouble(),
            TokenType.Divide => left.ToDouble() / right.ToDouble(),
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

    private static Identifier EvaluateComparison(Identifier left, Identifier right, Token @operator)
    {
        Identifier.EnsureSameTypes(left, right, @operator);

        var result = @operator.Type switch
        {
            TokenType.Equal => Equal(),
            TokenType.NotEqual => !Equal(),
            TokenType.Greater => Greater(),
            TokenType.GreaterOrEqual => Greater() || Equal(),
            TokenType.Less => Less(),
            TokenType.LessOrEqual => Less() || Equal(),
            _ => throw new Exception($"Unexpected comparison operator: {@operator.Type}")
        };

        return new Identifier(DataTypes.Bool, result);

        bool Equal()
        {
            if (left.DataType == DataTypes.String || right.DataType == DataTypes.String)
                return left.ToString() == right.ToString();

            if (left.DataType == DataTypes.Double || right.DataType == DataTypes.Double)
                return Math.Abs(left.ToDouble() - right.ToDouble()) < 0.01;

            if (left.DataType == DataTypes.Int || right.DataType == DataTypes.Int)
                return left.ToInt() == right.ToInt();

            if (left.DataType == DataTypes.Double || right.DataType == DataTypes.Int)
                return Math.Abs(left.ToDouble() - right.ToInt()) < 0.01;

            if (left.DataType == DataTypes.Int || right.DataType == DataTypes.Double)
                return Math.Abs(left.ToInt() - right.ToDouble()) < 0.01;

            if (left.DataType == DataTypes.Bool || right.DataType == DataTypes.Bool)
                return left.ToBool() == right.ToBool();

            return left.ToString() == right.ToString();
        }

        bool Greater()
        {
            if (left.DataType == DataTypes.Double || right.DataType == DataTypes.Double)
                return left.ToDouble() > right.ToDouble();

            if (left.DataType == DataTypes.Int || right.DataType == DataTypes.Int)
                return left.ToInt() > right.ToInt();

            if (left.DataType == DataTypes.Double || right.DataType == DataTypes.Int)
                return left.ToDouble() > right.ToInt();

            return left.ToInt() > right.ToDouble();
        }

        bool Less()
        {
            if (left.DataType == DataTypes.Double || right.DataType == DataTypes.Double)
                return left.ToDouble() < right.ToDouble();

            if (left.DataType == DataTypes.Int || right.DataType == DataTypes.Int)
                return left.ToInt() < right.ToInt();

            if (left.DataType == DataTypes.Double || right.DataType == DataTypes.Int)
                return left.ToDouble() < right.ToInt();

            return left.ToInt() < right.ToDouble();
        }
    }

    private static Identifier EvaluateLogicalOperation(Identifier left, Identifier right, Token @operator)
    {
        if (left.DataType != DataTypes.Bool || right.DataType != DataTypes.Bool)
            throw new Exception($"Logical operator {@operator.Type} can only be applied to bool types");

        var result = @operator.Type switch
        {
            TokenType.And => left.ToBool() && right.ToBool(),
            TokenType.Or => left.ToBool() || right.ToBool(),
            _ => throw new Exception($"Unexpected logical operator: {@operator.Type}")
        };

        return new Identifier(DataTypes.Bool, result);
    }

    private Token Peek()
    {
        var nextPosition = _currentPosition + 1;
        return nextPosition < tokens.Count ? tokens[nextPosition] : tokens.Last();
    }

    private Token NextIfTokenIs(TokenType type)
    {
        if (CurrentToken.Type != type)
            throw new Exception($"Unexpected token: {CurrentToken.Type}, expected: {type}");

        var token = CurrentToken;
        _currentPosition++;
        return token;
    }
}