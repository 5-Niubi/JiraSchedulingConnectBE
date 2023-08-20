using System.Net;

namespace UtilsLibrary.Exceptions
{
    public class MicroServiceAPIException : Exception
    {
        public string? mircoserviceResponse;
        public HttpStatusCode? httpCode;
        public MicroServiceAPIException()
        {

        }

        public MicroServiceAPIException(string message)
        : base(message)
        {

        }

        public MicroServiceAPIException(dynamic mircoserviceResponse, string message)
        : base(message)
        {
            this.mircoserviceResponse = mircoserviceResponse;
        }

        public MicroServiceAPIException( dynamic mircoserviceResponse, string message,
            HttpStatusCode? httpCode = HttpStatusCode.InternalServerError)
        : base(message)
        {
            this.httpCode = httpCode;
            this.mircoserviceResponse = mircoserviceResponse;
        }
    }
}
