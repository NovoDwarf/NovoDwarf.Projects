# Узлы моделирования

Узлы - это основные компоненты системы моделирования. Каждый узел выполняет определенную функцию в процессе обработки запросов.

## Типы узлов

### Source (Источник)

Генерирует новые запросы в системе согласно заданному распределению.

#### Основные параметры

- `Distribution` - распределение времени между генерациями
- `ClosedSystem` - режим закрытой системы
- `ClosedPopulation` - размер популяции для закрытой системы

#### Пример

```csharp
var sourceOptions = new SourceOptions
{
    Distribution = new Exponential(rate: 2.0),
    ClosedSystem = false
};

var source = new Source(sourceOptions);
```

### Queue (Очередь)

Хранит запросы в очереди перед их обработкой.

#### Основные параметры

- `Capacity` - максимальная емкость очереди (-1 для неограниченной)

#### Пример

```csharp
var queueOptions = new QueueOptions
{
    Capacity = 100
};

var queue = new Queue(queueOptions);
```

### Service (Сервис)

Обрабатывает запросы с заданным временем обслуживания.

#### Основные параметры

- `Distribution` - распределение времени обслуживания

#### Пример

```csharp
var serviceOptions = new ServiceOptions
{
    Distribution = new Normal(mean: 5.0, stdDev: 1.0)
};

var service = new Service(serviceOptions);
```

### Sink (Приемник)

Финальный узел, принимающий обработанные запросы.

#### Пример

```csharp
var sinkOptions = new SinkOptions();
var sink = new Sink(sinkOptions);
```

### Empty (Пустой узел)

Узел-маршрутизатор, который сразу передает запросы следующим узлам.

#### Основные параметры

- `Exits` - количество выходов

#### Пример

```csharp
var emptyOptions = new EmptyOptions
{
    Exits = 3
};

var router = new Empty(emptyOptions);
```

## Связывание узлов

Узлы связываются через методы `SetExits`:

```csharp
// Простая цепочка
source.SetExits(new[] { queue });
queue.SetExits(new[] { service });
service.SetExits(new[] { sink });

// Маршрутизация
var router = new Empty(new EmptyOptions { Exits = 2 });
router.SetExits(new[] { queue1, queue2 });
```

## Работа с запросами

### Создание запроса

```csharp
var request = Request.Create(nodeId);
```

### Обработка запроса

```csharp
node.Process(request);
```

## Метрики узлов

Каждый узел автоматически собирает метрики:

### Source

- `{Id}_Source_Generated` - счетчик сгенерированных запросов
- `{Id}_Source_GenerationTime` - время между генерациями

### Queue

- `{Id}_Queue_Size` - текущий размер очереди
- `{Id}_Queue_Size_History` - история размеров очереди
- `{Id}_Queue_WaitTime` - время ожидания в очереди
- `{Id}_Queue_Dropped` - счетчик отклоненных запросов (при переполнении)

### Service

- `{Id}_Service_Completed` - счетчик обработанных запросов
- `{Id}_Service_Duration` - продолжительность обслуживания
- `{Id}_Service_BusyTime` - время занятости
- `{Id}_Service_IsBusy` - индикатор занятости (0 или 1)

## Настройка узлов

Все узлы настраиваются через классы опций:

```csharp
var options = new NodeOptions
{
    Id = "my-node",
    Enters = 1,
    Exits = 1
};
```

Опции могут быть переданы в конструктор узла или установлены позже через свойства.
