using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using JiraCore.Data;
using JiraCore.Models;
using JiraFormatter.Formatters;
using JiraFormatter.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
 
namespace JiraPortal.Pages
{
    public class TemplateModel : PageModel
    {
        public string Input { get; set; }
        public string Output { get; set; }
        [BindProperty]
        public string PK { get; set; }
        [BindProperty]
        public string GROUP { get; set; }
        [BindProperty] 
        public string Sections { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Action { get; set; }

        public StringBuilder sbInput = new StringBuilder();
        public StringBuilder sbOutput = new StringBuilder();
        public void OnPost()
        { 
            ProcessForm(); 
        }
        public void OnGet()
        {
            PK = "20000";
            GROUP = "2";
            ProcessForm();
        }
        private void ProcessForm() {
            string DB_Update = "";
            using (TextReader tr = System.IO.File.OpenText(@"C:\_som\_src\_compile\IG\DB_Update7.34_IG_2021.sql"))
                DB_Update = tr.ReadToEnd();
            using (TextReader tr = System.IO.File.OpenText(@"C:\_som\T\SQL\PK_QUESTION_INSERT.sql")) 
                Input = tr.ReadToEnd();
         
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoCollection<JiraIssue> col = client.GetDatabase("jira").GetCollection<JiraIssue>("issues");
      
            List<JiraIssue> Prev = col.Find(x => Regex.IsMatch(x.epiclink, ".*7411.*")).ToList();
            List<JiraIssue> Next = col.Find(x => Regex.IsMatch(x.epiclink, ".*8098.*")).ToList(); 
            List<JiraIssue> merged = (from p in Prev  join n in Next
                          on p.section equals n.section where n.title.Contains("Section") 
                          && p.desc != n.desc  && n.title.Contains("Database")
                          select n).ToList();
             
            Output = "";
            foreach (JiraIssue issue in merged)
            {
                Output += "\n\n" + issue.section;
                foreach (var issueitem in issue.issueitem)
                {
                    string issitem = Regex.Replace(issueitem.metrictext, "[^A-Za-z0-9]", "").Trim();
                    string script = Regex.Replace(DB_Update, "[^A-Za-z0-9]", "").Trim();
                    if (!script.Contains(issitem))
                    {
                        Output += "\n" + issueitem.metricid + ' ' + issueitem.metrictext;
                        Output += "\n\n" + issitem + '\n' + script;
                    }
                }   
            }
        } 
    }
}

// var formatter = new DescriptionCleanup();
// string content = formatter.Format(issue.description);
// Output += "\n\n\n\n";
// foreach (var para in content.Split("<p>"))
// {
//     var m = Regex.Match(para, "(\\d{1,2}\\.\\s)(.*)</p>");
//     if (m.Groups.Count > 1)
//     {
//         var g1 = m.Groups[1];
//         var g2 = m.Groups[2];
//         Output += "\n" + Input
//             .Replace("20000", $"{Convert.ToInt32(PK) + cnt++}")
//             .Replace("QUESTION_TEXT", g2.Value)
//             .Replace("0,0,", $"0,{cnt++},")
//             .Replace("@PK_QGroup+0,", $"@PK_QGroup+{GROUP.Trim()},")
//             .Replace("N'0.0'", $"N'{g1.Value.Trim()}'");
// 
//     } 
//     Sections += issue.title + "\n";
//     Sections += issue.link + "\n"; 
// }

/*

       List<JiraIssue> ALL = col.Find(x => Regex.IsMatch(x.epiclink, ".*CS.*")).ToList();
foreach (var item in ALL)
{
XmlDocument doc = new XmlDocument();
doc.LoadXml(item.content); 
string description = doc.SelectSingleNode("//description")?.InnerText.Trim() ?? ""; 
col.UpdateOne(
    o => o.issuekey == item.issuekey
    , Builders<JiraIssue>.Update
    .Set(o => o.description, description)
    .Set(o => o.desc, Regex.Replace(description, "[^A-Za-z0-9]", "").Trim())
    .Set(o => o.section, Regex.Replace(item.section, "FY\\d{1,5}", "").Trim())
 );
} 

 */

