using System;
using System.Linq;

namespace V3.Parsing.Core
{
    public class StringPattern<T> : PatternBase<T>
    {
        public StringPattern(T nodeType, string open, string close) : base(nodeType)
        {
            Open = open;
            Close = close;
        }

        public string Open { get; set; }
        public string Close { get; set; }

        public override IsMatch IsMatch(string text, bool caseSensitive)
        {
            if (text.StartsWith(Open, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
            {
                var contains = text.Substring(Open.Length).Contains(Close);
                var endsWith = text.Substring(Open.Length).EndsWith(Close);

                if (contains && !endsWith)
                {
                    return Core.IsMatch.No;
                }

                if (endsWith)
                {
                    return Core.IsMatch.Yes;
                }

                return Core.IsMatch.Possible;
            }

            return Core.IsMatch.No;
        }
    }
}