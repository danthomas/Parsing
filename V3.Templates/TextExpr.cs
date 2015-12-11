namespace V3.Templates
{
    public class TextExpr : ExprBase
    {
        public string Text { get; set; }

        public TextExpr(string text)
        {
            Text = text;
        }
    }
}