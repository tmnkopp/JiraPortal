using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Text.RegularExpressions; 
using JiraCore; 
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 

namespace JiraPortal.Pages
{
    public class DiffModel : PageModel
    {
        public string Sections  { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string Top { get; set; }

        public readonly IConfiguration configuration;
        public readonly ILoggerFactory logger;
        public DiffModel(IConfiguration configuration, ILoggerFactory logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public void OnGet()
        {
            string source;
            using (TextReader tr = System.IO.File.OpenText(@"C:\_som\_src\_compile\IG\DB_Update7.34_IG_2021.sql"))
                source = tr.ReadToEnd();
  
            var issues = new JiraIssueProvider(issue => Regex.IsMatch(issue.epic, ".*FY21.*")).Items; 
            foreach (var issue in issues)
            {
                foreach (var item in issue.issueitems)
                {
                    if (!source.Hash().Contains(item.hash))
                    {
                        Top += $"{issue.title}\n\n{item.metrictext.StripHTML() }\n\n\n";
                    }
                }    
            } 
        } 
    }
}
