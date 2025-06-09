namespace Pug.Compiler.CodeAnalysis;

public enum TokenType
{
    EndOfFile,
    
    Number,
    Bool,
    String,
    
    Plus,
    Minus,
    Multiply,
    Divide,
    
    Assign,
    
    OpenParenthesis,
    CloseParenthesis,
    
    Function,
    Comma,
    
    Identifier,
    DataType
}