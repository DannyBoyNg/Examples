using System.Runtime.Serialization;

namespace ExampleWebApi.SharedKernel.Modules.Jwt
{
    [Serializable]
    internal class JwtSecretKeyNotSetException : Exception
    {
        public JwtSecretKeyNotSetException()
        {
        }

        public JwtSecretKeyNotSetException(string? message) : base(message)
        {
        }

        public JwtSecretKeyNotSetException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected JwtSecretKeyNotSetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}