# Структуры данных

Библиотека Structures предоставляет общие структуры данных и узлы для алгоритмов моделирования.

## Хранилища данных

### IStorage

Базовый интерфейс для хранилищ данных.

### FifoStorage

Хранилище типа "первым пришел - первым ушел" (FIFO).

```csharp
var storage = new FifoStorage<int>();
storage.Enqueue(1);
storage.Enqueue(2);
int value = storage.Dequeue(); // Возвращает 1
```

### LifoStorage

Хранилище типа "последним пришел - первым ушел" (LIFO), также известное как стек.

```csharp
var storage = new LifoStorage<int>();
storage.Push(1);
storage.Push(2);
int value = storage.Pop(); // Возвращает 2
```

## Узлы моделирования

### Базовые интерфейсы

#### INode

Базовый интерфейс для всех узлов моделирования.

#### IStorageNode

Узел с хранилищем данных.

#### IRouteNode

Узел-маршрутизатор для направления запросов.

#### IDistributionNode

Узел с распределением для генерации случайных значений.

### Абстрактные базовые классы

#### NodeBase

Базовый класс для всех узлов.

#### StorageNode

Базовый класс для узлов с хранилищем.

#### RouteNode

Базовый класс для узлов-маршрутизаторов.

#### DistributionNode

Базовый класс для узлов с распределением.

### Конкретные реализации узлов

#### EmptyBase

Пустой узел, используемый как маршрутизатор.

#### QueueBase

Базовый класс для узлов-очередей.

#### ServiceBase

Базовый класс для узлов-сервисов.

#### SourceBase

Базовый класс для узлов-источников.

#### SinkBase

Базовый класс для узлов-приемников.

## Модели

### Request

Представляет запрос в системе моделирования.

```csharp
var request = Request.Create(nodeId);
```

### Range

Представляет диапазон значений.

```csharp
var range = new Range(min: 0.0, max: 10.0);
```

## Контекст симуляции

### SimulationContext

Контекст симуляции, содержащий текущее время и метрики.

```csharp
var context = new SimulationContext(collector);
context.CurrentTime = 0.0;
context.Tick(0.1); // Увеличивает время на 0.1
```

### Simulation

Базовый класс для симуляций.

```csharp
public abstract class Simulation
{
    public SimulationContext? Context { get; protected set; }
    public MetricCollector? Collector { get; protected set; }
    public List<INode> Nodes { get; set; } = new();
    
    public abstract void Simulate();
}
```

## Опции узлов

Каждый тип узла имеет свой класс опций:

- `NodeOptions` - базовые опции узла
- `StorageOptions` - опции для узлов с хранилищем
- `RouteOptions` - опции для узлов-маршрутизаторов
- `DistributionOptions` - опции для узлов с распределением
- `EmptyOptions` - опции для пустых узлов
- `QueueOptions` - опции для очередей
- `ServiceOptions` - опции для сервисов
- `SourceOptions` - опции для источников
- `SinkOptions` - опции для приемников

## Метрики узлов

Система предоставляет классы для сбора метрик узлов:

- `NodeMetrics` - базовые метрики узла
- `StorageMetrics` - метрики хранилища
- `RouterMetrics` - метрики маршрутизатора
- `DistributionMetrics` - метрики распределения

## Расширения

### RouteNodeExtensions

Расширения для работы с узлами-маршрутизаторами.

```csharp
var nextNode = routeNode.GetAvailableExit();
```
