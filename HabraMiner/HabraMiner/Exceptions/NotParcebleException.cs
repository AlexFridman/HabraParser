using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HabraMiner.Exceptions
{
    class NotParcebleException : Exception
    {
        public NotParcebleException()
        {
        }

        public NotParcebleException(string message) : base(message)
        {
        }

        public NotParcebleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotParcebleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
