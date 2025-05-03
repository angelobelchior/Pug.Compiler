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

        var parser = new Parser(tokens);
        var result = parser.Parse();

        Console.WriteLine(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}