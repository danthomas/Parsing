using System.Collections.Generic;

namespace V3.Templates
{
    public class ConditionalExpr : ExprBase
    {
        public string Text { get; set; }
        public string Attr { get; set; }
        public string Operator { get; set; }
        public List<string> Values { get; set; }
        public BlockExpr TrueExpr { get; set; }
        public BlockExpr FalseExpr { get; set; }

        public ConditionalExpr(string attr, string @operator, List<string> values, BlockExpr trueExpr, BlockExpr falseExpr)
        {
            Attr = attr;
            Operator = @operator;
            Values = values;
            TrueExpr = trueExpr;
            FalseExpr = falseExpr;
        }
    }
}