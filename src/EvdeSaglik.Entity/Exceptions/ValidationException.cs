namespace EvdeSaglik.Entity.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors) : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }
}
