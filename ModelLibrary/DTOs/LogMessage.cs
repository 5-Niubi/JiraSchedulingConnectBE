using System;
namespace ModelLibrary.DTOs
{
	public class LogMessage
	{
       
        public int ThreadId { get; set; }
        public string LogLevel { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionType { get; set; }
        public string CloudId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        
    }
}

