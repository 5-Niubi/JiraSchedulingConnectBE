using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLibrary.Exceptions
{
    public class MicroServiceAPIException : Exception
    {
        public string? mircoserviceResponse;
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
    }
}
