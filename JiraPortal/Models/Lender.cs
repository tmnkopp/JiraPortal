using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JiraPortal.Models
{
 
    public class DataCallTicket
    {
        public string Key { get; set; }
        public string Section { get; set; }
        private List<DataCallMetric> metrics = new List<DataCallMetric>();
        public List<DataCallMetric> Metrics
        {
            get { return metrics; }
            set { metrics = value; }
        }
    }
    public class DataCallMetric
    {
        [Key]
        public string IDText { get; set; }
        public string MetricText { get; set; }
    }
}