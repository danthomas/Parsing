using System.Reflection;
using Parsing.Core.GrammarDef;

namespace Sql
{
    //Select : select [TopX] [distinct] SelectFields from Table JoinDef*
    //TopX : top Integer
    //SelectFields : star | SelectField [comma SelectField]
    //SelectField : SelectField [as] [Text] 
    //SelectField2 : Text 
    //             | count openParen Field closeParen 
    //             | [min|max] openParen Field closeParen
    //Field : star | ObjectRef                                 // * | Name | alias.Name
    //Join : [inner|left|right] [outer] join Table on ObjectRef equals ObjectRef
    //Table : Text [as] [Text]
    //ObjectRef : Text [dot Text] [dot Text]
    //
    //Text : .+
    //Integer : [0-9]+
    //
    //top : top
    //        
    //dot : .
    //equals : =
    

    public class SqlGrammar : Grammar
    {
        private Def _root;

        public SqlGrammar()
        {
            var dot = new Token("dot", ".");
            var star = new Token("star", "*");
            var equalTo = new Token("equalTo", "=");
            var comma = new Token("comma", ",");
            var openParen = new Token("openParen", "(");
            var closeParen = new Token("closeParen", ")");
            var whitespace1 = new Token("whitespace", " ");
            var whitespace2 = new Token("whitespace", "\t");
            var whitespace3 = new Token("whitespace", "\n");
            var whitespace4 = new Token("whitespace", "\r");
            var openSquare = new Token("openSquare", "[");
            var closeSquare = new Token("closeSquare", "]");

            var _select = new Token("select");
            var _from = new Token("from");
            var _distinct = new Token("distinct");
            var _top = new Token("top");
            var _inner = new Token("inner");
            var _left = new Token("left");
            var _right = new Token("right");
            var _outer = new Token("outer");
            var _join = new Token("join");
            var _on = new Token("on");
            var _as = new Token("as");
            var _count = new Token("count");
            var _min = new Token("min");
            var _max = new Token("max");
            var _avg = new Token("avg");
            var _where = new Token("where");
            var _cross = new Token("cross");
            var _order = new Token("order");
            var _by = new Token("by");
            var _with = new Token("with");
            var _nolock = new Token("nolock");
            var _like = new Token("like");

            var integer = new Text("Integer", "[0-9]+");
            var text = new Text("Text", ".+");
            
            var objectRef = new Def("ObjectRef", text, new Optional(dot, text), new Optional(dot, text));
            
            var table = new Def("Table", text, new Optional(_as), new Optional(text));
            
            var joinDef = new Def("JoinDef", new Optional(new OneOf(_inner, _left, _right)), new Optional(_outer), _join, table, _on, objectRef, equalTo, objectRef);
            
            var starOrObjectRef = new Def("StarOrObjectRef", new OneOf(star, objectRef));

            var aggregate = new Def("Aggregate", new Def("Agg", new OneOf(_count, _min, _max, _avg)), openParen, starOrObjectRef, closeParen);

            var objectRefOrAggregate = new Def("ObjectRefOrAggregate", new OneOf(objectRef, aggregate));
            
            var selectField = new Def("SelectField", objectRefOrAggregate, new Optional(_as), new Optional(text));

            var commaSelectField = new Def("CommaSelectField", comma, selectField);

            var selectFieldList = new Def("SelectFieldList", selectField, new ZeroOrMore(commaSelectField));

            var selectFields = new Def("SelectFields", new OneOf(star, selectFieldList));
            
            var topX = new Def("TopX", _top, integer);
            
            _root = new Def("SelectStatement", _select, new Optional(topX), new Optional(_distinct), selectFields, _from, table, new ZeroOrMore(joinDef));
        }

        public override Thing Root => _root;

        public override char StringQuote => '\'';
    }
}
