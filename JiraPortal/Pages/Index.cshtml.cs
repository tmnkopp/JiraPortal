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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JiraPortal.Pages
{
    public class IndexModel : PageModel
    {
        public readonly IConfiguration configuration;
        public readonly ILoggerFactory logger;
        public IndexModel(IConfiguration configuration, ILoggerFactory logger)
        { 
            this.configuration = configuration;
            this.logger = logger; 
        }
        [BindProperty(SupportsGet=true)]
        public string Action { get; set; } 
        public void OnGet()
        {
            Response.Redirect("/Viewer");  
        } 
    }
}
