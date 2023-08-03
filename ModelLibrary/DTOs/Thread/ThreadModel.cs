using UtilsLibrary;

namespace ModelLibrary.DTOs.Thread
{
    public class ThreadModel
    {
        public string? ThreadId
        {
            get; set;
        }
        public string? Status
        {
            get; set;
        }
        public dynamic? Result
        {
            get; set;
        }
        public string? Progress
        {
            get; set;
        }

        public ThreadModel(string? threadId)
        {
            this.Status = Const.THREAD_STATUS.RUNNING;
            this.ThreadId = threadId;
        }
    }
}
