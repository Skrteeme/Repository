using EventApi.Contracts;
using EventApi.Models;

namespace EventApi.Mapping;

/// <summary>
/// Маппинг между моделью <see cref="Event"/> и DTO
/// </summary>
public static class EventMapper
{
    public static Event ToEntity(EventRequest request, Guid id)
    {
        return new Event
        {
            Id = id,
            Title = request.Title.Trim(),
            Description = request.Description,
            StartAt = request.StartAt!.Value,
            EndAt = request.EndAt!.Value
        };
    }

    public static EventResponse ToResponse(Event entity)
    {
        return new EventResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            StartAt = entity.StartAt,
            EndAt = entity.EndAt
        };
    }
}