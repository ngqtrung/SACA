using System;
namespace SACA_Common.Exceptions
{
    public class BadException : Exception
    {
        public BadException()
        {
        }

        public BadException(string message) : base(message)
        {
        }

        public BadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

