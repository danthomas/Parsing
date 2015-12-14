using System;
using System.Collections.Generic;
using System.Linq;
using V3.Parsing.Core;

namespace V3.Templates
{
    public class ExprBuilder
    {
        public BlockExpr Build(Node<NodeType> root)
        {
            var blockExpr = new BlockExpr();

            foreach (var child in root.Nodes)
            {
                if (child.NodeType == NodeType.Text)
                {
                    blockExpr.Exprs.Add(new TextExpr(child.Text));
                }
                else if (child.NodeType == NodeType.SubExpr)
                {
                    var attr = child.Nodes.Single(x => x.NodeType == NodeType.Identifier).Text;
                    var valuesNode = child.Nodes.Single(x => x.NodeType == NodeType.Values);
                    List<string> values = null;

                    if (valuesNode != null)
                    {
                        values = valuesNode.Nodes.Select(x => x.Text).ToList();
                    }

                    ConditionalExpr conditionalExpr = new ConditionalExpr(attr, values, null, null);
                    blockExpr.Exprs.Add(new TextExpr(child.Text));
                }
                else
                {
                    throw new Exception();
                }
            }

            return blockExpr;
        }
    }
}
