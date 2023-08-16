using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLibrary.Exceptions
{
    public class ModelInvalidException : Exception
    {
        public ModelInvalidException()
        {

        }

        public ModelInvalidException(string message)
        : base(message)
        {

        }
    }
}
