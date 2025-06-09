using System.Globalization;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Runtime;

public class None
{
    public static readonly None Value = new();
}

public class Identifier(DataTypes dataType, object value)
{
    private static readonly Dictionary<string, Func<Identifier, Identifier>> TypeConverters = new()
    {
        ["int"] = value => new Identifier(DataTypes.Int, value.AsInt()),
        ["double"] = value => new Identifier(DataTypes.Double, value.AsDouble()),
        ["bool"] = value => new Identifier(DataTypes.Bool, value.AsBool()),
        ["string"] = value => new Identifier(DataTypes.String, value.AsString()),
    };

    public DataTypes DataType { get; } = dataType;

    public object Value =>
        DataType switch
        {
            DataTypes.Int => AsInt(),
            DataTypes.Double => AsDouble().ToString(CultureInfo.InvariantCulture),
            DataTypes.Bool => AsBool().ToString().ToLower(),
            DataTypes.String => AsString(),
            DataTypes.None => None.Value,
            _ => throw new Exception($"Invalid data type: {DataType}")
        };

    public static Identifier Create<T>(DataTypes types, T value)
        => new(types, value is null ? None.Value : value);

    public static Identifier Create()
        => new(DataTypes.None, None.Value);

    public static Identifier FromToken(Token token)
        => token.Type switch
        {
            TokenType.Number => new Identifier(DataTypes.Double,
                double.Parse(token.Value, CultureInfo.InvariantCulture)),
            TokenType.Bool => new Identifier(DataTypes.Bool, token.Value),
            TokenType.String => new Identifier(DataTypes.String, token.Value),
            _ => throw new Exception($"Invalid token type: {token.Type}")
        };

    public static bool ContainsDataType(string type)
        => TypeConverters.ContainsKey(type);

    public static Identifier Default(string typeName)
        => typeName switch
        {
            "int" => new Identifier(DataTypes.Int, 0),
            "double" => new Identifier(DataTypes.Double, 0),
            "bool" => new Identifier(DataTypes.Bool, false),
            "string" => new Identifier(DataTypes.String, string.Empty),
            _ => throw new Exception($"Invalid data type: {typeName}")
        };

    public Identifier Cast(string typeName)
        => DataType switch
        {
            DataTypes.Double when typeName != "int" && typeName != "double" => throw new Exception(
                $"Invalid type number. Expected a {typeName}"),
            DataTypes.Int when typeName != "int" && typeName != "double" => throw new Exception(
                $"Invalid type number. Expected a {typeName}"),
            DataTypes.String when typeName != "string" => throw new Exception(
                $"Invalid type string. Expected a {typeName}"),
            DataTypes.Bool when typeName != "bool" => throw new Exception(
                $"Invalid type bool. Expected a {typeName}"),
            _ => TypeConverters.TryGetValue(typeName, out var cast)
                ? cast(this)
                : throw new Exception($"Unknown type: {typeName}")
        };


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