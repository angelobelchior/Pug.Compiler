using Microsoft.AspNetCore.Mvc;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Editor.Endpoints;

public static class Compiler
{
    public static void AddEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/compile", (
                [FromBody] Editor editor) =>
            {
                var writer = Console.Out;

                using var sw = new StringWriter();
                Console.SetOut(sw);

                Dictionary<string, Identifier> variables = new();
                var lexer = new Lexer(editor.Code);
                var tokens = lexer.ExtractTokens();
                var syntaxParser = new SyntaxParser(variables, tokens);
                var identifiers = syntaxParser.Evaluate().Where(i => i.DataType != DataTypes.None).ToList();

                List<string> output = [];
                var console = sw.ToString();
                Console.SetOut(writer);
                if (!string.IsNullOrEmpty(console))
                    output = console
                        .Split("\n")
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .ToList();

                return Results.Ok(new Result(tokens, identifiers, output));
            })
            .WithName("Compile")
            .WithTags("Compiler");
    }
}

public record Editor(string Code);

public record Result(List<Token> Tokens, List<Identifier> Identifiers, List<string> Output);