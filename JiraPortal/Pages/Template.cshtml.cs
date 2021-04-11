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
         
             
        } 
    }
} 
