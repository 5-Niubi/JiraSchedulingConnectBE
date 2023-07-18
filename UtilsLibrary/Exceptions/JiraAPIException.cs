namespace UtilsLibrary.Exceptions
{
    public class JiraAPIException : Exception
    {
        public dynamic? jiraResponse;
        public JiraAPIException()
        {

        }

        public JiraAPIException(string message)
        : base(message)
        {

        }

        public JiraAPIException(dynamic jiraResponse, string message)
        : base(message)
        {
            this.jiraResponse = jiraResponse;
        }
    }
}
