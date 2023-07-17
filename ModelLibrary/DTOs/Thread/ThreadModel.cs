using JiraSchedulingConnectAppService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Thread
{
    public class ThreadModel
    {
        public string? ThreadId { get; set; }
        public string? Status { get; set; }
        public dynamic? Result { get; set; }

        public ThreadModel(string? threadId) {
            this.Status = Const.THREAD_STATUS.RUNNING;
            this.ThreadId = threadId;
        }
    }
}
