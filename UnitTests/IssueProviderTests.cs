using BOM.CORE;
using JiraCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace UnitTests
{
    [TestClass]
    public class IssueProvider_Tests
    {
        [TestMethod]
        public void BOMProvider_Provides()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("jira");
            var collection = database.GetCollection<BsonDocument>("issues");

            var ctx = Utils.Context();
            var epiclink = "CS-8098";// CS-7411
            var epic = "2021 FISMA Annual IG Data Call";// 
            ctx.SessionDriver.GetUrl("https://dayman.cyber-balance.com/jira/browse/" + epiclink);

            IList<IWebElement> inputs = ctx.SessionDriver.Driver.FindElements(By.CssSelector("table[id='ghx-issues-in-epic-table'] .ghx-minimal a"));
            List<string> items = new List<string>();
            foreach (var item in inputs) items.Add(item.Text);
            foreach (var item in items)
            {
                ctx.SessionDriver.Pause(450).GetUrl($"https://dayman.cyber-balance.com/jira/si/jira.issueviews:issue-xml/{item}/{item}.xml");
                var src = ctx.SessionDriver.Driver.PageSource.ToString();
                src = src.Substring(src.IndexOf("<item>"), src.IndexOf("</item>") - src.IndexOf("<item>")) + "</item>";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(src);
                string title = doc.SelectSingleNode("//title")?.InnerText.Trim() ?? "";
                string section = Regex.Replace(title, "\\[.*\\] ", "");
                string description = doc.SelectSingleNode("//description")?.InnerText.Trim() ?? "";
                string desc = Regex.Replace(description, "[^A-Za-z0-9]", "").Trim();

                BsonArray issueitems = new BsonArray();
                foreach (var para in description.Split("<p>"))
                {
                    var m = Regex.Match(para, "(\\d{1,2}\\.\\d{0,2})(.*)</p>");
                    if (m.Groups.Count > 1)
                    {
                        var metric = m.Groups[1].Value.Trim().TrimEnd('.');
                        var mettext = m.Groups[2].Value.Trim();
                        issueitems.Add(new BsonDocument { { "metricid", metric }, { "metrictext", mettext } });
                    }
                }
                var post = new BsonDocument  {
                        {"issuekey" , item},
                        {"epiclink" , epiclink},
                        {"epic" , epic},
                        {"title" , title},
                        {"section" , section ?? ""},
                        {"link" , doc.SelectSingleNode("//link")?.InnerText.Trim() ?? ""},
                        {"labels" , doc.SelectSingleNode("//labels")?.InnerText.Trim()  ?? ""},
                        {"version" , doc.SelectSingleNode("//version")?.InnerText.Trim() ?? ""},
                        {"summary" , doc.SelectSingleNode("//summary")?.InnerText.Trim() ?? ""},
                        {"content" , src  ?? ""},
                        {"description" , description  ?? ""},
                        {"desc" , desc  ?? ""} ,
                        {"issueitem" , issueitems }
                    };

                collection.ReplaceOneAsync(
                    filter: new BsonDocument("issuekey", item),
                    options: new ReplaceOptions { IsUpsert = true },
                    replacement: post);
            }
            ctx.SessionDriver.Dispose();

        }
    }
    public static class Utils
    {
        public static ISessionContext Context()
        {
            var config = new UnitTestManager().Configuration;

            var context = config.GetSection("contexts")
                .Get<List<BomContext>>()
                .Where(c => c.conn.Contains("jira")).FirstOrDefault();

            var mock = new Mock<ILogger>();
            ILogger  logger = mock.Object;
            IBScriptParser bomScriptParser = new BScriptParser();
            SessionContext ctx = new SessionContext();
            ctx.SessionDriver = new SessionDriver(config, logger, bomScriptParser, $"{context.conn}");
            ctx.SessionDriver.Connect();
            return ctx;
        }
    }
    
    public class UnitTestManager
    {
        private IConfiguration _config;

        public UnitTestManager()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(Configuration);
        }
        public IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false);
                    _config = builder.Build();
                }

                return _config;
            }
        }

    }
}


/*
       description

            var ig = (from c in IG2020
                      join n in IG2021
                      on new { c.section } equals new { n.section } 
                      select new
                      {
                          c_section = c.section,
                          n_section = n.section,
                          c_content = c.content,
                          n_content = n.content
                      }).ToList();

            foreach (var item in ig)
            {
                var b = item.c_content == item.n_content;
                var aresame = b;
            } 
 */
