using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Parsing
{
    public class Builder
    {
        private readonly ExpressionBuilder _expressionBuilder;

        public Builder()
        {
            _expressionBuilder = new ExpressionBuilder();
        }

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

            expressions.Add(Expression.Block(_expressionBuilder.Build(node.Children[0], helper)));

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
                _values = values.ToDictionary(x => x.Key.ToLower(), x => x.Value);
                _stringBuilder = new StringBuilder();
            }

            public void AppendText(string value)
            {
                _stringBuilder.Append(value);
            }

            public bool IsNull(string key)
            {
                return !_values.ContainsKey(key.ToLower())
                       || String.IsNullOrWhiteSpace(_values[key.ToLower()]);
            }

            public bool IsEqualTo(string key, string value)
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return !_values.ContainsKey(key.ToLower())
                        || String.IsNullOrWhiteSpace(_values[key.ToLower()]);
                }
                else
                {
                    return _values.ContainsKey(key.ToLower())
                           && _values[key.ToLower()].ToLower() == value.ToLower();
                }
            }

            public void AppendValue(string key)
            {
                if (_values.ContainsKey(key.ToLower()))
                {
                    _stringBuilder.Append(_values[key.ToLower()]);
                }
            }

            public string GetValue()
            {
                return _stringBuilder.ToString();
            }
        }

        class ExpressionBuilder
        {
            private readonly MethodInfo _appendValue;
            private readonly MethodInfo _appendText;
            private readonly MethodInfo _isEqualTo;
            private readonly MethodInfo _isNull;

            public ExpressionBuilder()
            {
                _appendText = typeof(Helper).GetMethod("AppendText");
                _appendValue = typeof(Helper).GetMethod("AppendValue");
                _isEqualTo = typeof(Helper).GetMethod("IsEqualTo");
                _isNull = typeof(Helper).GetMethod("IsNull");
            }

            public Expression Build(Node node, ParameterExpression helper)
            {
                if (node.TokenType != TokenType.Expressions)
                {
                    throw new Exception("Expressions Node expected");
                }

                List<Expression> expressions = new List<Expression>();

                foreach (Node child in node.Children)
                {
                    expressions.Add(BuildExpression(child, helper));
                }

                return Expression.Block(expressions);
            }

            private Expression BuildExpression(Node node, ParameterExpression helper)
            {
                if (node.TokenType != TokenType.Expression)
                {
                    throw new Exception("Expression Node expected");
                }

                Expression expression;

                if (node.Children.Count == 1 && node.Children[0].TokenType == TokenType.Text)
                {
                    expression = Expression.Call(helper, _appendText, Expression.Constant(node.Children[0].Text));
                }
                else if (node.Children.Count == 1
                    && (node.Children[0].TokenType == TokenType.Identifier || node.Children[0].TokenType == TokenType.Attrib))
                {
                    expression = Expression.Call(helper, _appendValue, Expression.Constant(node.Children[0].Text));
                }
                else
                {
                    Expression condition = Expression.Call(helper, _isEqualTo, Expression.Constant(node.Children[0].Text), Expression.Constant(node.Children[2].Children[0].Text));

                    foreach (Node value in node.Children[2].Children.Skip(1))
                    {
                        condition = Expression.Or(condition, Expression.Call(helper, _isEqualTo, Expression.Constant(node.Children[0].Text), Expression.Constant(value.Text)));
                    }

                    if (node.Children[1].TokenType == TokenType.NotEqualTo)
                    {
                        condition = Expression.Not(condition);
                    }

                    if (node.Children.Any(x => x.TokenType == TokenType.Question)
                        && node.Children.Any(x => x.TokenType == TokenType.Colon))
                    {
                        expression = Expression.IfThenElse(condition, Build(node.Children[4], helper), Build(node.Children[6], helper));
                    }
                    else if (node.Children.Any(x => x.TokenType == TokenType.Question))
                    {

                        expression = Expression.IfThen(condition, Build(node.Children[4], helper));
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