namespace DrMusa.Common.Exceptions;

public class ValidationException : DrMusaException
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToList().AsReadOnly();
    }

    public ValidationException(string error) : this(new[] { error }) { }
}
