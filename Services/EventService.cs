using EventApi.Contracts;
using EventApi.Exceptions;
using EventApi.Mapping;
using EventApi.Models;

namespace EventApi.Services;

/// <summary>
/// Реализация сервиса <see cref="IEventService"/> с хранением событий в памяти приложения.
/// </summary>
public class EventService : IEventService
{
    // Данные хранятся в памяти приложения (in-memory storage).
    private readonly List<Event> _events = [];
    private readonly object _lock = new();

    public PaginatedResult<EventResponse> GetAll(EventFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        // Значения пагинации приводятся к допустимому диапазону,
        // чтобы некорректные query-параметры не приводили к ошибке запроса.
        var page = filter.Page < EventFilter.DefaultPage ? EventFilter.DefaultPage : filter.Page;
        var pageSize = Math.Clamp(filter.PageSize, 1, EventFilter.MaxPageSize);

        lock (_lock)
        {
            // Каждый фильтр применяется только если параметр передан,
            // все условия объединяются логическим И.
            var query = _events.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var title = filter.Title.Trim();
                query = query.Where(e => e.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.From is not null)
            {
                var from = filter.From.Value;
                query = query.Where(e => e.StartAt >= from);
            }

            if (filter.To is not null)
            {
                var to = filter.To.Value;
                query = query.Where(e => e.EndAt <= to);
            }

            var totalCount = query.Count();

            // Смещение считается в long, чтобы большие номера страниц не приводили к переполнению int.
            var skip = Math.Min((long)(page - 1) * pageSize, int.MaxValue);

            var items = query
                .OrderBy(e => e.StartAt)
                .ThenBy(e => e.Id)
                .Skip((int)skip)
                .Take(pageSize)
                .Select(EventMapper.ToResponse)
                .ToList();

            return new PaginatedResult<EventResponse>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = items.Count,
                Items = items
            };
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
        Validate(request);

        var entity = EventMapper.ToEntity(request, Guid.NewGuid());

        lock (_lock)
        {
            _events.Add(entity);
            return EventMapper.ToResponse(entity);
        }
    }

    public EventResponse? Update(Guid id, EventRequest request)
    {
        Validate(request);

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

    /// <summary>
    /// Проверяет бизнес-правила события. Нарушение приводит к <see cref="ValidationException"/>,
    /// которая обрабатывается middleware и возвращается клиенту как <c>400 Bad Request</c>.
    /// </summary>
    private static void Validate(EventRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Поле Title обязательно.");
        }

        if (request.StartAt is null || request.EndAt is null)
        {
            throw new ValidationException("Поля StartAt и EndAt обязательны.");
        }

        if (request.EndAt.Value <= request.StartAt.Value)
        {
            throw new ValidationException("EndAt должен быть позже StartAt.");
        }
    }
}
