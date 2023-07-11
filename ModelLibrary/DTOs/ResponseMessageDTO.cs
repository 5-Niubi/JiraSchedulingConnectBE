namespace ModelLibrary.DTOs
{
    public class ResponseMessageDTO
    {
        private string message;
        private dynamic? data;
        public ResponseMessageDTO(string message)
        {
            this.message = message;
        }
        public ResponseMessageDTO(string message, dynamic data)
        {
            this.message = message;
            this.data = data;
        }
        public string Message { get { return message; } set { message = value; } }
        public dynamic? Data { get { return data; } set { data = value; } }
    }
}