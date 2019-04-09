using System;

namespace LogSystem.Models
{
    class MyLogEventInfo
    {
        public MyLogEventInfo()
        {
        }

        public string ContextId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public DateTime Datetime { get; set; }
    }
}