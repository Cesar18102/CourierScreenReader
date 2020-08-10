using System;
using System.Collections.Generic;

namespace Server.Models.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<string> messages) : 
            base(string.Join(";\n", messages)) { }

        public ValidationException(params string[] messages) :
            this(messages as IEnumerable<string>) { }
    }
}