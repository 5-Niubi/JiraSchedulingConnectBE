using System;

namespace UtilsLibrary.Exceptions
{
	public class ParamInputFailureException: Exception
	{

		public dynamic Errors;


        public ParamInputFailureException()
        {

        }

        public ParamInputFailureException(dynamic Errors)

		{
            this.Errors = Errors;


        }
	}
}