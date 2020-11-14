using System;
using System.Collections.Generic;
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
    public class IndexModel : PageModel
    {
        public string Sections  { get; set; }
        public string NewPicklists { get; set; }
        public string NewMetrics { get; set; }

        [BindProperty(SupportsGet=true) ]
        public string Action { get; set; }
         
        public StringBuilder sbSections = new StringBuilder();
        public StringBuilder sbNewMetrics = new StringBuilder();
        public StringBuilder sbNewPicklists = new StringBuilder();
        public void OnGet()
        {

            JiraFormatter.Program.InvokeProcessor(); 

            string src = "C:\\temp\\jira\\!CIO2020Q4$DataCallProcessor\\_dest";
            string dest = "C:\\temp\\jira\\!CIO2021Q1$DataCallProcessor\\_dest";
             
            List<DataCallIssue> I19 = IssueProvider.Load(src).OrderBy(x => x.Title).ToList();
            List<DataCallIssue> I20 = IssueProvider.Load(dest).OrderBy(x => x.Title).ToList();

            for (int i = 0; i < I19.Count; i++)
            {
                sbSections.AppendFormat("\n{0}: {1}", I19[i].SectionNo, I20[i].Title); 
                Sections = sbSections.ToString(); 
            }
        } 
    }
}
