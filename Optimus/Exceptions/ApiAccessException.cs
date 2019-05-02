using System;

namespace Optimus.Exceptions
{
    public class ApiAccessException : Exception
    {
        public ApiAccessException(string message) : base(message)
        {
            
        }
    }
}