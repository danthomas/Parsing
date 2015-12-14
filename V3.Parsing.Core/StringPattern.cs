using System;
using System.Linq;

namespace V3.Parsing.Core
{
    public class StringPattern<T> : PatternBase<T>
    {
        public StringPattern(T nodeType, string open, params string[] closes) : base(nodeType)
        {
            Open = open;
            Closes = closes;
        }

        public string Open { get; set; }
        public string[] Closes { get; set; }

        public override IsMatch IsMatch(string text, bool caseSensitive)
        {
            if (text.StartsWith(Open, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
            {
                if (Closes.Any(close => text.EndsWith(close, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)))
                {
                    return Core.IsMatch.Yes;
                }

                return Core.IsMatch.Possible;
            }

            return Core.IsMatch.No;
        }
    }
}