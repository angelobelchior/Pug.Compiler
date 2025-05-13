using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

Dictionary<string, Identifier> identifiers = new();

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

        if (string.IsNullOrEmpty(line))
            break;

        var lexer = new Lexer(line);
        var tokens = lexer.ExtractTokens();

        var syntaxParser = new SyntaxParser(identifiers, tokens);
        var result = syntaxParser.Evaluate();

        if (result.DataType != DataTypes.None)
            WriteLine(result.Value, ConsoleColor.Blue);
    }
    catch (Exception ex)
    {
        WriteLine(ex.Message, ConsoleColor.Red);
    }
}

static void WriteLine(object message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}

[ExcludeFromCodeCoverage]
public partial class Program;