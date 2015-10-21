using System.Collections.Generic;

namespace Parsing.Core.Domain
{
    public class Grammar
    {
        public string Name { get; set; }
        public virtual Thing Root { get; set; }
        public virtual char StringQuote { get; set; }
        public Token[] IgnoreTokens { get; set; }
        public string[] DiscardThings { get; set; }
        public List<Token> Punctuation { get; set; }
        public List<Token> Keywords { get; set; }
        public List<Token> Texts { get; set; }
    }
}