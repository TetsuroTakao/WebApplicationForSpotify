using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore21Utilities
{
    public class LogModel
    {
        public DateTime EventOccours { get; set; }
        public string EventSource { get; set; }
        public string EventMessage { get; set; }
        public ErrorModel Error { get; set; }
        public string Memo { get; set; }
    }
    public class ErrorModel
    {
        public DateTime ErrorOccours { get; set; }
        public string ErrorSource { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum LogType
    {
        Default = 0,
        PassCheck = 1
    }
}
