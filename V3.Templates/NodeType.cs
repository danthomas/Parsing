namespace V3.Templates
{
    public enum NodeType
    {
        Text,
        OpenCurly,
        CloseCurly,
        Identifier,
        EndOfFile,
        Expr,
        TextOrSubExpr,
        SubExpr,
        Else,
        Then,
        Equals,
        Value,
        Dollar,
        Or,
        Values,
        Whitespace
    }
}