using System;

namespace V3.Parsing.Core
{
    public class TokenPattern<T> : PatternBase<T>
    {
        public TokenPattern(T nodeType, string pattern) : base(nodeType)
        {
            Pattern = pattern;
        }

        public string Pattern { get; set; }

        public override IsMatch IsMatch(string text, bool caseSensitive)
        {
            if (String.Equals(Pattern, text, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
            {
                return Core.IsMatch.Yes;
            }

            if (Pattern.Length > text.Length
                && String.Equals(Pattern.Substring(text.Length), text, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
            {
                return Core.IsMatch.Possible;
            }

            return Core.IsMatch.No;
        }
    }
}