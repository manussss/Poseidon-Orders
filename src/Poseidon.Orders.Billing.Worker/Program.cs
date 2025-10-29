var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Worker>();

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

builder.Services.AddHostedService<MassTransitHostedService>();

var host = builder.Build();
host.Run();
