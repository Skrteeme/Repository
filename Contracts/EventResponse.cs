namespace EventApi.Contracts;

/// <summary>
/// DTO для возврата данных о событии
/// </summary>
public class EventResponse
{
    /// <summary>
    /// Уникальный идентификатор события
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название события
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание события
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Дата и время начала события
    /// </summary>
    public DateTime StartAt { get; set; }

    /// <summary>
    /// Дата и время окончания события
    /// </summary>
    public DateTime EndAt { get; set; }
}