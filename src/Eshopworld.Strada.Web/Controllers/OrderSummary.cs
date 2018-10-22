using System;

namespace Eshopworld.Strada.Web.Controllers
{
    public class OrderSummary
    {
        public long MinTimeDelay { get; set; }
        public long AvgTimeDelay { get; set; }
        public long MaxTimeDelay { get; set; }
        public string MinFirstEventName { get; set; }
        public string MinSecondEventName { get; set; }
        public string MaxFirstEventName { get; set; }
        public string MaxSecondEventName { get; set; }
        public string LastEventName { get; set; }
        public bool Complete { get; set; }
        public string OrderNumber { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public float OrderValue { get; set; }
        public long TotalTime { get; set; }
        public string Country { get; set; }
        public int UnitsPerOrder { get; set; }
    }
}