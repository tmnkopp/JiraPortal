using JiraCore.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class IssueProviderTests
    {
        [TestMethod]
        public void IssueProvider_Provides()
        {
            string src = "C:\\temp\\jira\\!SAOP2019$DataCallProcessor\\_dest";
            var result = IssueProvider.Load(src);
            Assert.IsNotNull(result);
        }
    }
}
