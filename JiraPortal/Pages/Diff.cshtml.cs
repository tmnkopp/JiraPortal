using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using JiraCore.Data;
using JiraCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            Sections = "";
            string src = "C:\\temp\\jira\\!CIO2020Q4$DataCallProcessor\\_dest";
            string dest = "C:\\temp\\jira\\!CIO2021Q1$DataCallProcessor\\_dest";

            List<DataCallIssue> Prev = IssueProvider.Load(src).OrderBy(x => x.Title).ToList();
            List<DataCallIssue> Next = IssueProvider.Load(dest).OrderBy(x => x.Title).ToList();

            for (int i = 0; i < Prev.Count; i++)
            {
                if (IsDiff(Prev[i].GetDetails,  Next[i].GetDetails))
                {
                    string[] lines = Prev[i].GetDetails.Split("\n");
                    string diff = Next[i].GetDetails;
                    foreach (string line in lines)
                    {
                        diff = diff.Replace(line, "\n"); 
                    }
                    diff = diff.Replace("\n\n", "");
                    sbDiff.Append(Next[i].Title + "\n\n");
                    sbDiff.Append(diff + "\n\n");
                    sbSections.Append(Next[i].Title + "\n");
                    sbSections.Append(Prev[i].Link + "\n");
                    sbSections.Append(Next[i].Link + "\n");

                    sbPrevDescriptions.Append(Prev[i].Title + "\n");
                    sbPrevDescriptions.Append(Prev[i].Link + "\n");
                    sbPrevDescriptions.Append(Prev[i].GetDetails + "\n\n\n");

                    sbNextDescriptions.Append(Next[i].Title + "\n");
                    sbNextDescriptions.Append(Next[i].Link + "\n");
                    sbNextDescriptions.Append(Next[i].GetDetails + "\n\n\n"); 
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
            txtx = txtx.Replace("FY20", "FY21");
            if (txtx != txty) {
                return true;
            }
            return false;
        }
    }
}
