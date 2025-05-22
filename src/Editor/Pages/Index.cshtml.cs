using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Editor.Pages;

public class IndexModel : PageModel
{
    public string ConsoleResult { get; set; } = string.Empty;
    public string Code { get; set; } = "print(\"Olá Mundo!\")";
    public List<Token> Tokens { get; set; } = [];
    public List<Identifier> Identifiers { get; set; } = [];

    public void OnPost()
    {
        Code = Request.Form["editor-content"].ToString();
        
        if (string.IsNullOrWhiteSpace(Code))
            return;
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);
        
            Dictionary<string, Identifier> identifiers = new();
            var lexer = new Lexer(Code);
            Tokens = lexer.ExtractTokens();
            var syntaxParser = new SyntaxParser(identifiers, Tokens);
            Identifiers = syntaxParser.Evaluate();
            ConsoleResult = sw.ToString().Replace("\n", "<br/>");
        }
        catch (Exception e)
        {
            ConsoleResult = e.Message;
        }
    }
}