var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitSection = configuration.GetSection("RabbitMq");

        cfg.Host(
            rabbitSection["Host"],
            ushort.Parse(rabbitSection["Port"]!),
            rabbitSection["VirtualHost"],
            h =>
            {
                h.Username(rabbitSection["Username"]!);
                h.Password(rabbitSection["Password"]!);
            });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/v1/orders", async (
    IBus bus
) =>
{
    await bus.Publish(new OrderCreatedEvent(Guid.NewGuid()));

    return Results.Created();
})
.WithOpenApi();

await app.RunAsync();