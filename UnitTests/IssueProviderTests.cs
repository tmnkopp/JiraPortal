using BOM.CORE;
using JiraCore;
using JiraCore.Data;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace UnitTests
{ 
    [TestClass]
    public class IssueProvider_Tests
    {
        [TestMethod]
        public void IssueProvider_Providers()
        { 
            var issues = new JiraIssueProvider(issue => Regex.IsMatch(issue.epic, ".*")).Items; 
            Assert.IsNotNull(issues);
        }
        [TestMethod]
        public void IssuePopulator_Populates()
        {
            var config = new UnitTestManager().Configuration;
            var mock = new Mock<ILogger>();
            ILogger logger = mock.Object; 
            JiraEpic jiraEpic = new JiraEpic("https://dayman.cyber-balance.com/jira/browse/CS-8132");
            EpicPopulator epicPopulator = new EpicPopulator(config, logger);
            epicPopulator.Populate(jiraEpic);
        } 
    }
    public static class Utils
    {
        public static ISessionContext Context()
        {
            var config = new UnitTestManager().Configuration;

            var context = config.GetSection("contexts")
                .Get<List<BomConfigContext>>()
                .Where(c => c.conn.Contains("jira")).FirstOrDefault();

            var mock = new Mock<ILogger>();
            ILogger  logger = mock.Object; 
            SessionContext ctx = new SessionContext();
            ctx.SessionDriver = new SessionDriver(config, logger);
            ctx.SessionDriver.Connect(ctx.configContext.conn);
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
                    var builder = new ConfigurationBuilder() 
                        .SetBasePath("c:\\bom\\")
                        .AddJsonFile($"appsettings.json", optional: false);
                    _config = builder.Build();
                } 
                return _config;
            }
        } 
    }
}
 