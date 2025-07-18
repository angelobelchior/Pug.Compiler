using System.Globalization;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Runtime;

public class None
{
    public static readonly None Value = new();
}

public class Identifier(DataTypes dataType, object value)
{
    private const string IntType = "int";
    private const string DoubleType = "double";
    private const string BoolType = "bool";
    private const string StringType = "string";

    private static readonly Dictionary<string, Func<Identifier, Identifier>> TypeConverters = new()
    {
        [IntType] = value => new Identifier(DataTypes.Int, value.ToInt()),
        [DoubleType] = value => new Identifier(DataTypes.Double, value.ToDouble()),
        [BoolType] = value => new Identifier(DataTypes.Bool, value.ToBool()),
        [StringType] = value => new Identifier(DataTypes.String, value.ToString()),
    };

    public static readonly Identifier None = new(DataTypes.None, Pug.Compiler.Runtime.None.Value);

    public DataTypes DataType { get; } = dataType;

    public object Value
        => DataType switch
        {
            DataTypes.Int => ToInt(),
            DataTypes.Double => ToDouble(),
            DataTypes.Bool => ToBool(),
            DataTypes.String => ToString(),
            DataTypes.None => Identifier.None,
            _ => throw new SyntaxParserException($"Invalid data type: {DataType}")
        };

    public static Identifier Create<T>(DataTypes types, T value)
        => new(types, value is null ? None.Value : value);

    public static Identifier FromToken(Token token)
        => token.Type switch
        {
            TokenType.Number => new Identifier(DataTypes.Double,
                double.Parse(token.Value, CultureInfo.InvariantCulture)),
            TokenType.Bool => new Identifier(DataTypes.Bool, token.Value),
            TokenType.String => new Identifier(DataTypes.String, token.Value),
            _ => throw new SyntaxParserException($"Invalid token type: {token.Type}")
        };

    public static bool ContainsDataType(string type)
        => TypeConverters.ContainsKey(type);

    public static void EnsureSameTypes(Identifier left, Identifier right, Token @operator)
    {
        if (AllAreNumberTypes(left.DataType, right.DataType))
            return;

        if (left.DataType != right.DataType)
            throw new SyntaxParserException(
                $"Cannot apply {@operator.Type} operator to different types: {left.DataType} and {right.DataType}",
                @operator, [@operator], [], [left, right]);
    }

    public static bool AllAreNumberTypes(params DataTypes[] types)
        => types.All(type => type is DataTypes.Double or DataTypes.Int);

    public static Identifier Default(string typeName)
        => typeName switch
        {
            IntType => new Identifier(DataTypes.Int, 0),
            DoubleType => new Identifier(DataTypes.Double, 0.0),
            BoolType => new Identifier(DataTypes.Bool, false),
            StringType => new Identifier(DataTypes.String, string.Empty),
            _ => throw new SyntaxParserException($"Invalid data type: {typeName}")
        };

    public Identifier Cast(string typeName)
        => DataType switch
        {
            DataTypes.Double when typeName != IntType && typeName != DoubleType
                => throw new SyntaxParserException($"Invalid type {IntType} or {DoubleType}. Expected a {typeName}"),
            DataTypes.Int when typeName != IntType && typeName != DoubleType
                => throw new SyntaxParserException($"Invalid type {IntType} or {DoubleType}. Expected a {typeName}"),
            DataTypes.String when typeName != StringType
                => throw new SyntaxParserException($"Invalid type string. Expected a {typeName}"),
            DataTypes.Bool when typeName != BoolType
                => throw new SyntaxParserException($"Invalid type bool. Expected a {typeName}"),
            _ => TypeConverters.TryGetValue(typeName, out var cast)
                ? cast(this)
                : throw new SyntaxParserException($"Unknown type: {typeName}")
        };

    public double ToDouble()
    {
        if (value is double d)
            return d;

        var @string = value.ToString();
        var success = double.TryParse(
            @string,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result);

        if (!success)
            throw new SyntaxParserException($"Can't convert {value} to double");

        return result;
    }

    public int ToInt()
    {
        if (value is int i)
            return i;

        if (value is double d)
            return (int)Math.Round(d);

        var @string = ToString();
        var success = int.TryParse(@string, out var result);
        if (!success)
            throw new SyntaxParserException($"Can't convert {value} to int");

        return result;
    }

    public bool ToBool()
    {
        if (value is bool b)
            return b;

        var @string = ToString();
        var success = bool.TryParse(@string, out var result);
        if (!success)
            throw new SyntaxParserException($"Can't convert {value} to bool");

        return result;
    }

    public override string ToString()
        => value.ToString() ?? string.Empty;
}