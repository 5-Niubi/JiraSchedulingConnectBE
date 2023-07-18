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
