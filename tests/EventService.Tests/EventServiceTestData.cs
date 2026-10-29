using EventApi.Contracts;
using EventApi.Services;

namespace EventApi.Tests;

/// <summary>
/// Вспомогательные методы для подготовки тестовых данных.
/// </summary>
internal static class EventServiceTestData
{
    /// <summary>
    /// Базовая дата, от которой отсчитываются события в тестах.
    /// </summary>
    public static readonly DateTime BaseDate = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    public static EventRequest Request(string title, DateTime startAt, DateTime endAt) =>
        new()
        {
            Title = title,
            StartAt = startAt,
            EndAt = endAt
        };

    public static EventResponse CreateEvent(EventService service, string title, DateTime startAt, DateTime endAt) =>
        service.Create(Request(title, startAt, endAt));
}
