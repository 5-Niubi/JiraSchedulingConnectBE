using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLibrary.Exceptions
{
    public class JiraAPIException : Exception
    {
        public string? jiraResponse;
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
