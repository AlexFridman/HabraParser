using System;
using System.Runtime.Serialization;

namespace HabraMiner.Exceptions
{
    class NotParcebleArticleException : Exception
    {
        public NotParcebleArticleException()
        {
        }

        public NotParcebleArticleException(string message) : base(message)
        {
        }

        public NotParcebleArticleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotParcebleArticleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
