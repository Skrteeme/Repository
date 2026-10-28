using EventApi.Contracts;
using EventApi.Mapping;
using EventApi.Models;

namespace EventApi.Services;

/// <summary>
/// Реализация сервиса <see cref="IEventService"/> с хранением событий в памяти приложения
/// </summary>
public class EventService : IEventService
{
    // Данные хранятся в памяти приложения (in-memory storage)
    private readonly List<Event> _events = [];
    private readonly object _lock = new();

    public IReadOnlyList<EventResponse> GetAll()
    {
        lock (_lock)
        {
            return _events.Select(EventMapper.ToResponse).ToList();
        }
    }

    public EventResponse? GetById(Guid id)
    {
        lock (_lock)
        {
            var entity = _events.FirstOrDefault(e => e.Id == id);
            return entity is null ? null : EventMapper.ToResponse(entity);
        }
    }

    public EventResponse Create(EventRequest request)
    {
        var entity = EventMapper.ToEntity(request, Guid.NewGuid());

        lock (_lock)
        {
            _events.Add(entity);
            return EventMapper.ToResponse(entity);
        }
    }

    public EventResponse? Update(Guid id, EventRequest request)
    {
        lock (_lock)
        {
            var entity = _events.FirstOrDefault(e => e.Id == id);
            if (entity is null)
            {
                return null;
            }

            entity.Title = request.Title.Trim();
            entity.Description = request.Description;
            entity.StartAt = request.StartAt!.Value;
            entity.EndAt = request.EndAt!.Value;

            return EventMapper.ToResponse(entity);
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            var entity = _events.FirstOrDefault(e => e.Id == id);
            if (entity is null)
            {
                return false;
            }

            _events.Remove(entity);
            return true;
        }
    }
}