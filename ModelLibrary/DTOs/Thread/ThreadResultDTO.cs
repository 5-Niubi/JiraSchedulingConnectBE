namespace ModelLibrary.DTOs.Thread
{
    public class ThreadResultDTO
    {
        public string? ThreadId { get; set; }
        public string? Status { get; set; }
        public dynamic? Result { get; set; }
        public string? Progress { get; set; }
    }
}
