using System;

namespace ScreenReaderService.Data.Exceptions
{
    public class NotFoundException : Exception
    {
        public string What { get; private set; }

        public NotFoundException(string what)
        {
            What = what;
        }

        public override string Message => $"{What} not found";
    }
}