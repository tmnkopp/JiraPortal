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
    public class MetricsModel : PageModel
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

             
            List<DataCallIssue> I19 = IssueProvider.Load(src).OrderBy(x => x.SectionNo).ToList();
            List<DataCallIssue> I20 = IssueProvider.Load(dest).OrderBy(x => x.SectionNo).ToList();

            for (int i = 0; i < I19.Count; i++)
            {
                sbSections.AppendFormat("\n{0}: {1}", I19[i].SectionNo, I20[i].Section);

                for (int j = 0; j < I19[i].Metrics.Count; j++)
                {
                    string I19MetricText = I19[i].Metrics[j].MetricText;
                    string I20MetricText = I20[i].Metrics[j].MetricText;
                    string I19MetricPicklist = I19[i].Metrics[j].MetricPicklist;
                    string I20MetricPicklist = I20[i].Metrics[j].MetricPicklist;
                    if (I20MetricPicklist != "")
                    {
                        if (I19MetricPicklist != I20MetricPicklist)
                        {
                            sbNewPicklists.AppendFormat("\n{0}", I20[i].Metrics[j].IDText);
                            sbNewPicklists.AppendFormat("\n{0}\n{1}", I19MetricPicklist, I20MetricPicklist);
                        }
                    }

                    if (I19MetricText != I20MetricText)
                    {
                        sbNewMetrics.AppendFormat("\nI19 - {0}: {1}", I19[i].Metrics[j].IDText, I19[i].Metrics[j].MetricText);
                        sbNewMetrics.AppendFormat("\nI20 - {0}: {1}", I20[i].Metrics[j].IDText, I20[i].Metrics[j].MetricText);

                    }
                }
                Sections = sbSections.ToString();
                NewPicklists = sbNewPicklists.ToString();
                NewMetrics = sbNewMetrics.ToString();
            }
        } 
    }
}
