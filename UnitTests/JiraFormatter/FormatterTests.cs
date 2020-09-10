using JiraCore.Data;
using JiraFormatter.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class FormatterTests
    {
        [TestMethod]
        public void DescriptionTagger()
        {
            string arg = "<metric><p>0123456789012345</p></metric>";
            DescriptionTagger o = new DescriptionTagger();
            var result = o.Format(arg);
            Assert.IsTrue(result.Contains("question-text"));
        }
        [TestMethod]
        public void ListTagger()
        {
            string arg = "<metric><ul><li>0123456789012345</li><li>0123456789012345</li></ul></metric>";
            ListTagger o = new ListTagger();
            var result = o.Format(arg);
            Assert.IsTrue(result.Contains("picklist"));
        }
        [TestMethod]
        public void ControlTagger()
        {
            string arg = "<metric><ul><li>Checkbox</li></ul></metric>";
            ListTagger o = new ListTagger();
            var result = o.Format(arg);
            Assert.IsTrue(result.Contains("control"));
        }

        [TestMethod]
        public void MetricTagger()
        {
            string arg = "<item><description><p>12.b</p></description></item>";
            MetricTagger o = new MetricTagger();
            var result = o.Format(arg);
            Assert.IsTrue(result.Contains("id-text"));
        }
         
    }
}
