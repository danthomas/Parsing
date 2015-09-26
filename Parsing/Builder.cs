using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parsing
{
    public class Builder
    {
        public Func<IDictionary<string, string>, string> Build(Node node)
        {
            var dictionaryType = typeof(IDictionary<string, string>);

            var values = Expression.Parameter(dictionaryType);

            var helper = Expression.Variable(typeof(Helper));

            var constructorInfo = typeof(Helper).GetConstructor(new[] { dictionaryType });

            List<Expression> expressions = new List<Expression>();


            var assign = Expression.Assign(helper, Expression.New(constructorInfo, values));

            var getValue = Expression.Call(helper, typeof(Helper).GetMethod("GetValue"));

            expressions.Add(assign);

            ExpressionBuilder expressionBuilder = new ExpressionBuilder(node, helper, expressions);

            expressionBuilder.Walk();

            expressions.Add(getValue);


            Expression block = Expression.Block(new[] { helper }, expressions);


            return Expression.Lambda<Func<IDictionary<string, string>, string>>(block, values).Compile();
        }

        private class Helper
        {
            private readonly StringBuilder _stringBuilder;

            private readonly IDictionary<string, string> _values;

            public Helper(IDictionary<string, string> values)
            {
                _values = values;
                _stringBuilder = new StringBuilder();
            }

            public void AppendText(string value)
            {
                _stringBuilder.Append(value);
            }

            public bool IsNull(string key)
            {
                return !_values.ContainsKey(key)
                       || String.IsNullOrWhiteSpace(_values[key]);
            }

            public bool IsEqualTo(string key, string value)
            {
                return _values.ContainsKey(key)
                       && _values[key] == value;
            }

            public void AppendValue(string key)
            {
                if (_values.ContainsKey(key))
                {
                    _stringBuilder.Append(_values[key]);
                }
            }

            public string GetValue()
            {
                return _stringBuilder.ToString();
            }
        }

        class ExpressionBuilder
        {
            private readonly Node _node;
            private readonly ParameterExpression _helper;
            private readonly List<Expression> _expressions;
            private readonly MethodInfo _appendValue;
            private readonly MethodInfo _appendText;
            private readonly MethodInfo _isEqualTo;
            private readonly MethodInfo _isNull;

            public ExpressionBuilder(Node node, ParameterExpression helper, List<Expression> expressions)
            {
                _node = node;
                _helper = helper;
                _expressions = expressions;
                _appendText = typeof(Helper).GetMethod("AppendText");
                _appendValue = typeof(Helper).GetMethod("AppendValue");
                _isEqualTo = typeof(Helper).GetMethod("IsEqualTo");
                _isNull = typeof(Helper).GetMethod("IsNull");
            }

            public void Walk()
            {
                Walk(_node);
            }

            private void Walk(Node parent)
            {
                if (parent.NodeType == NodeType.Expression)
                {
                    Expand(parent);
                    _expressions.Add(BuildExpression(parent));
                }
                else
                {
                    foreach (Node child in parent.Children)
                    {
                        Walk(child);
                    }
                }
            }

            private void Expand(Node parent)
            {
                if (parent.Children.Count == 1 && parent.Children[0].NodeType == NodeType.Text)
                {
                    //text$text => text{attrib}
                }
                else if (parent.Children.Count == 1 && parent.Children[0].NodeType == NodeType.Identifier)
                {
                    //identifier -> identifier != null ? { identifer }
                    parent.Children.Add(new Node(NodeType.NotEqualTo));
                    parent.Children.Add(new Node(NodeType.Null));
                    parent.Children.Add(new Node(NodeType.Question));
                    parent.Children.Add(new Node(NodeType.Expression, "", new Node(NodeType.Identifier, parent.Children[0].Text)));
                }
                else if (parent.Children.Count == 3 && parent.Children[1].NodeType == NodeType.Question)
                {
                    //identifier?expression -> identifier != null ? expression
                    parent.Children.Insert(1, new Node(NodeType.NotEqualTo));
                    parent.Children.Insert(2, new Node(NodeType.Null));
                }
                else if (parent.Children.Count == 3)
                {
                    //identifier =|!= value -> identifier =|!= value ? { identifer }
                    parent.Children.Add(new Node(NodeType.Question));
                    parent.Children.Add(new Node(NodeType.Expression, "", new Node(NodeType.Identifier, parent.Children[0].Text)));
                }
            }

            private Expression BuildExpression(Node node)
            {
                Expression expression = null;

                if (node.Children.Count == 1 && node.Children[0].NodeType == NodeType.Text)
                {
                    expression = Expression.Call(_helper, _appendText, Expression.Constant(node.Children[0].Text));
                }
                else if (node.Children.Count == 1 && node.Children[0].NodeType == NodeType.Identifier)
                {
                    expression = Expression.Call(_helper, _appendValue, Expression.Constant(node.Children[0].Text));
                }
                else
                {
                    if (node.Children.Any(x => x.NodeType == NodeType.Question)
                        && node.Children.Any(x => x.NodeType == NodeType.Colon))
                    {
                        Expression condition = Expression.Call(_helper, _isEqualTo, Expression.Constant(node.Children[0].Text), Expression.Constant(node.Children[2].Text));
                        if (node.Children[1].NodeType == NodeType.NotEqualTo)
                        {
                            condition = Expression.Not(condition);
                        }

                        expression = Expression.IfThenElse(condition, BuildExpression(node.Children[4]), BuildExpression(node.Children[6]));
                    }
                    else if (node.Children.Any(x => x.NodeType == NodeType.Question))
                    {
                        Expression condition = Expression.Call(_helper, _isEqualTo, Expression.Constant(node.Children[0].Text), Expression.Constant(node.Children[2].Text));
                        if (node.Children[1].NodeType == NodeType.NotEqualTo)
                        {
                            condition = Expression.Not(condition);
                        }

                        expression = Expression.IfThen(condition, BuildExpression(node.Children[4]));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                return expression;
            }
        }
    }
}