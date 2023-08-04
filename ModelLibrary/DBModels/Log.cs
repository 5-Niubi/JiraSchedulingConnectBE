namespace ModelLibrary.DBModels
{
    public partial class Log
    {
        public DateTimeOffset? Timestamp
        {
            get; set;
        }
        public string? Message
        {
            get; set;
        }
        public string? ExceptionSource
        {
            get; set;
        }
        public string? ExceptionType
        {
            get; set;
        }
        public string? LogLevel
        {
            get; set;
        }
        public int? ThreadId
        {
            get; set;
        }
    }
}
