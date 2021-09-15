using System;
using System.Runtime.Serialization;

namespace SignalrJwtAuthenticationApi.Exceptions
{
    [Serializable]
    internal class EncryptionKeyNotSetException : Exception
    {
        public EncryptionKeyNotSetException()
        {
        }

        public EncryptionKeyNotSetException(string? message) : base(message)
        {
        }

        public EncryptionKeyNotSetException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EncryptionKeyNotSetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}