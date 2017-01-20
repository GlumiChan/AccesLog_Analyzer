using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class LogEntry
    {
        public static int entryCount = 0;
        public string time { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public string host { get; set; }
        public string useragent { get; set; }

        public LogEntry(string _time, string _request, string _response, string _host, string _useragent)
        {
            entryCount++;
            time = _time;
            request = _request;
            response = _response;
            host = _host;
            useragent = _useragent;
        }
        public bool HasTerm(string s)
        {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                if (propertyInfo.GetValue(this).ToString().ToLower().Contains(s.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
