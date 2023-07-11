using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
