namespace Parsing.Core.Domain
{
    public class Grammar
    {
        public string Name { get; set; }
        public virtual Thing Root { get; set; }
        public virtual char StringQuote { get; set; }
        public Token[] IgnoreTokens { get; set; }
        public string[] DiscardThings { get; set; }
        public Def[] DiscardDefs { get; set; }
    }
}