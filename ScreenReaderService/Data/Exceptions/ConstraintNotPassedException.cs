using System;

namespace ScreenReaderService.Data.Exceptions
{
    public class ConstraintNotPassedException : Exception
    {
        public string Reason { get; private set; }

        public ConstraintNotPassedException(string reason)
        {
            Reason = reason;
        }
    }
}