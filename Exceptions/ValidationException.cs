namespace EventApi.Exceptions;

/// <summary>
/// Исключение нарушения бизнес-правил валидации.
/// Перехватывается middleware и преобразуется в ответ <c>400 Bad Request</c>.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }
}
