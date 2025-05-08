namespace Pug.Compiler.CodeAnalysis;

public enum TokenType
{
    EOF,
    
    Number,
    
    Plus,
    Minus,
    Multiply,
    Divide,
    
    OpenParenthesis,
    CloseParenthesis,
    
    Function,
    Comma
}