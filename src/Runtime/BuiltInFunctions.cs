using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Runtime;

public static class BuiltInFunctions
{
    private static readonly Dictionary<string, Func<List<Identifier>, Identifier>> Functions = new()
    {
        ["sqrt"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Sqrt(args[0].AsDouble()))
            : throw new Exception("Invalid number of arguments for sqrt"),

        ["pow"] = args => args.Count switch
        {
            1 => Identifier.Create(DataTypes.Double, Math.Pow(args[0].AsDouble(), 2)),
            2 => Identifier.Create(DataTypes.Double, Math.Pow(args[0].AsDouble(), args[1].AsDouble())),
            _ => throw new Exception("Invalid number of arguments for pow")
        },

        ["min"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.Double, Math.Min(args[0].AsDouble(), args[1].AsDouble()))
            : throw new Exception("Invalid number of arguments for min"),

        ["max"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.Double, Math.Max(args[0].AsDouble(), args[1].AsDouble()))
            : throw new Exception("Invalid number of arguments for max"),

        ["round"] = args => args.Count switch
        {
            1 => Identifier.Create(DataTypes.Double, Math.Round(args[0].AsDouble())),
            2 => Identifier.Create(DataTypes.Double, Math.Round(args[0].AsDouble(), args[1].AsInt())),
            _ => throw new Exception("Invalid number of arguments for round")
        },

        ["random"] = args => args.Count switch
        {
            0 => Identifier.Create(DataTypes.Double, Random.Shared.NextDouble()),
            1 => Identifier.Create(DataTypes.Double, Random.Shared.Next(args[0].AsInt())),
            2 => Identifier.Create(DataTypes.Double, Random.Shared.Next(args[0].AsInt(), args[1].AsInt())),
            _ => throw new Exception("Invalid number of arguments for random")
        },
        
        ["upper"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].AsString().ToUpperInvariant())
            : throw new Exception("Invalid number of arguments for upper"),
        
        ["lower"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].AsString().ToLowerInvariant())
            : throw new Exception("Invalid number of arguments for lower"),

        ["len"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].AsString().Length)
            : throw new Exception("Invalid number of arguments for len"),
        
        ["replace"] = args => args.Count == 3
            ? Identifier.Create(DataTypes.String, args[0].AsString().Replace(args[1].AsString(), args[2].AsString()))
            : throw new Exception("Invalid number of arguments for replace"),
        
        ["substr"] = args => args.Count switch
        {
            2 => Identifier.Create(DataTypes.String, args[0].AsString().Substring(args[1].AsInt())),
            3 => Identifier.Create(DataTypes.String, args[0].AsString().Substring(args[1].AsInt(), args[2].AsInt())),
            _ => throw new Exception("Invalid number of arguments for substr")
        },

        ["print"] = args =>
        {
            if (args.Count != 1)
                throw new Exception("Invalid number of arguments for print");

            Console.WriteLine(args[0].AsString());

            return Identifier.Create();
        }
    };

    public static bool Contains(string functionName)
        => Functions.ContainsKey(functionName);

    public static Identifier Invoke(Token token, List<Identifier> args)
    {
        var result = Functions.TryGetValue(token.Value, out var function)
            ? function(args)
            : throw new Exception($"Function {token.Value} not found");

        return result;
    }
}