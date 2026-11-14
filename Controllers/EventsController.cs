using EventApi.Contracts;
using EventApi.Exceptions;
using EventApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventApi.Controllers;

/// <summary>
/// REST API для работы с событиями.
/// </summary>
[ApiController]
[Route("events")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Получить страницу событий с фильтрацией по названию и датам.
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации событий.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] EventFilter filter)
    {
        var result = _eventService.GetAll(filter);
        return Ok(result);
    }

    /// <summary>
    /// Получить событие по идентификатору.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var entity = _eventService.GetById(id);
        if (entity is null)
        {
            throw new NotFoundException($"Событие с id '{id}' не найдено.");
        }

        return Ok(entity);
    }

    /// <summary>
    /// Создать новое событие.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] EventRequest request)
    {
        var entity = _eventService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    /// <summary>
    /// Обновить событие целиком.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] EventRequest request)
    {
        var entity = _eventService.Update(id, request);
        if (entity is null)
        {
            throw new NotFoundException($"Событие с id '{id}' не найдено.");
        }

        return Ok(entity);
    }

    /// <summary>
    /// Удалить событие.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = _eventService.Delete(id);
        if (!deleted)
        {
            throw new NotFoundException($"Событие с id '{id}' не найдено.");
        }

        return NoContent();
    }
}
