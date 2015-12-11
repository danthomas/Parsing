using System.Collections.Generic;
using System.Linq;

namespace V3.Templates
{
    public class BlockExpr : ExprBase
    {
        public BlockExpr(List<ExprBase> exprs)
        {
            Exprs = exprs;
        }

        public BlockExpr(params ExprBase[] exprs)
        {
            Exprs = exprs.ToList();
        }

        public List<ExprBase> Exprs { get; set; }
    }
}