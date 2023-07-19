namespace UtilsLibrary.Exceptions
{
    public class NoSuitableWorkerException : Exception
    {
        public NoSuitableWorkerException()
        {

        }

        public NoSuitableWorkerException(string message)
        : base(message)
        {

        }
    }
}
