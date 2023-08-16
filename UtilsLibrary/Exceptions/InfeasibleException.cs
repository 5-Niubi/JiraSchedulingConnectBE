using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLibrary.Exceptions
{
    public class InfeasibleException : AlgorithmErrorException
    {
        public InfeasibleException()
        {

        }

        public InfeasibleException(string message)
        : base(message)
        {

        }
    }
}
