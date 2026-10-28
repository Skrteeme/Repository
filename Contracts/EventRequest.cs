using System.ComponentModel.DataAnnotations;

namespace EventApi.Contracts;

/// <summary>
/// DTO для создания или обновления события
/// </summary>
public class EventRequest : IValidatableObject
{
    /// <summary>
    /// Название события (обязательное)
    /// </summary>
    [Required(ErrorMessage = "Поле Title обязательно.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание события (опциональное)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Дата и время начала события (обязательное)
    /// </summary>
    [Required(ErrorMessage = "Поле StartAt обязательно.")]
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Дата и время окончания события (обязательное)
    /// </summary>
    [Required(ErrorMessage = "Поле EndAt обязательно.")]
    public DateTime? EndAt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Проверка выполняется только когда оба поля присутствуют (остальные случаи обрабатывает [Required])
        if (EndAt is not null && StartAt is not null && EndAt <= StartAt)
        {
            yield return new ValidationResult(
                "EndAt должен быть позже StartAt.",
                new[] { nameof(EndAt) });
        }
    }
}