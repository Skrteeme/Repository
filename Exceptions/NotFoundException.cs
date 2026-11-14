namespace EventApi.Exceptions;

/// <summary>
/// Исключение, сигнализирующее об отсутствии запрошенного ресурса.
/// Перехватывается middleware и преобразуется в ответ <c>404 Not Found</c>.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
