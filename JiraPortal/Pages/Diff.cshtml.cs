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
        public string All  { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string Diff { get; set; }

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
            using (TextReader tr = System.IO.File.OpenText(@"C:\_som\_src\_compile\BOD\DB_Update7.34_BOD_2021.sql"))
                source = tr.ReadToEnd();

            var prev = new JiraIssueProvider(i => Regex.IsMatch(i.epic, ".*BOD 18-02.*2020.*Modernization.*") && Regex.IsMatch(i.title, ".*Section.*")).Items;
            foreach (var issue in prev)
            {
                //Left += $"\n{issue.section }\n";
                Left += $"\n{issue.description.Hash()}\n";

            }
            var next = new JiraIssueProvider(i => Regex.IsMatch(i.epic, ".*BOD 18-02.*2021.*Modernization.*") && Regex.IsMatch(i.title, ".*Section.*")).Items;
            foreach (var issue in next)
            {
                //Right += $"\n{issue.section}\n";
                Right += $"\n{issue.description.Hash()}\n";
            }

            var issues = new JiraIssueProvider(i => Regex.IsMatch(i.epic, ".*BOD 18-02.*2021.*") &&  Regex.IsMatch(i.title, ".*Section.*")).Items; 
            foreach (var issue in issues)
            {
                All += $"\n\n{issue.section}\n";
                foreach (var item in issue.issueitems)
                {
                    All += $"\n{item.metricid}\n{item.metrictext}\n\n";
                    if (!source.Hash().ToLower().Contains(item.metrictext.Hash().ToLower())) 
                        Diff += $"{issue.title}\n\n{item.metrictext.StripHTML() }\n\n\n";
                  
                }    
            } 
        } 
    }
}
