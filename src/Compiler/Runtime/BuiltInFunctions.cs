namespace Pug.Compiler.Runtime;

public static class BuiltInFunctions
{
    private static readonly Dictionary<string, Func<List<Identifier>, Identifier>> Functions = new()
    {
        ["sqrt"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Double, Math.Sqrt(args[0].ToDouble()))
            : throw new Exception("Invalid number of arguments for sqrt"),

        ["pow"] = args => args.Count switch
        {
            1 => Identifier.Create(DataTypes.Double, Math.Pow(args[0].ToDouble(), 2)),
            2 => Identifier.Create(DataTypes.Double, Math.Pow(args[0].ToDouble(), args[1].ToDouble())),
            _ => throw new Exception("Invalid number of arguments for pow")
        },

        ["min"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.Double, Math.Min(args[0].ToDouble(), args[1].ToDouble()))
            : throw new Exception("Invalid number of arguments for min"),

        ["max"] = args => args.Count == 2
            ? Identifier.Create(DataTypes.Double, Math.Max(args[0].ToDouble(), args[1].ToDouble()))
            : throw new Exception("Invalid number of arguments for max"),

        ["round"] = args => args.Count switch
        {
            1 => Identifier.Create(DataTypes.Double, Math.Round(args[0].ToDouble())),
            2 => Identifier.Create(DataTypes.Double, Math.Round(args[0].ToDouble(), args[1].ToInt())),
            _ => throw new Exception("Invalid number of arguments for round")
        },

        ["random"] = args => args.Count switch
        {
            0 => Identifier.Create(DataTypes.Double, Random.Shared.NextDouble()),
            1 => Identifier.Create(DataTypes.Double, Random.Shared.Next(args[0].ToInt())),
            2 => Identifier.Create(DataTypes.Double, Random.Shared.Next(args[0].ToInt(), args[1].ToInt())),
            _ => throw new Exception("Invalid number of arguments for random")
        },
        
        ["upper"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].ToString().ToUpperInvariant())
            : throw new Exception("Invalid number of arguments for upper"),
        
        ["lower"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].ToString().ToLowerInvariant())
            : throw new Exception("Invalid number of arguments for lower"),

        ["len"] = args => args.Count == 1
            ? Identifier.Create(DataTypes.Int, args[0].ToString().Length)
            : throw new Exception("Invalid number of arguments for len"),
        
        ["replace"] = args => args.Count == 3
            ? Identifier.Create(DataTypes.String, args[0].ToString().Replace(args[1].ToString(), args[2].ToString()))
            : throw new Exception("Invalid number of arguments for replace"),
        
        ["substr"] = args => args.Count switch
        {
            2 => Identifier.Create(DataTypes.String, args[0].ToString().Substring(args[1].ToInt())),
            3 => Identifier.Create(DataTypes.String, args[0].ToString().Substring(args[1].ToInt(), args[2].ToInt())),
            _ => throw new Exception("Invalid number of arguments for substr")
        },

        ["print"] = args =>
        {
            if (args.Count != 1)
                throw new Exception("Invalid number of arguments for print");

            Console.WriteLine(args[0].ToString());

            return Identifier.None;
        },
        
        ["clear"] = args =>
        {
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
            : throw new Exception($"Function {functionName} not found");

        return result;
    }
}