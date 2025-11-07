# Примеры

Здесь собраны примеры использования библиотеки Interesting Algorithms.

## Простая симуляция очереди

Пример создания простой системы массового обслуживания с одним источником, очередью и сервисом:

```csharp
using DeltaT.Algorithm.Models.Simulations;
using DeltaT.Algorithm.Models.Nodes;
using Structures.Models.Abstracts.Options;
using Shared.Mathematics.Models.Distributions.Basic;

// Настройка узлов
var sourceOptions = new SourceOptions
{
    Distribution = new Exponential(rate: 2.0), // 2 запроса в секунду
    ClosedSystem = false
};

var queueOptions = new QueueOptions
{
    Capacity = 100
};

var serviceOptions = new ServiceOptions
{
    Distribution = new Exponential(rate: 1.0) // 1 запрос в секунду
};

var sinkOptions = new SinkOptions();

// Создание узлов
var source = new Source(sourceOptions);
var queue = new Queue(queueOptions);
var service = new Service(serviceOptions);
var sink = new Sink(sinkOptions);

// Связывание узлов
source.SetExits(new[] { queue });
queue.SetExits(new[] { service });
service.SetExits(new[] { sink });

// Создание и запуск симуляции
var simulation = new SequentialSimulation
{
    DeltaTime = 0.1,
    Nodes = new List<INode> { source, queue, service, sink }
};

simulation.Simulate();

// Анализ результатов
var collector = simulation.Collector;
var avgQueueSize = collector.GetGaugeAverage("Queue_Size");
var avgServiceTime = collector.GetList("Service_Duration").Average();
var totalProcessed = collector.GetCounter("Service_Completed");

Console.WriteLine($"Average queue size: {avgQueueSize:F2}");
Console.WriteLine($"Average service time: {avgServiceTime:F2}");
Console.WriteLine($"Total processed: {totalProcessed}");
```

## Симуляция с несколькими сервисами

Пример системы с несколькими параллельными сервисами:

```csharp
// Создание маршрутизатора
var routerOptions = new EmptyOptions { Exits = 3 };
var router = new Empty(routerOptions);

// Создание нескольких сервисов
var service1 = new Service(new ServiceOptions 
{ 
    Distribution = new Exponential(rate: 1.0) 
});
var service2 = new Service(new ServiceOptions 
{ 
    Distribution = new Exponential(rate: 1.0) 
});
var service3 = new Service(new ServiceOptions 
{ 
    Distribution = new Exponential(rate: 1.0) 
});

var sink = new Sink(new SinkOptions());

// Связывание
source.SetExits(new[] { queue });
queue.SetExits(new[] { router });
router.SetExits(new[] { service1, service2, service3 });
service1.SetExits(new[] { sink });
service2.SetExits(new[] { sink });
service3.SetExits(new[] { sink });
```

## Симуляция с нормальным распределением

Пример использования нормального распределения для времени обслуживания:

```csharp
var serviceOptions = new ServiceOptions
{
    Distribution = new Normal(mean: 5.0, stdDev: 1.0)
};

var service = new Service(serviceOptions);
```

## Закрытая система

Пример создания закрытой системы с фиксированной популяцией:

```csharp
var sourceOptions = new SourceOptions
{
    Distribution = new Exponential(rate: 2.0),
    ClosedSystem = true,
    ClosedPopulation = 10
};

var source = new Source(sourceOptions);
```

## Сбор пользовательских метрик

Пример добавления пользовательских метрик в узел:

```csharp
public class CustomService : ServiceBase
{
    public override void Update(double deltaTime)
    {
        base.Update(deltaTime);
        
        // Пользовательские метрики
        if (IsBusy)
        {
            Context.Collector.GaugeRecord("CustomService_Processing", 1);
        }
        else
        {
            Context.Collector.GaugeRecord("CustomService_Processing", 0);
        }
    }
}
```

## Использование различных распределений

### Экспоненциальное распределение

```csharp
var dist = new Exponential(rate: 2.0);
```

### Нормальное распределение

```csharp
var dist = new Normal(mean: 5.0, stdDev: 1.0);
```

### Равномерное распределение

```csharp
var dist = new Uniform(min: 1.0, max: 10.0);
```

### Гамма-распределение

```csharp
var dist = new Gamma(shape: 2.0, scale: 1.0);
```

### Распределение Вейбулла

```csharp
var dist = new Weibull(shape: 2.0, scale: 1.0);
```

## Экспорт метрик

Пример экспорта метрик в файл:

```csharp
var collector = new MetricCollector();
collector.AddSink(new FileSink("metrics.json"));
collector.AddSink(new ConsoleSink());

// После симуляции
collector.Collect();
```

## Полные примеры

Полные рабочие примеры находятся в проектах:

- `Examples/DeltaT.Example/Program.cs` - примеры использования DeltaT
- `Examples/EventDriven.Example/Program.cs` - примеры событийно-ориентированного моделирования

Запустите эти проекты для просмотра работающих примеров.
