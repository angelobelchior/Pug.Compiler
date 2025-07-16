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
    Remainder,

    Assign,

    OpenParenthesis,
    CloseParenthesis,

    Function,
    Comma,

    Identifier,
    DataType,

    If,
    Else,
    End,
    
    Equal,
    NotEqual,
    Greater,
    Less,
    GreaterOrEqual,
    LessOrEqual,
    
    And,
    Or,
}