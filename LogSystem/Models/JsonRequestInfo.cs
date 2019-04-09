using System.IO;
using Microsoft.AspNetCore.Http;

namespace LogSystem.Models
{
    class JsonRequestInfo
    {
        public JsonRequestInfo()
        {
        }

        public string FullRequestPath { get; set; }
        public QueryString QueryString { get; set; }
        public string Body { get; set; }
    }
}