using System;
using System.Runtime.Serialization;

namespace SignalrJwtAuthenticationApi.Exceptions
{
    [Serializable]
    internal class EncryptionKeyIsTooShortException : Exception
    {
        public EncryptionKeyIsTooShortException()
        {
        }

        public EncryptionKeyIsTooShortException(string? message) : base(message)
        {
        }

        public EncryptionKeyIsTooShortException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EncryptionKeyIsTooShortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}