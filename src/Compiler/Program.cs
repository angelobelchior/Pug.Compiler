using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

Dictionary<string, Identifier> identifiers = new();

var printTokens = false;

while (true)
{
    try
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("> ");

        var line = Console.ReadLine() ?? string.Empty;

        Console.ResetColor();

        if (line.Equals("/quit", StringComparison.CurrentCultureIgnoreCase))
            return;

        if (line.Equals("/cls", StringComparison.CurrentCultureIgnoreCase))
        {
            Console.Clear();
            continue;
        }

        if (line.Equals(":t", StringComparison.CurrentCultureIgnoreCase))
        {
            printTokens = !printTokens;
            Console.WriteLine($"Print tokens: {printTokens}");
            continue;
        }

        if (string.IsNullOrEmpty(line)) break;

        var lexer = new Lexer(line);
        var tokens = lexer.ExtractTokens();

        if (printTokens)
            PrintTokens(tokens);

        var syntaxParser = new SyntaxParser(identifiers, tokens);
        var results = syntaxParser.Evaluate();

        foreach (var result in results.Where(result => result.DataType != DataTypes.None))
            WriteLine(result.Value, ConsoleColor.Blue);
    }
    catch (Exception ex)
    {
        WriteLine(ex.Message, ConsoleColor.Red);
    }
}

return;

static void WriteLine(object message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}

static void PrintTokens(IEnumerable<Token> tokens)
{
    foreach (var token in tokens)
        Console.WriteLine(token);
}

[ExcludeFromCodeCoverage]
public partial class Program
{
    
}