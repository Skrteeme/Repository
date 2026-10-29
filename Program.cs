using EventApi.Services;

var builder = WebApplication.CreateBuilder(args);

// EventService хранит данные в памяти приложения, поэтому используется Singleton:
// один экземпляр на всё время жизни приложения
builder.Services.AddSingleton<IEventService, EventService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();