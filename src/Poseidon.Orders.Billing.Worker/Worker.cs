namespace Poseidon.Orders.Billing.Worker;

public class Worker(ILogger<Worker> logger) : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        logger.LogInformation("📦 Order received: {OrderId}", context.Message.Id);

        return Task.CompletedTask;
    }
}
