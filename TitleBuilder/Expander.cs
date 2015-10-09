using System;
using System.Linq;

namespace TitleBuilder
{
    public class Expander
    {
        public Node Expand(Node node)
        {
            Walk(node, ExpandDollar);
            Walk(node, ExpandIdentifier);
            Walk(node, ExpandIdentifierQuestion);
            Walk(node, ExpandIdentifierEqualToValue);

            return node;
        }

        private void ExpandDollar(Node node)
        {
            if (node.TokenType == TokenType.Dollar)
            {
                node.TokenType = TokenType.Attrib;

                Node parent = node.Parent;
                string text = "";

                while (parent != null)
                {
                    Node identifier = parent.Children.FirstOrDefault(x => x.TokenType == TokenType.Identifier);

                    if (identifier != null)
                    {
                        text = identifier.Text;
                        break;
                    }

                    parent = parent.Parent;
                }

                node.Text = text;
            }
        }

        private void ExpandIdentifier(Node node)
        {
            if (node.Children.Count == 1 && node.Children[0].TokenType == TokenType.Identifier)
            {
                //identifier -> identifier != null ? attrib
                node.AddChild(TokenType.NotEqualTo);
                Node values = node.AddChild(TokenType.Values);
                values.AddChild(TokenType.Null);
                node.AddChild(TokenType.Question);
                Node expressions = node.AddChild(TokenType.Expressions);
                Node expression = expressions.AddChild(TokenType.Expression);
                expression.AddChild(TokenType.Attrib, node.Children[0].Text);
            }
        }

        private void ExpandIdentifierQuestion(Node node)
        {
            if (node.Children.Count > 2
                && node.Children[0].TokenType == TokenType.Identifier
                && node.Children[1].TokenType == TokenType.Question)
            {
                //identifier?expression -> identifier != null ? expression

                node.InsertChild(1, TokenType.NotEqualTo);
                Node values = node.InsertChild(2, TokenType.Values);
                values.AddChild(TokenType.Null);
            }
        }

        private void ExpandIdentifierEqualToValue(Node node)
        {
            if (node.Children.Count == 3
                && node.Children[0].TokenType == TokenType.Identifier
                && (node.Children[1].TokenType == TokenType.EqualTo || node.Children[1].TokenType == TokenType.NotEqualTo)
                && node.Children[2].TokenType == TokenType.Values)
            {
                //identifier=text -> identifier=text?{identifier}
                node.AddChild(TokenType.Question);

                Node expressions = node.AddChild(TokenType.Expressions);
                Node expression = expressions.AddChild(TokenType.Expression);
                expression.AddChild(TokenType.Attrib, node.Children[0].Text);
            }
        }

        private void Walk(Node parent, Action<Node> action)
        {
            foreach (Node child in parent.Children)
            {
                action(child);
                Walk(child, action);
            }
        }
    }
}
