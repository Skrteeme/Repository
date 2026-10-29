using EventApi.Contracts;
using EventApi.Exceptions;
using EventApi.Services;
using Xunit;

namespace EventApi.Tests;

/// <summary>
/// Тесты CRUD-операций и валидации <see cref="EventService"/>.
/// </summary>
public class EventServiceTests
{
    private static DateTime Base => EventServiceTestData.BaseDate;

    private readonly EventService _service = new();

    [Fact]
    public void Create_ValidRequest_ReturnsEventWithGeneratedId()
    {
        // Arrange
        var request = EventServiceTestData.Request("Конференция", Base, Base.AddHours(8));

        // Act
        var created = _service.Create(request);

        // Assert
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Конференция", created.Title);
        Assert.Equal(request.StartAt, created.StartAt);
        Assert.Equal(request.EndAt, created.EndAt);
    }

    [Fact]
    public void Create_ValidRequest_SavesEventToStorage()
    {
        // Arrange
        var request = EventServiceTestData.Request("Конференция", Base, Base.AddHours(8));

        // Act
        var created = _service.Create(request);
        var stored = _service.GetById(created.Id);

        // Assert
        Assert.NotNull(stored);
        Assert.Equal(created.Id, stored!.Id);
        Assert.Equal(created.Title, stored.Title);
    }

    [Fact]
    public void Create_TitleWithSpaces_IsTrimmed()
    {
        // Arrange
        var request = EventServiceTestData.Request("  Конференция  ", Base, Base.AddHours(8));

        // Act
        var created = _service.Create(request);

        // Assert
        Assert.Equal("Конференция", created.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyTitle_ThrowsValidationException(string title)
    {
        // Arrange
        var request = EventServiceTestData.Request(title, Base, Base.AddHours(8));

        // Act & Assert
        Assert.Throws<ValidationException>(() => _service.Create(request));
    }

    [Fact]
    public void Create_EndAtBeforeStartAt_ThrowsValidationException()
    {
        // Arrange
        var request = EventServiceTestData.Request("Конференция", Base, Base.AddHours(-1));

        // Act & Assert
        Assert.Throws<ValidationException>(() => _service.Create(request));
    }

    [Fact]
    public void Create_EndAtEqualToStartAt_ThrowsValidationException()
    {
        // Arrange
        var request = EventServiceTestData.Request("Конференция", Base, Base);

        // Act & Assert
        Assert.Throws<ValidationException>(() => _service.Create(request));
    }

    [Fact]
    public void GetAll_WithoutFilters_ReturnsAllEvents()
    {
        // Arrange
        EventServiceTestData.CreateEvent(_service, "Первое", Base, Base.AddHours(1));
        EventServiceTestData.CreateEvent(_service, "Второе", Base.AddDays(1), Base.AddDays(1).AddHours(1));

        // Act
        var result = _service.GetAll(new EventFilter());

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public void GetAll_EmptyStorage_ReturnsEmptyPage()
    {
        // Act
        var result = _service.GetAll(new EventFilter());

        // Assert
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsEvent()
    {
        // Arrange
        var created = EventServiceTestData.CreateEvent(_service, "Конференция", Base, Base.AddHours(8));

        // Act
        var found = _service.GetById(created.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
        Assert.Equal(created.Title, found.Title);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        // Act
        var found = _service.GetById(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void Update_ExistingId_ReplacesEventData()
    {
        // Arrange
        var created = EventServiceTestData.CreateEvent(_service, "Старое название", Base, Base.AddHours(1));
        var request = EventServiceTestData.Request("Новое название", Base.AddDays(2), Base.AddDays(2).AddHours(3));

        // Act
        var updated = _service.Update(created.Id, request);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal("Новое название", updated.Title);
        Assert.Equal(request.StartAt, updated.StartAt);
        Assert.Equal(request.EndAt, updated.EndAt);
    }

    [Fact]
    public void Update_UnknownId_ReturnsNull()
    {
        // Arrange
        var request = EventServiceTestData.Request("Новое название", Base, Base.AddHours(1));

        // Act
        var updated = _service.Update(Guid.NewGuid(), request);

        // Assert
        Assert.Null(updated);
    }

    [Fact]
    public void Update_EndAtBeforeStartAt_ThrowsValidationException()
    {
        // Arrange
        var created = EventServiceTestData.CreateEvent(_service, "Конференция", Base, Base.AddHours(1));
        var request = EventServiceTestData.Request("Конференция", Base.AddHours(5), Base.AddHours(2));

        // Act & Assert
        Assert.Throws<ValidationException>(() => _service.Update(created.Id, request));
    }

    [Fact]
    public void Delete_ExistingId_RemovesEvent()
    {
        // Arrange
        var created = EventServiceTestData.CreateEvent(_service, "Конференция", Base, Base.AddHours(1));

        // Act
        var deleted = _service.Delete(created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(_service.GetById(created.Id));
        Assert.Equal(0, _service.GetAll(new EventFilter()).TotalCount);
    }

    [Fact]
    public void Delete_UnknownId_ReturnsFalse()
    {
        // Act
        var deleted = _service.Delete(Guid.NewGuid());

        // Assert
        Assert.False(deleted);
    }
}
