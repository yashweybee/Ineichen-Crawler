using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineichen_Crawler.View_Models
{
    public class VMDate
    {
        public int StartDate { get; set; }
        public int EndDate { get; set; }
        public string? StartMonth { get; set; }
        public string? EndMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }

    }
}
