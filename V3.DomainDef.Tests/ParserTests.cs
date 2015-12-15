using NUnit.Framework;

namespace V3.DomainDef.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Test()
        {
            Parser parser = new Parser();

            var domainDef = @"domain CrossFitJST
entity Account";

            var node = parser.Parse(domainDef);

            var domain = new Domain(node);

            var actual = domain.ToString();

            Assert.That(actual, Is.EqualTo(domainDef));
        }
    }

}
