namespace UtilsLibrary.Exceptions
{
    public class NotSuitableInputException : Exception
    {

        public dynamic Errors;

        public NotSuitableInputException()
        {

        }

        public NotSuitableInputException(dynamic Errors)
        {
            this.Errors = Errors;
        }
    }
}

