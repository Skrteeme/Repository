using EventApi.Contracts;

namespace EventApi.Services;

/// <summary>
/// Сервис для работы с событиями
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Возвращает список всех событий
    /// </summary>
    IReadOnlyList<EventResponse> GetAll();

    /// <summary>
    /// Возвращает событие по идентификатору
    /// </summary>
    EventResponse? GetById(Guid id);

    /// <summary>
    /// Создаёт новое событие
    /// </summary>
    EventResponse Create(EventRequest request);

    /// <summary>
    /// Обновляет событие целиком
    /// </summary>
    EventResponse? Update(Guid id, EventRequest request);

    /// <summary>
    /// Удаляет событие. Возвращает true, если событие было найдено и удалено
    /// </summary>
    bool Delete(Guid id);
}