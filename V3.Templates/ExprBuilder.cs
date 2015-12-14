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

            Build(blockExpr, root);

            return blockExpr;
        }

        public BlockExpr Build(BlockExpr blockExpr, Node<NodeType> parent, string name = "")
        {
            foreach (var child in parent.Nodes)
            {
                if (child.NodeType == NodeType.Text)
                {
                    blockExpr.Exprs.Add(new TextExpr(child.Text));
                }
                else if (child.NodeType == NodeType.Dollar)
                {
                    blockExpr.Exprs.Add(new AttrExpr(name, ""));
                }
                else if (child.NodeType == NodeType.SubExpr)
                {
                    var attr = child.Nodes.Single(x => x.NodeType == NodeType.Identifier).Text;
                    var regexNode = child.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Regex);
                    string regex = null;
                    if (regexNode != null)
                    {
                        regex = regexNode.Nodes[0].Text;
                    }

                    var @operator = child.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Equals) != null
                        ? "="
                        : "!=";

                    var valuesNode = child
                        .Nodes
                        .SingleOrDefault(x => x.NodeType == NodeType.Values);

                    var values = valuesNode?.Nodes.Select(x => x.Text).ToList() ?? new List<string> { null };

                    BlockExpr thenExpr;
                    BlockExpr elseExpr = null;

                    var then = child.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Then);

                    thenExpr = new BlockExpr();

                    if (then != null)
                    {
                        Build(thenExpr, then, attr);

                        var @else = then.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Else);

                        if (@else != null)
                        {
                            elseExpr = new BlockExpr();

                            Build(elseExpr, @else, attr);
                        }
                    }
                    else
                    {
                        thenExpr.Exprs.Add(new AttrExpr(attr, regex));
                    }

                    blockExpr.Exprs.Add(new ConditionalExpr(attr, @operator, values, thenExpr, elseExpr));
                }
                else if (child.NodeType == NodeType.Else)
                {

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
