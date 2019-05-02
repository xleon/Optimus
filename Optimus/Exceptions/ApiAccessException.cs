using System;

namespace Optimus.Exceptions
{
    public class ApiAccessException : Exception
    {
        internal ApiAccessException(string message) : base(message)
        {
            
        }
    }
}