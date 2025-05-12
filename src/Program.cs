using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

Dictionary<string, ExpressionResult> expressionResults = new();

while (true)
{
    try
    {
        Console.Write("> ");

        var line = Console.ReadLine() ?? string.Empty;

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

        var syntaxParser = new SyntaxParser(expressionResults, tokens);
        var result = syntaxParser.Parse();

        if (result.DataType != DataTypes.Empty)
            Console.WriteLine($": {result}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}

[ExcludeFromCodeCoverage]
public partial class Program;