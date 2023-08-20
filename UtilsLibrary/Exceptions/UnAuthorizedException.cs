namespace UtilsLibrary.Exceptions
{
    public class UnAuthorizedException : Exception
    {
        public dynamic? Errors;
        public UnAuthorizedException()
        {

        }

        public UnAuthorizedException(dynamic Errors)
        {
            this.Errors = Errors;
        }

        public UnAuthorizedException(string message)
        : base(message)
        {

        }
    }
}
