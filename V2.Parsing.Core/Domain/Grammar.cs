using System.Collections.Generic;

namespace V2.Parsing.Core.Domain
{
    public class Grammar
    {
        public string Name { get; set; }
        public List<Def> Defs { get; set; }
        public List<Pattern> Patterns { get; set; }
        public List<Ignore> Ignores { get; set; }
        public List<Discard> Discards { get; set; }
        public bool CaseSensitive { get; set; }

        public Grammar()
        {
            Defs = new List<Def>();
            Patterns = new List<Pattern>();
            Ignores = new List<Ignore>();
            Discards = new List<Discard>();
        }
    }
}
