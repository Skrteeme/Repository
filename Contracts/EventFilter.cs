namespace EventApi.Contracts;

/// <summary>
/// Параметры фильтрации и пагинации списка событий.
/// </summary>
public class EventFilter
{
    /// <summary>
    /// Номер страницы по умолчанию.
    /// </summary>
    public const int DefaultPage = 1;

    /// <summary>
    /// Размер страницы по умолчанию.
    /// </summary>
    public const int DefaultPageSize = 10;

    /// <summary>
    /// Максимально допустимый размер страницы.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Поиск по названию: регистронезависимое частичное совпадение.
    /// </summary>
    /// <example>конференция</example>
    public string? Title { get; set; }

    /// <summary>
    /// События, которые начинаются не раньше указанной даты.
    /// </summary>
    /// <example>2026-01-01T00:00:00Z</example>
    public DateTime? From { get; set; }

    /// <summary>
    /// События, которые заканчиваются не позже указанной даты.
    /// </summary>
    /// <example>2026-12-31T23:59:59Z</example>
    public DateTime? To { get; set; }

    /// <summary>
    /// Номер возвращаемой страницы (начиная с 1).
    /// </summary>
    /// <example>1</example>
    public int Page { get; set; } = DefaultPage;

    /// <summary>
    /// Количество элементов на странице (от 1 до 100).
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; } = DefaultPageSize;
}
