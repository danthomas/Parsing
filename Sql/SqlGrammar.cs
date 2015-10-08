using System.Reflection;
using Parsing.Core.GrammarDef;

namespace Sql
{
    //Select : select [TopX] [distinct] SelectFields from Table Join*
    //TopX : top Integer
    //SelectFields : * | SelectField [comma SelectField]
    //SelectField : SelectField [as] [Text] 
    //SelectField2 : Text 
    //             | count openParen Field closeParen 
    //             | [min|max] openParen Field closeParen
    //Field : * | ObjectRef                                 // * | Name | alias.Name
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
            var dot = new Token("Dot", ".");
            var star = new Token("Star", "*");
            var equalTo = new Token("EqualTo", "=");
            var comma = new Token("Comma", ",");
            var openParen = new Token("OpenParen", "(");
            var closeParen = new Token("CloseParen", ")");
            var whitespace1 = new Token("CloseParen", " ");
            var whitespace2 = new Token("CloseParen", "\t");
            var whitespace3 = new Token("CloseParen", "\n");
            var whitespace4 = new Token("CloseParen", "\r");
            var openSquare = new Token("CloseParen", "[");
            var closeSquare = new Token("CloseParen", "]");

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
            var _where = new Token("where");
            var _cross = new Token("cross");
            var _order = new Token("order");
            var _by = new Token("by");
            var _with = new Token("with");
            var _nolock = new Token("nolock");
            var _like = new Token("like");

            var integer = new Text("Integer", "[0-9]+");
            var text = new Text("Text", ".+");

            //ObjectRef : Text [dot Text] [dot Text]
            var objectRef = new Def("ObjectRef", text, new Optional(dot, text), new Optional(dot, text));

            //Table : Text [as] [Text]
            var table = new Def("Table", text, new Optional(_as), new Optional(text));

            //JoinDef : [inner|left|right] [outer] join TableRef on ObjectRef equals ObjectRef
            var joinDef = new Def("JoinDef", new OptionalOneOf(_inner, _left, _right), new Optional(_outer), _join, table, _on, objectRef, equalTo, objectRef);

            //Field : * | ObjectRef
            var field = new Def("Field", new OneOf(star, objectRef));

            //SelectField2 : Text 
            //             | count openParen Field closeParen 
            //             | [min|max] openParen Field closeParen
            var selectField2 = new Def("SelectField2", new OneOf(text, 
                new Def(null, _count, openParen, field, closeParen),
                new OptionalOneOf(_min, _max), openParen, field, closeParen));

            //SelectField : SelectField [as] [Text] 
            var selectField = new Def("SelectField", selectField2, new Optional(_as), new Optional(text));

            //SelectFields : * | SelectField [comma SelectField]
            var selectFields = new Def("SelectFields", new OneOf(star, new Def(null, selectField, new Optional(comma, selectField))));

            //TopX : top Integer
            var topX = new Def("TopX", _top, integer);

            //Select : select [TopX] [distinct] SelectFields from Table Join*
            _root = new Def("SelectStatement", _select, new Optional(topX), new Optional(_distinct), selectFields, _from, table, joinDef);
        }

        public override Thing Root => _root;

        public override char StringQuote => '\'';
    }
}
