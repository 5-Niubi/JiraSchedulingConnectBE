using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Thread
{
    public class ThreadStartDTO
    {
        public string? ThreadId { get; set; }
        public string? ThreadName { get; set; }

        public ThreadStartDTO(string threadId)
        {
            ThreadId = threadId;
        }
    }
}
