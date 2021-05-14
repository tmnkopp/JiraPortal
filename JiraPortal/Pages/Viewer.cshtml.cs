using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Text.RegularExpressions; 
using JiraCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 

namespace JiraPortal.Pages
{
    [BindProperties]
    public class ViewerModel : PageModel
    {
        public string All  { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string View { get; set; }
        public string Title { get; set; } = "Modernization"; 
        public readonly IConfiguration configuration;
        public readonly ILoggerFactory logger;
        public ViewerModel(IConfiguration configuration, ILoggerFactory logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public void OnGet()
        {
            string source;
            using (TextReader tr = System.IO.File.OpenText(@"D:\dev\CyberScope\CyberScopeBranch\CSwebdev\database\DB_Update7.34_BOD_2021.sql"))
                source = tr.ReadToEnd();

             
            var issues = new JiraIssueProvider(i => Regex.IsMatch(i.epic, $".*EINSTEIN.*") ).Items; 
            foreach (var issue in issues)
            {
                All += $"\n{issue.link}";
                string isu = Regex.Match(issue.description.Replace("\n",""), $"ISSUE:(.*)WHERE")?.Groups[1]?.Value ?? issue.description;
                All += $"{ isu}";     
            } 
        } 
    }
}
