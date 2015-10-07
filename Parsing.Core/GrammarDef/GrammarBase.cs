using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Core.GrammarDef
{
    public class GrammarBase : Thing
    {
        public GrammarBase(string name) : base(name, Options.None)
        {
        }
    }
}
