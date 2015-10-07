using System;
using System.Collections.Generic;
using System.Text;

namespace Parsing.Core.GrammarDef
{
    public class ParserGenerator
    {
        public string Generate(Grammar grammar)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var defs = GetThings(grammar, ThingType.Def);

            foreach(Thing def in defs)
            {
                if (!String.IsNullOrWhiteSpace(def.Name))
                {
                    stringBuilder.Append($@"

        public void {def.Name}(Node<NodeType> parent)
        {{");

                    foreach (Thing thing in def.Children)
                    {
                        if (thing.ThingType == ThingType.Token)
                        {
                            stringBuilder.Append($@"
            Consume(parent, TokenType.{thing.Name}, NodeType.{thing.Name});");
                        }
                        else if (thing.ThingType == ThingType.Optional)
                        {
                            var optionalThings = GetOptionalThings(thing);
                        }
                    }


                    stringBuilder.Append(@"
        }");
                }
            }
            
            return stringBuilder.ToString();
        }

        private List<Thing> GetOptionalThings(Thing parent)
        {
            List<Thing> things = new List<Thing>();
            bool optional = false;

            Walk(parent, thing =>
            {
                if 
            });

            return things;
        } 

        private List<Thing> GetThings(Thing parent, ThingType thingType)
        {
            List<Thing> things = new List<Thing>();

            Walk(parent, thing =>
            {
                if (thing.ThingType == thingType
                 &&  !things.Contains(thing))
                {
                    things.Add(thing);
                }
            });

            return things;
        } 
        
        private void Walk(Thing parent, Action<Thing> action)
        {
            action(parent);
            foreach(Thing child in parent.Children)
            {
                Walk(child, action);
            }
        }
    }
}
