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

            var getValue = Expression.Call(helper, typeof(Helper).GetMethod("GetValue"), Expression.Constant("abc"));

            expressions.Add(assign);
            expressions.Add(getValue);


            Expression block = Expression.Block(new[] { helper }, expressions);


            return Expression.Lambda<Func<IDictionary<string, string>, string>>(block, values).Compile();
        }

        class Helper
        {
            private readonly IDictionary<string, string> _values;

            public Helper(IDictionary<string, string> values)
            {
                _values = values;
            }

            public string GetValue(string key)
            {
                return _values[key];
            }
        }
    }
}
