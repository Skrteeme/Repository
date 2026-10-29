namespace EventApi.Contracts;

/// <summary>
/// Результат постраничной выборки.
/// </summary>
/// <typeparam name="T">Тип элементов страницы.</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Общее количество элементов, удовлетворяющих фильтру (без учёта пагинации).
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Номер текущей страницы.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Количество элементов на текущей странице.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Элементы текущей страницы.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = [];
}
