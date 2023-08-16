using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLibrary.Exceptions
{
    public class AlgorithmErrorException : Exception
    {
        public AlgorithmErrorException()
        {

        }

        public AlgorithmErrorException(string message)
        : base(message)
        {

        }
    }
}
