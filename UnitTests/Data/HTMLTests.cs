using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Web;

namespace UnitTests
{
    [TestClass]
    public class HTMLTests
    {
        [TestMethod]
        public void Encoder_Encodes()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); 
            string html = @"<table></table>" ; 
            string encode = HttpUtility.HtmlEncode(html); 
            string decode = HttpUtility.HtmlDecode(encode); 
            Assert.AreEqual(decode, "<table></table>"); 
        } 
        [TestMethod]
        public void Decoder_Decodes()
        {


        }

    }
 
}
