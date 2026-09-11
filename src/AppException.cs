namespace src
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string entity, object id)
            : base($"{entity} з ID '{id}' не знайдено.") { }
    }

    public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>
    /// Used when an operation conflicts with the current state of the system
    /// </summary>
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message) { }
    }
}
