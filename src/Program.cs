using Pug.Compiler.CodeAnalysis;

while (true)
{
    try
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input) || input.Equals(":q", StringComparison.CurrentCultureIgnoreCase))
            break;

        var lexer = new Lexer(input);
        var tokens = lexer.CreateTokens();

        var syntaxParser = new SyntaxParser(tokens);
        var result = syntaxParser.Parse();

        Console.WriteLine(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}