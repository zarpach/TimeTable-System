using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Exceptions
{
    public class ModelValidationException : Exception
    {
        public string Property { get; protected set; }
        public ModelValidationException(string message, string prop) : base(message)
        {
            Property = prop;
        }
    }
}
