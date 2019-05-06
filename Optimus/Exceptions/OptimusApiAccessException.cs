using System;

namespace Optimus.Exceptions
{
    public class OptimusApiAccessException : Exception
    {
        internal OptimusApiAccessException(string message) : base(message)
        {
            
        }
    }
}