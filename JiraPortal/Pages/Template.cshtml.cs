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
    public class TemplateModel : PageModel
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

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); 
            string src = HttpUtility.HtmlDecode(issue.content);
            doc.LoadHtml(src);
            src = doc.DocumentNode.SelectSingleNode("//description").InnerHtml;
            src = Regex.Replace(src, @"[\^]", "");
            src = Regex.Replace(src, @"[“|”]", "\"");
 
            int qsorter = 0;
            foreach (JiraIssueItem item in ParseIssueItems(src))
            { 
                QPK++;
                var picklistitems = item.control.Split("·").Select(  s => s.Trim().Replace("(Text Box)", "")  ).Skip(1).ToList();
                string metrictext = item.metrictext;
                if (picklistitems.Count > 0)
                {
                    PLT++;
                    PL += 2;
                    int cnt = 1;
                    Footer += string.Format(PicktypeInsert, PLT, $"BODHVA-{PLT}-{item.metricid}", $"BODHVA IDT-{item.metricid}");
                    Left += $"\nEnum Metric_{item.metricid.Replace(".","_")}"; 

                    Footer += "\nSET IDENTITY_INSERT [dbo].[PickLists] ON";
                    Footer += "\nINSERT INTO PickLists(PK_PickList, PK_PickListType, CodeValue,DisplayValue, SortPos, LastUpdated, isActive) VALUES ";

                    foreach (string c in picklistitems)
                    {
                        PL++;
                        PickList pl = new PickList(c);
                        Footer += $"\n({PL},@PKT, N'{pl.CodeValue}','{c.Trim()}',{cnt++}, GETDATE(), 1),";
                        Left += $"\n\t{pl.CodeValue} = {PL} '{c.Trim()}";
                    }
                    Left += $"\nEnd Enum";

                    Footer = Footer.Substring(0, Footer.Length - 1);
                    Footer += $"\nSET IDENTITY_INSERT [dbo].[PickLists] OFF";


                    if (item.control.Contains("Select all"))  {
                        Body += string.Format(MetricInsert, QPK, $"{qsorter++}", $"{item.metricid}", "43", $"{PLT}", $"{metrictext}");
                    } else {
                        Body += string.Format(MetricInsert, QPK, $"{qsorter++}", $"{item.metricid}", "17", $"{PLT}", $"{metrictext}");
                    } 
                } 
                else if (item.control.Contains("Y/N/Unknown")) 
                { 
                    Body += string.Format(MetricInsert, QPK, $"{qsorter++}", $"{item.metricid}", "5", "NULL", $"{metrictext}");
                }
                else if (item.control.Contains("Y/N"))  
                {
                    Body += string.Format(MetricInsert, QPK, $"{qsorter++}", $"{item.metricid}", "1", "NULL", $"{metrictext}"); 
                }
                else if (item.control.Contains("STTL"))
                {
                    Body += string.Format(MetricInsert, QPK, $"{qsorter++}", $"", "63", "NULL", $"{metrictext}");
                }
                else 
                {
                    Body += string.Format(MetricInsert, QPK, $"{qsorter++}", $"{item.metricid}", "9", "NULL", $"{metrictext}");
                }  
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
  

        private static string PicktypeInsert =  @" 
 
SET @PKT={0}
DELETE PickLists WHERE PK_PickListType = @PKT
DELETE PickListTypes WHERE PK_PickListType = @PKT 
INSERT INTO PickListTypes(PK_PickListType, Description, UsageTable, UsageField)
SELECT @PKT, '{1}', NULL, '{2}'
            ";
        // PK, order, idt, qt, plt, qtext
        private static string MetricInsert = @"  
({0}, @FormName, @PK_QGroup, {1}, N'{2}', NULL, NULL, NULL, {3}, 1, {4}, 0, NULL, NULL, NULL, N'{5}',  NULL),";


    }
} 
