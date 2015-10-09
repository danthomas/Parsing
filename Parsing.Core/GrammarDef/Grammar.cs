namespace Parsing.Core.GrammarDef
{
    public class Grammar
    {
        public virtual Thing Root { get; set; }
        public virtual char StringQuote { get; set; }
        public Token[] IgnoreTokens { get; set; }
    }
}