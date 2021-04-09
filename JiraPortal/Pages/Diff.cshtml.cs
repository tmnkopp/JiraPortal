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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace JiraPortal.Pages
{
    public class DiffModel : PageModel
    {
        public string Sections  { get; set; }
        public string PrevDescriptions { get; set; }
        public string NextDescriptions { get; set; }
        public string Diff { get; set; }
        [BindProperty(SupportsGet=true) ]
        public string Action { get; set; }
         
        public StringBuilder sbSections = new StringBuilder(); 
        public StringBuilder sbDiff = new StringBuilder(); 
        public StringBuilder sbPrevDescriptions = new StringBuilder();
        public StringBuilder sbNextDescriptions = new StringBuilder();
        public void OnGet()
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
                    .Set(o => o.section, Regex.Replace(item.section, "FY\\d{1,5}", "").Trim())
                 );
            }
        
            List<JiraIssue> Prev = col.Find(x => Regex.IsMatch(x.epiclink, ".*7411.*")).ToList() ;
            List<JiraIssue> Next = col.Find(x => Regex.IsMatch(x.epiclink, ".*8098.*")).ToList() ;

            var merged = (from p in Prev
                          join n in Next
                          on p.section equals n.section
                          where n.title.Contains("Section")
                          select new
                          {
                              ptitle = p.title,
                              ntitle = n.title,
                              pdescription = p.description,
                              ndescription = n.description,
                              plink = p.link,
                              nlink = n.link,
                              pdesc = p.desc,
                              ndesc = n.desc,
                              diff = IsDiff(p.description,  n.description)
                          }).ToList();


            foreach (var item in merged)
            {
                if (item.diff)
                {  
                    sbDiff.Append(item.ntitle + "\n\n");
                    sbDiff.Append(item.ndescription.Replace("\n\n", "") + "\n\n");

                    sbSections.Append(item.ntitle + "\n");
                    sbSections.Append(item.plink + "\n");
                    sbSections.Append(item.nlink + "\n");

                    sbPrevDescriptions.Append(item.ptitle + "\n");
                    sbPrevDescriptions.Append(item.plink + "\n");
                    sbPrevDescriptions.Append(item.pdescription + "\n\n\n");

                    sbNextDescriptions.Append(item.ntitle + "\n");
                    sbNextDescriptions.Append(item.nlink + "\n");
                    sbNextDescriptions.Append(item.ndescription + "\n\n\n");
                }
            }

    
            Sections = sbSections.ToString();
            Diff = sbDiff.ToString();
            PrevDescriptions = sbPrevDescriptions.ToString();
            NextDescriptions = sbNextDescriptions.ToString(); 
        }

        private bool IsDiff(string txtx, string txty) {
            if (txtx.Length == txty.Length) {
                return false;
            } 
            if (txtx != txty) {
                return true;
            }
            return false;
        }
    }
}
