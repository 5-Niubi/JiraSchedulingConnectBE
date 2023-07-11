using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Thread
{
    public class ThreadModel
    {
        public int ThreadId { get; set; }
        public string Status { get; set; }
        public object Result { get; set; }
    }
}
