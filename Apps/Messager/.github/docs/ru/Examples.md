## Examples

### Регистрация в Autofac

```csharp
using Autofac;
using Microsoft.Extensions.Logging;
using Messager.Extensions;

var builder = new ContainerBuilder();

// Включаем систему сообщений (можно указать уровень логирования и др. опции)
builder.AddEventSystem(opts => { /* opts.LogLevel = LogLevel.Debug; */ });

// Регистрируем ваши сервисы как обычно
builder.RegisterType<MyService>().SingleInstance();

var container = builder.Build();
```

### Базовый pub/sub (без ключа)

```csharp
public sealed record UserCreated(string UserId);

public sealed class Consumer
{
    private readonly IDisposable _subscription;

    public Consumer(IReceiver<UserCreated> receiver, ILogger<Consumer> logger)
    {
        _subscription = receiver.Subscribe(evt =>
        {
            logger.LogInformation("Handled UserCreated for {UserId}", evt.UserId);
        });
    }
}

public sealed class Producer
{
    private readonly ISender<UserCreated> _sender;

    public Producer(ISender<UserCreated> sender)
    {
        _sender = sender;
    }

    public void Create(string userId)
    {
        // ... доменная логика ...
        _sender.Send(new UserCreated(userId));
    }
}
```

### Pub/sub с ключом

```csharp
public sealed record OrderUpdated(string OrderId, decimal Sum);

// Подписчик на обновления по конкретному магазину (ключ — ShopId)
public sealed class ShopOrderConsumer
{
    private readonly IDisposable _subscription;

    public ShopOrderConsumer(IReceiver<string, OrderUpdated> receiver, string shopId, ILogger<ShopOrderConsumer> logger)
    {
        _subscription = receiver.Subscribe(shopId, evt =>
        {
            logger.LogInformation("[{Shop}] Order {OrderId} updated: {Sum}", shopId, evt.OrderId, evt.Sum);
        });
    }
}

public sealed class ShopOrderProducer
{
    private readonly ISender<string, OrderUpdated> _sender;

    public ShopOrderProducer(ISender<string, OrderUpdated> sender)
    {
        _sender = sender;
    }

    public void Update(string shopId, string orderId, decimal sum)
    {
        _sender.Send(shopId, new OrderUpdated(orderId, sum));
    }
}
```

### Асинхронный вариант

```csharp
public sealed record FileImported(string Path);

public sealed class AsyncConsumer
{
    private readonly IAsyncDisposable _subscription;

    public AsyncConsumer(IAsyncReceiver<FileImported> receiver, ILogger<AsyncConsumer> logger)
    {
        _subscription = receiver.Subscribe(async evt =>
        {
            await Task.Delay(10);
            logger.LogInformation("Imported: {Path}", evt.Path);
        });
    }
}

public sealed class AsyncProducer
{
    private readonly IAsyncSender<FileImported> _sender;

    public AsyncProducer(IAsyncSender<FileImported> sender)
    {
        _sender = sender;
    }

    public async Task ImportAsync(string path)
    {
        await _sender.SendAsync(new FileImported(path));
    }
}
```