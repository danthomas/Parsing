using System.Text.RegularExpressions;

namespace V3.Parsing.Core
{
    public class RegexPattern<T> : PatternBase<T>
    {
        public RegexPattern(T nodeType, string pattern) : base(nodeType)
        {
            Regex = new Regex(pattern);
        }

        public Regex Regex { get; set; }

        public override IsMatch IsMatch(string text, bool caseSensitive)
        {
            return Regex.IsMatch(text) ? Core.IsMatch.Yes : Core.IsMatch.No;
        }
    }
}