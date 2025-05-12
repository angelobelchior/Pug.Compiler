using System.Globalization;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Runtime;

public class ExpressionResultVoid
{
    public static readonly ExpressionResultVoid Void = new();
}

public class ExpressionResult(DataTypes dataType, object value)
{
    private static readonly Dictionary<string, Func<ExpressionResult, ExpressionResult>> _typeConverters = new()
    {
        ["int"] = value => new ExpressionResult(DataTypes.Int, Convert.ToInt32(value.AsDouble())),
        ["double"] = value => new ExpressionResult(DataTypes.Double, value.AsDouble()),
        ["bool"] = value => new ExpressionResult(DataTypes.Bool, value.AsBool()),
        ["string"] = value => new ExpressionResult(DataTypes.String, value.AsString()),
    };

    public DataTypes DataType { get; } = dataType;

    public object Value =>
        DataType switch
        {
            DataTypes.Int => AsInt(),
            DataTypes.Double => AsDouble().ToString(CultureInfo.InvariantCulture),
            DataTypes.Bool => AsBool().ToString().ToLower(),
            DataTypes.String => AsString(),
            DataTypes.Empty => ExpressionResultVoid.Void,
            _ => throw new Exception($"Invalid data type: {DataType}")
        };

    public static ExpressionResult Create<T>(DataTypes types, T value)
        => new(types, value is null ? ExpressionResultVoid.Void : value);

    public static ExpressionResult Create()
        => new(DataTypes.Empty, ExpressionResultVoid.Void);

    public static ExpressionResult FromToken(Token token)
        => token.Type switch
        {
            TokenType.Number => new ExpressionResult(DataTypes.Double,
                double.Parse(token.Value, CultureInfo.InvariantCulture)),
            TokenType.Bool => new ExpressionResult(DataTypes.Bool, token.Value),
            TokenType.String => new ExpressionResult(DataTypes.String, token.Value),
            _ => throw new Exception($"Invalid token type: {token.Type}")
        };

    public static bool ContainsDataType(string type)
        => _typeConverters.ContainsKey(type);

    public ExpressionResult Cast(string typeName)
        => _typeConverters.TryGetValue(typeName, out var cast)
            ? cast(this)
            : throw new Exception($"Unknown type: {typeName}");

    public double AsDouble()
    {
        var @string = AsString();
        var success = double.TryParse(@string, out var result);
        if (!success)
            throw new Exception($"Can't convert {value} to double");

        return result;
    }

    public int AsInt()
    {
        var @string = AsString();
        var success = int.TryParse(@string, out var result);
        if (!success)
            throw new Exception($"Can't convert {value} to int");

        return result;
    }

    public bool AsBool()
    {
        var @string = AsString();
        var success = bool.TryParse(@string, out var result);
        if (!success)
            throw new Exception($"Can't convert {value} to bool");

        return result;
    }

    public string AsString()
        => value.ToString() ?? string.Empty;
}