namespace Parsing.Core.GrammarDef
{
    public abstract class Grammar
    {
        public abstract Thing Root { get; }
        public abstract char StringQuote { get; }
    }
}