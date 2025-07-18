using Microsoft.AspNetCore.Mvc;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Editor.Endpoints.Models;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Editor.Endpoints;

[ExcludeFromCodeCoverage]
internal static class Compiler
{
    public static void AddCompilerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/compile", (
                [FromBody] Models.Editor editor) =>
            {
                var writer = Console.Out;

                try
                {
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

                    return Results.Ok(new Result(tokens, Variable.ToList(variables), identifiers, output));
                }
                catch (LexerException le)
                {
                    return Results.Ok(Result.FromLexerException(le));
                }
                catch (SyntaxParserException se)
                {
                    return Results.Ok(Result.FromSyntaxParserException(se));
                }
                catch (Exception e)
                {
                    return Results.Ok(Result.FromException(e));
                }
            })
            .WithName("Compile")
            .WithTags("Compiler");
    }
}