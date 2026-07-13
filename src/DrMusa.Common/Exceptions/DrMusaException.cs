namespace DrMusa.Common.Exceptions;

public class DrMusaException : Exception
{
    public DrMusaException(string message) : base(message) { }
    public DrMusaException(string message, Exception innerException) : base(message, innerException) { }
}
