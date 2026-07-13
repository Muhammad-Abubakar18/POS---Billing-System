namespace DrMusa.Common.Exceptions;

public class NotFoundException : DrMusaException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}
