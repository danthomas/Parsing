using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace V3.Templates
{
    public class FuncBuilder
    {
        public Func<IDictionary<string, string>, string> Build(string template)
        {
            template = template
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ");

            while (template.Contains("  "))
            {
                template = template.Replace("  ", " ");
            }

            template = template.Trim();

            BlockExpr blockExpr = new ExprBuilder().Build(new Parser().Parse(template));

            ParameterExpression helperExpression = Expression.Variable(typeof(Helper));
            ParameterExpression valuesParamExpression = Expression.Parameter(typeof(IDictionary<string, string>));

            List<Expression> expressions = new List<Expression>
            {
                Expression.Assign(helperExpression, Expression.New(typeof(Helper).GetConstructor(new[] { typeof(IDictionary<string, string>) }), valuesParamExpression))
            };

            AddExpressions(blockExpr, helperExpression, expressions);

            expressions.Add(Expression.Property(helperExpression, "Title"));

            Expression block = Expression.Block(new[] { helperExpression }, Expression.Block(expressions));

            return Expression.Lambda<Func<IDictionary<string, string>, string>>(block, valuesParamExpression).Compile();
        }

        private static void AddExpressions(BlockExpr blockExpr, Expression helperExpression, List<Expression> expressions)
        {
            if (blockExpr == null)
            {
                return;
            }

            foreach (ExprBase expr in blockExpr.Exprs)
            {
                TextExpr textExpr = expr as TextExpr;
                ConditionalExpr conditionalExpr = expr as ConditionalExpr;
                AttrExpr attrExpr = expr as AttrExpr;

                if (textExpr != null)
                {
                    var methodInfo = typeof(Helper).GetMethod("AddText", new[] { typeof(string) });

                    expressions.Add(Expression.Call(helperExpression, methodInfo, Expression.Constant(textExpr.Text)));
                }
                else if (conditionalExpr != null)
                {
                    List<Expression> trueExpressions = new List<Expression>();
                    List<Expression> falseExpressions = new List<Expression>();

                    AddExpressions(conditionalExpr.TrueExpr, helperExpression, trueExpressions);
                    AddExpressions(conditionalExpr.FalseExpr, helperExpression, falseExpressions);

                    Expression conditionExpression = null;

                    if (conditionalExpr.Values.Count == 1 && conditionalExpr.Values[0] == null)
                    {
                        MethodInfo operatorMethod = typeof(Helper).GetMethod(conditionalExpr.Operator == "=" ? "IsNull" : "IsNotNull", new[] { typeof(string) });
                        Expression[] paramExpressions = { Expression.Constant(conditionalExpr.Attr) };
                        conditionExpression = Expression.Call(helperExpression, operatorMethod, paramExpressions);
                    }
                    else
                    {
                        MethodInfo operatorMethod = typeof(Helper).GetMethod(conditionalExpr.Operator == "=" ? "IsEqualTo" : "IsNotEqualTo", new[] { typeof(string), typeof(string) });

                        foreach (string value in conditionalExpr.Values)
                        {
                            Expression[] paramExpressions = { Expression.Constant(conditionalExpr.Attr), Expression.Constant(value) };

                            if (conditionExpression == null)
                            {
                                conditionExpression = Expression.Call(helperExpression, operatorMethod, paramExpressions);
                            }
                            else
                            {
                                conditionExpression = conditionalExpr.Operator == "="
                                    ? Expression.Or(conditionExpression, Expression.Call(helperExpression, operatorMethod, paramExpressions))
                                    : Expression.And(conditionExpression, Expression.Call(helperExpression, operatorMethod, paramExpressions));
                            }
                        }

                    }


                    if (conditionalExpr.FalseExpr == null)
                    {
                        expressions.Add(Expression.IfThen(conditionExpression
                                , Expression.Block(trueExpressions)));
                    }
                    else
                    {
                        expressions.Add(Expression.IfThenElse(conditionExpression
                                , Expression.Block(trueExpressions)
                                , Expression.Block(falseExpressions)));
                    }
                }
                else if (attrExpr != null)
                {
                    var addAttrMethod = typeof(Helper).GetMethod("AddAttr", new[] { typeof(string), typeof(string) });

                    expressions.Add(Expression.Call(helperExpression, addAttrMethod, Expression.Constant(attrExpr.Name), Expression.Constant(attrExpr.Regex, typeof(string))));
                }
            }
        }

        internal class Helper
        {
            private readonly IDictionary<string, string> _attrs;
            private readonly StringBuilder _stringBuilder;

            public Helper(IDictionary<string, string> attrs)
            {
                _attrs = attrs;
                _stringBuilder = new StringBuilder();
            }

            public void AddText(string text)
            {
                _stringBuilder.Append(text);
            }

            public void AddAttr(string key, string regex)
            {
                if (_attrs.ContainsKey(key))
                {
                    var value = _attrs[key];

                    if (!String.IsNullOrWhiteSpace(regex))
                    {
                        var matches = new Regex(regex).Matches(value);

                        value = matches.Count > 0 ? matches[0].Value : "";
                    }

                    _stringBuilder.Append(value);
                }
            }

            public bool IsNull(string key)
            {
                return !_attrs.ContainsKey(key) || String.IsNullOrWhiteSpace(_attrs[key]);
            }

            public bool IsNotNull(string key)
            {
                return _attrs.ContainsKey(key) && !String.IsNullOrWhiteSpace(_attrs[key]);
            }

            public bool IsEqualTo(string key, string value)
            {
                return _attrs.ContainsKey(key) && String.Equals(_attrs[key], value, StringComparison.OrdinalIgnoreCase);
            }

            public bool IsNotEqualTo(string key, string value)
            {
                return !_attrs.ContainsKey(key) || !String.Equals(_attrs[key], value, StringComparison.OrdinalIgnoreCase);
            }

            public string Title
            {
                get
                {
                    var title = _stringBuilder.ToString().Trim();
                    while (title.Contains("  "))
                    {
                        title = title.Replace("  ", " ");
                    }
                    while (title.Contains(", , "))
                    {
                        title = title.Replace(", , ", ", ");
                    }
                    while (title.Contains(" ,"))
                    {
                        title = title.Replace(" ,", ",");
                    }
                    while (title.Contains(",,"))
                    {
                        title = title.Replace(",,", ",");
                    }
                    return title;
                }
            }
        }
    }
}
