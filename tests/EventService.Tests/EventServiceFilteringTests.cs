using EventApi.Contracts;
using EventApi.Services;
using Xunit;

namespace EventApi.Tests;

/// <summary>
/// Тесты фильтрации событий по названию и датам.
/// </summary>
public class EventServiceFilteringTests
{
    private static DateTime Base => EventServiceTestData.BaseDate;

    private readonly EventService _service = new();

    public EventServiceFilteringTests()
    {
        // Набор событий с разными названиями и датами.
        EventServiceTestData.CreateEvent(_service, "Конференция разработчиков", Base, Base.AddHours(8));
        EventServiceTestData.CreateEvent(_service, "Встреча с командой", Base.AddDays(1), Base.AddDays(1).AddHours(1));
        EventServiceTestData.CreateEvent(_service, "Конференция по безопасности", Base.AddDays(5), Base.AddDays(5).AddHours(4));
        EventServiceTestData.CreateEvent(_service, "Отпуск", Base.AddDays(10), Base.AddDays(20));
    }

    [Theory]
    [InlineData("конференция")]
    [InlineData("КОНФЕРЕНЦИЯ")]
    [InlineData("ференц")]
    public void GetAll_FilterByTitle_ReturnsCaseInsensitivePartialMatches(string title)
    {
        // Act
        var result = _service.GetAll(new EventFilter { Title = title, PageSize = EventFilter.MaxPageSize });

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item => Assert.Contains("конференция", item.Title, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetAll_FilterByTitle_NoMatches_ReturnsEmptyPage()
    {
        // Act
        var result = _service.GetAll(new EventFilter { Title = "семинар" });

        // Assert
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void GetAll_FilterByTitle_WhitespaceOnly_IsIgnored()
    {
        // Act
        var result = _service.GetAll(new EventFilter { Title = "   ", PageSize = EventFilter.MaxPageSize });

        // Assert
        Assert.Equal(4, result.TotalCount);
    }

    [Fact]
    public void GetAll_FilterByFrom_ReturnsEventsStartingAtOrAfterDate()
    {
        // Arrange: граница фильтра совпадает с началом второго события.
        var from = Base.AddDays(1);

        // Act
        var result = _service.GetAll(new EventFilter { From = from, PageSize = EventFilter.MaxPageSize });

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, item => Assert.True(item.StartAt >= from));
    }

    [Fact]
    public void GetAll_FilterByTo_ReturnsEventsEndingAtOrBeforeDate()
    {
        // Arrange: граница фильтра совпадает с окончанием первого события.
        var to = Base.AddHours(8);

        // Act
        var result = _service.GetAll(new EventFilter { To = to, PageSize = EventFilter.MaxPageSize });

        // Assert
        Assert.Single(result.Items);
        Assert.All(result.Items, item => Assert.True(item.EndAt <= to));
    }

    [Fact]
    public void GetAll_FilterByFromAndTo_ReturnsEventsInsideRange()
    {
        // Arrange
        var from = Base.AddDays(1);
        var to = Base.AddDays(5).AddHours(4);

        // Act
        var result = _service.GetAll(new EventFilter { From = from, To = to, PageSize = EventFilter.MaxPageSize });

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item =>
        {
            Assert.True(item.StartAt >= from);
            Assert.True(item.EndAt <= to);
        });
    }

    [Fact]
    public void GetAll_FilterByTitleAndDates_AppliesAllConditionsTogether()
    {
        // Arrange
        var from = Base.AddDays(1);
        var to = Base.AddDays(6);

        // Act
        var result = _service.GetAll(new EventFilter
        {
            Title = "конференция",
            From = from,
            To = to,
            PageSize = EventFilter.MaxPageSize
        });

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Конференция по безопасности", Assert.Single(result.Items).Title);
    }
}
