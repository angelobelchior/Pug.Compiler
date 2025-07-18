using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Runtime;

public static class BuiltInFunctions
{
    private static readonly Dictionary<string, Func<List<Identifier>, Identifier>> Functions = new()
    {
        ["log"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Log(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for log", "log"),

        ["exp"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Exp(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for exp", "exp"),

        ["sin"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Sin(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for sin", "sin"),

        ["cos"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Cos(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for cos", "cos"),

        ["tan"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Tan(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for tan", "tan"),

        ["atn"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Atan(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for atn", "atn"),
        
        ["abs"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Abs(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for abs", "abs"),

        ["sgn"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].ToDouble() < 0 ? -1 : (args[0].ToDouble() == 0 ? 0 : 1))
            : throw SyntaxParserException("Invalid number of arguments for sgn", "sgn"),
        
        ["sqrt"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Sqrt(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for sqrt", "sqrt"),

        ["pow"] = args => args.Count switch
        {
            1 => Identifier.Create(DataTypes.Double, Math.Pow(args[0].ToDouble(), 2)),
            2 => Identifier.Create(DataTypes.Double, Math.Pow(args[0].ToDouble(), args[1].ToDouble())),
            _ => throw SyntaxParserException("Invalid number of arguments for pow", "pow")
        },

        ["min"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.Double, Math.Min(args[0].ToDouble(), args[1].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for min", "min"),

        ["max"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.Double, Math.Max(args[0].ToDouble(), args[1].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for max", "max"),

        ["round"] = args => args.Count switch
        {
            1 => Identifier.Create(DataTypes.Double, Math.Round(args[0].ToDouble())),
            2 => Identifier.Create(DataTypes.Double, Math.Round(args[0].ToDouble(), args[1].ToInt())),
            _ => throw SyntaxParserException("Invalid number of arguments for round", "round")
        },

        ["random"] = args => args.Count switch
        {
            0 => Identifier.Create(DataTypes.Double, Random.Shared.Next()),
            1 => Identifier.Create(DataTypes.Double, Random.Shared.Next(args[0].ToInt())),
            2 => Identifier.Create(DataTypes.Double, Random.Shared.Next(args[0].ToInt(), args[1].ToInt())),
            _ => throw SyntaxParserException("Invalid number of arguments for random", "random")
        },

        ["upper"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.String, args[0].ToString().ToUpperInvariant())
            : throw SyntaxParserException("Invalid number of arguments for upper", "upper"),

        ["lower"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.String, args[0].ToString().ToLowerInvariant())
            : throw SyntaxParserException("Invalid number of arguments for lower", "lower"),

        ["len"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].ToString().Length)
            : throw SyntaxParserException("Invalid number of arguments for len", "len"),

        ["replace"] = args => args.Count == 3
            ? Identifier.Create(DataTypes.String, args[0].ToString().Replace(args[1].ToString(), args[2].ToString()))
            : throw SyntaxParserException("Invalid number of arguments for replace", "replace"),

        ["substr"] = args => args.Count switch
        {
            2 => Identifier.Create(DataTypes.String, args[0].ToString().Substring(args[1].ToInt())),
            3 => Identifier.Create(DataTypes.String, args[0].ToString().Substring(args[1].ToInt(), args[2].ToInt())),
            _ => throw SyntaxParserException("Invalid number of arguments for substr", "substr")
        },
        
        ["left"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.String, args[0].ToString().Substring(0, args[1].ToInt()))
            : throw SyntaxParserException("Invalid number of arguments for left", "left"),
        
        ["right"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.String, args[0].ToString().Substring(args[0].ToString().Length - args[1].ToInt()))
            : throw SyntaxParserException("Invalid number of arguments for right", "right"),
        
        ["mid"] = args => args.Count == 3
            ? Identifier.Create(DataTypes.String, args[0].ToString().Substring(args[1].ToInt(), args[2].ToInt()))
            : throw SyntaxParserException("Invalid number of arguments for mid", "mid"),
        
        ["trim"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.String, args[0].ToString().Trim())
            : throw SyntaxParserException("Invalid number of arguments for trim", "trim"),
        
        ["trim_end"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.String, args[0].ToString().TrimEnd())
            : throw SyntaxParserException("Invalid number of arguments for trim", "trim_end"),
        
        ["trim_start"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.String, args[0].ToString().TrimStart())
            : throw SyntaxParserException("Invalid number of arguments for trim", "trim_start"),
        
        ["to_str"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.String, args[0])
            : throw SyntaxParserException("Invalid number of arguments for to_string", "to_str"),

        ["to_bool"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Bool, args[0])
            : throw SyntaxParserException("Invalid number of arguments for to_bool", "to_bool"),
        
        ["to_int"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, Math.Floor(args[0].ToDouble()))
            : throw SyntaxParserException("Invalid number of arguments for int", "to_int"),
        
        ["to_double"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, args[0])
            : throw SyntaxParserException("Invalid number of arguments for double", "to_double"),

        ["iif"] = args =>
        {
            if (args.Count != 3)
                throw SyntaxParserException("Invalid number of arguments for iif", "iif");

            if (args[0].DataType != DataTypes.Bool)
                throw SyntaxParserException("First argument of iif must be a boolean", "iif");

            if (args[1].DataType != args[2].DataType)
                throw SyntaxParserException("Second and third arguments of iif must be of the same type", "iif");

            return args[0].ToBool() ? args[1] : args[2];
        },

        ["print"] = args =>
        {
            if (args.Count != 1)
                throw SyntaxParserException("Invalid number of arguments for print", "print");

            Console.WriteLine(args[0].ToString());

            return Identifier.None;
        },
        
        ["read"] = args =>
        {
            if (args.Count != 0)
                throw SyntaxParserException("Invalid number of arguments for read", "read");

            var response = Console.ReadLine();
            return Identifier.Create(DataTypes.String, response);
        },

        ["clear"] = args =>
        {
            if (args.Count != 0)
                throw SyntaxParserException("Invalid number of arguments for clear", "clear");
            
            Console.Clear();

            return Identifier.None;
        },
    };

    public static bool Contains(string functionName)
        => Functions.ContainsKey(functionName);

    public static Identifier Invoke(string functionName, List<Identifier> args)
    {
        var result = Functions.TryGetValue(functionName, out var function)
            ? function(args)
            : throw SyntaxParserException($"Function {functionName} not found", functionName);

        return result;
    }

    private static SyntaxParserException SyntaxParserException(
        string message, string functionName)
        => new(message, Token.Function(0, functionName), [], new Dictionary<string, Identifier>(), []);
}