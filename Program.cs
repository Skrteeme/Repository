using System.Reflection;
using EventApi.Middleware;
using EventApi.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов в DI-контейнере.
// EventService хранит данные в памяти приложения, поэтому используется Singleton:
// один экземпляр на всё время жизни приложения.
builder.Services.AddSingleton<IEventService, EventService>();

// Swagger (OpenAPI). Learn more at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Подключаем XML-комментарии, чтобы описания параметров и ответов отображались в Swagger.
    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddControllers();

// Ошибки валидации модели возвращаются в том же формате Problem Details,
// что и ошибки, обработанные middleware, — ответы единообразны.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ошибка валидации",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

var app = builder.Build();

// Middleware глобальной обработки исключений подключается первым в pipeline,
// чтобы перехватывать исключения всех последующих компонентов.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
