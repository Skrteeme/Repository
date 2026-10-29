using EventApi.Contracts;
using EventApi.Services;
using Xunit;

namespace EventApi.Tests;

/// <summary>
/// Тесты пагинации списка событий.
/// </summary>
public class EventServicePaginationTests
{
    private static DateTime Base => EventServiceTestData.BaseDate;

    private readonly EventService _service = new();

    private void SeedEvents(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var startAt = Base.AddDays(i);
            EventServiceTestData.CreateEvent(_service, $"Событие {i}", startAt, startAt.AddHours(1));
        }
    }

    [Fact]
    public void GetAll_DefaultPagination_ReturnsFirstPageWithTenItems()
    {
        // Arrange
        SeedEvents(12);

        // Act
        var result = _service.GetAll(new EventFilter());

        // Assert
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal("Событие 0", result.Items[0].Title);
    }

    [Fact]
    public void GetAll_SecondPage_ReturnsRemainingItems()
    {
        // Arrange
        SeedEvents(12);

        // Act
        var result = _service.GetAll(new EventFilter { Page = 2 });

        // Assert
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal("Событие 10", result.Items[0].Title);
        Assert.Equal("Событие 11", result.Items[1].Title);
    }

    [Fact]
    public void GetAll_PageOutOfRange_ReturnsEmptyItemsButKeepsTotalCount()
    {
        // Arrange
        SeedEvents(3);

        // Act
        var result = _service.GetAll(new EventFilter { Page = 10 });

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(10, result.Page);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.PageSize);
    }

    [Fact]
    public void GetAll_PageLessThanOne_IsClampedToFirstPage()
    {
        // Arrange
        SeedEvents(3);

        // Act
        var result = _service.GetAll(new EventFilter { Page = 0 });

        // Assert
        Assert.Equal(1, result.Page);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public void GetAll_PageSizeGreaterThanMax_IsClampedToMaxPageSize()
    {
        // Arrange
        SeedEvents(EventFilter.MaxPageSize + 5);

        // Act
        var result = _service.GetAll(new EventFilter { PageSize = 1000 });

        // Assert
        Assert.Equal(EventFilter.MaxPageSize, result.Items.Count);
        Assert.Equal(EventFilter.MaxPageSize + 5, result.TotalCount);
    }

    [Fact]
    public void GetAll_PageSizeLessThanOne_IsClampedToSingleItem()
    {
        // Arrange
        SeedEvents(3);

        // Act
        var result = _service.GetAll(new EventFilter { PageSize = 0 });

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.PageSize);
    }

    [Fact]
    public void GetAll_FilterAndPagination_AppliesFilterBeforePaging()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            var startAt = Base.AddDays(i);
            EventServiceTestData.CreateEvent(_service, $"Встреча {i}", startAt, startAt.AddHours(1));
            EventServiceTestData.CreateEvent(_service, $"Семинар {i}", startAt, startAt.AddHours(1));
        }

        // Act
        var result = _service.GetAll(new EventFilter { Title = "встреча", Page = 2, PageSize = 2 });

        // Assert
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.StartsWith("Встреча", item.Title));
    }
}
