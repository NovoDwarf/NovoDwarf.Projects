# Быстрый старт

Это руководство поможет вам быстро начать работу с библиотекой Interesting Algorithms.

## Установка

### Требования

- .NET 8.0 или выше
- Visual Studio 2022, Rider или другой совместимый IDE

### Добавление проектов в решение

Проекты находятся в следующих директориях:

- `Algorithms/Modeling/DeltaT.Algorithm` - алгоритм DeltaT
- `Shared/Data` - структуры данных
- `Shared/Logging` - система логирования и метрик
- `Shared/Mathematics` - математические распределения
- `Shared/Structures` - структуры для моделирования

## Первая симуляция

### Создание простой симуляции

Создайте новый проект и добавьте ссылки на необходимые библиотеки:

```csharp
using DeltaT.Algorithm.Models.Simulations;
using DeltaT.Algorithm.Models.Nodes;
using Structures.Models.Abstracts.Options;
using Shared.Mathematics.Models.Distributions.Basic;

// Создаем опции для источника
var sourceOptions = new SourceOptions
{
    Distribution = new Exponential(rate: 2.0),
    ClosedSystem = false
};

// Создаем опции для очереди
var queueOptions = new QueueOptions
{
    Capacity = -1 // Неограниченная емкость
};

// Создаем опции для сервиса
var serviceOptions = new ServiceOptions
{
    Distribution = new Exponential(rate: 1.0)
};

// Создаем опции для приемника
var sinkOptions = new SinkOptions();

// Создаем узлы
var source = new Source(sourceOptions);
var queue = new Queue(queueOptions);
var service = new Service(serviceOptions);
var sink = new Sink(sinkOptions);

// Связываем узлы
source.SetExits(new[] { queue });
queue.SetExits(new[] { service });
service.SetExits(new[] { sink });

// Создаем и запускаем симуляцию
var simulation = new SequentialSimulation
{
    DeltaTime = 0.1,
    Nodes = new List<INode> { source, queue, service, sink }
};

simulation.Simulate();
```

## Работа с метриками

После завершения симуляции вы можете получить метрики:

```csharp
var collector = simulation.Collector;

// Получаем счетчики
var generatedCount = collector.GetCounter("Source_Generated");

// Получаем средние значения
var avgQueueSize = collector.GetGaugeAverage("Queue_Size");

// Получаем списки значений
var serviceDurations = collector.GetList("Service_Duration");
var avgDuration = serviceDurations.Average();
```

## Примеры

Полные примеры использования находятся в проектах:

- `Examples/DeltaT.Example` - примеры использования алгоритма DeltaT
- `Examples/EventDriven.Example` - примеры событийно-ориентированного моделирования

## Следующие шаги

- Изучите [руководство по узлам моделирования](nodes.md)
- Узнайте больше о [метриках](metrics.md)
- Посмотрите [примеры использования](examples.md)
