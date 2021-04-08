using JiraCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
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
        public void IssueProvider_Provides()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoCollection<JiraIssue> col = client.GetDatabase("jira").GetCollection<JiraIssue>("issues");

            List<JiraIssue> ALL = col.Find(x => Regex.IsMatch(x.epiclink, ".*CS.*")).ToList();
            foreach (var item in ALL)
            {
                XmlDocument doc = new XmlDocument(); 
                doc.LoadXml(item.content);

                string description = doc.SelectSingleNode("//description")?.InnerText.Trim() ?? ""; 
                string desc = Regex.Replace(description, "[^A-Za-z0-9]", "").Trim();
                col.UpdateOne(
                    o => o.issuekey == item.issuekey
                    , Builders<JiraIssue>.Update
                    .Set(o => o.description, description)
                    .Set(o => o.desc, desc) 
                 ); 
            }

            List<JiraIssue> IG2021 = col.Find(x => Regex.IsMatch(x.epiclink, ".*8098.*")).ToList();
            List<JiraIssue> IG2020 = col.Find(x => Regex.IsMatch(x.epiclink, ".*7411.*")).ToList();

            var ig = (from c in IG2020
                      join n in IG2021
                      on new { c.section } equals new { n.section }
                      select new
                      {
                          c_section = c.section,
                          n_section = n.section,
                          c_desc = c.desc,
                          n_desc = n.desc ,
                          c_description = c.description,
                          n_description = n.description
                      }).ToList();

            foreach (var item in ig)
            {
               
                if (item.c_desc != item.n_desc)
                {
                    var n_description = item.n_description;
                    var c_description = item.c_description;
                    var n_section = item.n_section;
                }
             
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
