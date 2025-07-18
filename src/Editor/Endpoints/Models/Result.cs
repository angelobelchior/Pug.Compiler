using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Editor.Endpoints.Models;

[ExcludeFromCodeCoverage]
public record Result(
    IReadOnlyList<Token> Tokens,
    IReadOnlyList<Variable> Variables,
    IReadOnlyList<Identifier> Identifiers,
    IReadOnlyList<string> Output,
    Token? CurrentToken = null,
    bool HasError = false)
{
    public static Result FromLexerException(LexerException le)
        => new(le.Tokens, [], [], [le.Message], Token.Identifier(le.CurrentPosition, le.CurrentChar.ToString()), true);

    public static Result FromSyntaxParserException(SyntaxParserException se)
        => new(
            se.Tokens ?? [],
            Variable.ToList(se.Variables ?? []),
            se.Identifiers ?? [],
            [se.Message],
            se.CurrentToken, true);

    public static Result FromException(Exception e)
        => new(new Result([], [], [], [e.Message], null, true));
}