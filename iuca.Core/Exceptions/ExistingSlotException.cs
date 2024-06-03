using System;
namespace iuca.Application.Exceptions
{
    public class ExistingSlotException : ModelValidationException
    {
        public ExistingSlotException(string message, string prop) : base(message, prop)
        {
        }
    }
}

