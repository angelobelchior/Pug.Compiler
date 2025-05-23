using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Editor.Pages;

public class IndexModel : PageModel
{
    public string Result { get; set; } = string.Empty;

    public string Code { get; set; } =
"""
int idade = 41
if idade == 1 then
    print("igual")
else
    print("diferente")
end
""";
    public List<Token> Tokens { get; set; } = [];
    public List<Identifier> Identifiers { get; set; } = [];

    public void OnPost()
    {
        Code = Request.Form["editor-content"].ToString();

        if (string.IsNullOrWhiteSpace(Code))
            return;
        try
        {
            var writer = Console.Out;
            
            using var sw = new StringWriter();
            Console.SetOut(sw);

            Dictionary<string, Identifier> identifiers = new();
            var lexer = new Lexer(Code);
            Tokens = lexer.ExtractTokens();
            var syntaxParser = new SyntaxParser(identifiers, Tokens);
            Identifiers = syntaxParser.Evaluate();
            
            var result = sw.ToString();
            if (string.IsNullOrEmpty(result))
            {
                Result = string.Empty;
            }
            else
            {
                var parts = result
                    .Split("\n")
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Select(v => $"> {v}<br/>");
                Result = string.Join("", parts);
            }
            
            Console.SetOut(writer);
        }
        catch (Exception e)
        {
            Result = e.Message;
        }
    }
}