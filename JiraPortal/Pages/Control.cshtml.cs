using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using JiraCore;
using JiraCore.Data;
using JiraCore.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
 
namespace JiraPortal.Pages
{
    public class ControlModel : PageModel
    {
        public string Head { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public string Right { get; set; }
        public string Left { get; set; }
        public int PLT { get; set; } = 299;
        public int PL { get; set; } = 3780;
        public int QPK { get; set; } = 22595;
        private void ProcessForm() {
  
            var issue = new JiraIssueProvider(i => Regex.IsMatch(i.epic, ".*BOD 18-02.*2021.*") && Regex.IsMatch(i.title, ".*8156.*"))
                .Items.FirstOrDefault();

            string source = "";
            string src = "";
            using (TextReader tr = System.IO.File.OpenText(@"D:\dev\CyberScope\CyberScopeBranch\CSwebdev\database\DB_Update7.34_BOD_2021.sql"))
                src = tr.ReadToEnd();
    
            int qsorter = 0;
            foreach (JiraIssueItem item in ParseIssueItems(src))
            { 
                QPK++;
                if (item.control.Contains("Select all"))
                {
                    using (TextReader tr = System.IO.File.OpenText(@"C:\_som\_src\MULTICHECKBOX.aspx"))
                        source = tr.ReadToEnd();
                }

                Body += string.Format(source, item.metricid, ""); 
                //Control 
            }
            Body = Body.Substring(0, Body.Length - 1);
        }

        public List<JiraIssueItem> ParseIssueItems(string content){
             
            List<JiraIssueItem> items = new List<JiraIssueItem>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); 
            doc.LoadHtml(content);

            var tables = doc.DocumentNode.SelectNodes("//table").ToList();

            foreach (var table in tables)
            {
                var trs = table.Descendants("tr")
                .Where(tr => tr.Elements("td").Count() > 0)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).Where(td => td.Length > 0).ToList())
                .ToList();
                foreach (var tds in trs)
                {
                    if (tds.Count < 2)  { 
                        items.Add(new JiraIssueItem() { metricid = "", metrictext = tds[0], control = "STTL" });
                    }  else {
                        items.Add(new JiraIssueItem() { metricid = tds[0], metrictext = tds[1], control = tds[2] });
                    }
                    
                }
            } 
            return items; 

        }
        public void OnPost()
        {
            ProcessForm();
        }
        public void OnGet()
        {

            ProcessForm();
        } 

    }
} 
