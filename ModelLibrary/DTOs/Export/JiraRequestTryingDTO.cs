using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraRequestTryingDTO
    {
        public string? Url { get; set; }
        public string? Method { get; set; }
        public dynamic? Body { get; set; }
    }
}
