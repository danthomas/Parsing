namespace V3.Templates
{
    public class ConditionalExpr : ExprBase
    {
        public string Text { get; set; }
        public string Attr { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public BlockExpr TrueExpr { get; set; }
        public BlockExpr FalseExpr { get; set; }

        public ConditionalExpr(string attr, string @operator, string value, BlockExpr trueExpr, BlockExpr falseExpr)
        {
            Attr = attr;
            Operator = @operator;
            Value = value;
            TrueExpr = trueExpr;
            FalseExpr = falseExpr;
        }
    }
}