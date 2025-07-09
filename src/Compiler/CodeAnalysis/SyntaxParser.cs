using System.Text;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.CodeAnalysis;

public class SyntaxParser(Dictionary<string, Identifier> variables, List<Token> tokens)
{
    private int _currentPosition;

    private Token CurrentToken
        => _currentPosition < tokens.Count
            ? tokens[_currentPosition]
            : tokens.First(t => t.Type == TokenType.EndOfFile);

    private readonly List<Identifier> _identifiers = new();

    public List<Identifier> Evaluate()
    {
        _identifiers.Clear();
        while (_currentPosition < tokens.Count && CurrentToken.Type != TokenType.EndOfFile)
        {
            var identifier = EvaluateExpression();
            _identifiers.Add(identifier);
        }

        return _identifiers;
    }

    private Identifier EvaluateExpression()
        => EvaluateLogicalOr();

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
        var left = EvaluatePlusOrMinusOrRemainder();
        if (!Token.IsMathOperatorType(CurrentToken.Type)) return left;
        var @operator = NextIfTokenIs(CurrentToken.Type);
        var right = EvaluatePlusOrMinusOrRemainder();
        left = EvaluateComparison(left, right, @operator);
        return left;
    }

    private Identifier EvaluatePlusOrMinusOrRemainder()
    {
        var left = EvaluateMultiplyOrDivide();
        while (CurrentToken.Type is TokenType.Plus or TokenType.Minus or TokenType.Remainder)
        {
            var op = NextIfTokenIs(CurrentToken.Type);
            var right = EvaluateMultiplyOrDivide();
            left = EvaluateOperation(left, right, op);
        }

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
            TokenType.If => EvaluateIf(),
            TokenType.DataType => EvaluateDataType(),
            TokenType.Identifier => EvaluateIdentifier(),
            TokenType.Function => EvaluateFunction(),
            TokenType.Plus or TokenType.Minus => EvaluateSignal(),
            TokenType.Number => EvaluateNumber(),
            TokenType.String => EvaluateString(),
            TokenType.Bool => EvaluateBool(),
            TokenType.OpenParenthesis => EvaluateParenthesis(),
            _ => throw SyntaxParserException($"Unexpected token {CurrentToken.Type}")
        };

    private Identifier EvaluateIf()
    {
        NextIfTokenIs(TokenType.If);
        var condition = EvaluateExpression();
        NextIfTokenIs(TokenType.Then);

        if (condition.ToBool())
            ParseBlock(TokenType.Else, TokenType.End);
        else
            SkipBlockUntil(TokenType.Else, TokenType.End);

        if (CurrentToken.Type == TokenType.Else)
        {
            NextIfTokenIs(TokenType.Else);
            if (!condition.ToBool())
                ParseBlock(TokenType.End);
            else
                SkipBlockUntil(TokenType.End);
        }

        NextIfTokenIs(TokenType.End);
        return condition;
    }

    private void ParseBlock(params TokenType[] delimiters)
    {
        while (!delimiters.Contains(CurrentToken.Type) && CurrentToken.Type != TokenType.EndOfFile)
            EvaluateToken();
    }

    private void SkipBlockUntil(params TokenType[] delimiters)
    {
        var nestedIfCount = 0;
        while (CurrentToken.Type != TokenType.EndOfFile)
        {
            if (delimiters.Contains(CurrentToken.Type) && nestedIfCount == 0)
                return;

            if (CurrentToken.Type == TokenType.If)
                nestedIfCount++;
            else if (CurrentToken.Type == TokenType.End && nestedIfCount > 0)
                nestedIfCount--;

            _currentPosition++;
        }
    }

    private Identifier EvaluateDataType()
    {
        var dataTypeToken = NextIfTokenIs(TokenType.DataType);
        if (!Identifier.ContainsDataType(dataTypeToken.Value))
            throw SyntaxParserException($"Unknown type: {dataTypeToken.Value}");

        var checkNext = Peek();

        var name = NextIfTokenIs(TokenType.Identifier).Value;
        var identifier = Identifier.Default(dataTypeToken.Value);
        variables[name] = identifier;

        if (checkNext.Type != TokenType.Assign)
            return identifier;

        NextIfTokenIs(TokenType.Assign);
        var value = EvaluateExpression();
        identifier = value.Cast(dataTypeToken.Value);

        variables[name] = identifier;
        return identifier;
    }

    private Identifier EvaluateIdentifier()
    {
        var checkNext = Peek();
        if (CurrentToken.Type == TokenType.Identifier && checkNext.Type == TokenType.Assign)
            return EvaluateAssignment();

        var token = NextIfTokenIs(TokenType.Identifier);

        return !variables.TryGetValue(token.Value, out var identifier)
            ? throw SyntaxParserException($"Unknown identifier: {token.Value}")
            : identifier;
    }

    private Identifier EvaluateFunction()
    {
        var token = NextIfTokenIs(TokenType.Function);
        NextIfTokenIs(TokenType.OpenParenthesis);

        var args = new List<Identifier>();
        while (CurrentToken.Type != TokenType.CloseParenthesis)
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
            : +result.ToDouble();

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

    private Identifier EvaluateOperation(Identifier left, Identifier right, Token @operator)
    {
        if (left.DataType == DataTypes.String || right.DataType == DataTypes.String)
            return EvaluateStringOperation(left, right, @operator);

        if (Identifier.AllAreNumberTypes(left.DataType, right.DataType))
            return EvaluateNumberOperation(left, right, @operator);

        throw SyntaxParserException($"Unexpected token: {@operator.Type}");
    }

    private Identifier EvaluateStringOperation(Identifier left, Identifier right, Token @operator)
    {
        var value = @operator.Type switch
        {
            TokenType.Plus => left + right.ToString(),
            TokenType.Minus => Minus(left.ToString(), right.ToString()),
            TokenType.Multiply => Multiply(),
            _ => throw SyntaxParserException($"Unexpected token: {@operator.Type}")
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

    private Identifier EvaluateNumberOperation(Identifier left, Identifier right, Token @operator)
    {
        var value = @operator.Type switch
        {
            TokenType.Plus => left.ToDouble() + right.ToDouble(),
            TokenType.Minus => left.ToDouble() - right.ToDouble(),
            TokenType.Multiply => left.ToDouble() * right.ToDouble(),
            TokenType.Divide => left.ToDouble() / right.ToDouble(),
            TokenType.Remainder => left.ToDouble() % right.ToDouble(),
            _ => throw SyntaxParserException($"Unexpected token: {@operator.Type}")
        };

        return new Identifier(DataTypes.Double, value);
    }

    private Identifier EvaluateAssignment()
    {
        var variableName = NextIfTokenIs(TokenType.Identifier).Value;
        NextIfTokenIs(TokenType.Assign);
        var value = EvaluateExpression();

        if (!variables.ContainsKey(variableName))
            throw SyntaxParserException($"Unknown identifier: {variableName}");

        variables[variableName] = value;
        return value;
    }

    private Identifier EvaluateComparison(Identifier left, Identifier right, Token @operator)
    {
        const double epsilon = 1e-6;

        Identifier.EnsureSameTypes(left, right, @operator);

        var result = @operator.Type switch
        {
            TokenType.Equal => Equal(),
            TokenType.NotEqual => !Equal(),
            TokenType.Greater => Greater(),
            TokenType.GreaterOrEqual => Greater() || Equal(),
            TokenType.Less => Less(),
            TokenType.LessOrEqual => Less() || Equal(),
            _ => throw SyntaxParserException($"Unexpected comparison operator: {@operator.Type}")
        };

        return new Identifier(DataTypes.Bool, result);

        bool Equal()
            => (left.DataType, right.DataType) switch
            {
                (DataTypes.Double, _) or (_, DataTypes.Double) =>
                    Math.Abs(left.ToDouble() - right.ToDouble()) < epsilon,

                (DataTypes.Int, DataTypes.Int) =>
                    left.ToInt() == right.ToInt(),

                (DataTypes.Bool, DataTypes.Bool) =>
                    left.ToBool() == right.ToBool(),

                (DataTypes.String, DataTypes.String) =>
                    left.ToString() == right.ToString(),

                _ => ThrowException()
            };

        bool Greater()
            => (left.DataType, right.DataType) switch
            {
                (DataTypes.Double, _) or (_, DataTypes.Double) =>
                    left.ToDouble() > right.ToDouble(),

                (DataTypes.Int, DataTypes.Int) =>
                    left.ToInt() > right.ToInt(),

                _ => ThrowException()
            };

        bool Less()
            => (left.DataType, right.DataType) switch
            {
                (DataTypes.Double, _) or (_, DataTypes.Double) =>
                    left.ToDouble() < right.ToDouble(),

                (DataTypes.Int, DataTypes.Int) =>
                    left.ToInt() < right.ToInt(),

                _ => ThrowException()
            };

        bool ThrowException()
            => throw SyntaxParserException($"Cannot compare types: {left.DataType} and {right.DataType}");
    }

    private Identifier EvaluateLogicalOperation(Identifier left, Identifier right, Token @operator)
    {
        if (left.DataType != DataTypes.Bool || right.DataType != DataTypes.Bool)
            throw SyntaxParserException($"Logical operator {@operator.Type} can only be applied to bool types");

        var result = @operator.Type switch
        {
            TokenType.And => left.ToBool() && right.ToBool(),
            TokenType.Or => left.ToBool() || right.ToBool(),
            _ => throw SyntaxParserException($"Unexpected logical operator: {@operator.Type}")
        };

        return new Identifier(DataTypes.Bool, result);
    }

    private Token Peek()
    {
        var next = _currentPosition + 1;
        return next < tokens.Count
            ? tokens[next]
            : tokens.First(t => t.Type == TokenType.EndOfFile);
    }

    private Token NextIfTokenIs(TokenType type)
    {
        if (CurrentToken.Type != type)
            throw SyntaxParserException(
                $"Unexpected token: {CurrentToken.Type}({CurrentToken.Value}). Expected: {type}");

        var token = CurrentToken;
        _currentPosition++;
        return token;
    }

    private SyntaxParserException SyntaxParserException(string message)
        => new(message,
            CurrentToken,
            tokens,
            variables,
            _identifiers);
}

public class SyntaxParserException(
    string message,
    Token? currentToken = null,
    IReadOnlyList<Token>? tokens = null,
    Dictionary<string, Identifier>? variables = null,
    IReadOnlyList<Identifier>? identifiers = null)
    : Exception(message)
{
    public Token? CurrentToken => currentToken;
    public IReadOnlyList<Token>? Tokens => tokens;
    public IDictionary<string, Identifier>? Variables => variables;
    public IReadOnlyList<Identifier>? Identifiers => identifiers;
}