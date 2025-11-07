# Метрики

Система метрик позволяет собирать и анализировать данные о работе симуляции.

## MetricCollector

`MetricCollector` - основной класс для сбора метрик.

### Типы метрик

#### Counter (Счетчик)

Увеличивает значение на заданную величину.

```csharp
collector.CounterIncrement("my_counter");
collector.CounterIncrement("my_counter", 5);
```

#### Gauge (Измеритель)

Записывает текущее значение метрики.

```csharp
collector.GaugeRecord("queue_size", 10);
collector.GaugeRecord("cpu_usage", 0.75);
```

#### List (Список)

Добавляет значение в список для последующего анализа.

```csharp
collector.ListAdd("service_duration", 5.2);
collector.ListAdd("wait_time", 3.1);
```

#### Histogram (Гистограмма)

Записывает значения для построения гистограммы.

```csharp
collector.HistogramRecord("response_time", 100);
```

## Получение метрик

### Получение счетчика

```csharp
var count = collector.GetCounter("my_counter");
```

### Получение измерителя

```csharp
var currentValue = collector.GetGauge("queue_size");
var average = collector.GetGaugeAverage("queue_size");
var min = collector.GetGaugeMin("queue_size");
var max = collector.GetGaugeMax("queue_size");
```

### Получение списка

```csharp
var values = collector.GetList("service_duration");
var average = values.Average();
var sum = values.Sum();
var count = values.Count;
```

### Получение гистограммы

```csharp
var histogram = collector.GetHistogram("response_time");
```

## Сбор метрик

Метрики собираются после завершения симуляции:

```csharp
simulation.Simulate();
collector.Collect(); // Собирает все метрики
```

## Sinks (Приемники метрик)

Sinks определяют, куда отправляются собранные метрики.

### ConsoleSink

Выводит метрики в консоль.

```csharp
collector.AddSink(new ConsoleSink());
```

### FileSink

Записывает метрики в файл.

```csharp
collector.AddSink(new FileSink("metrics.json"));
```

### StatisticsSink

Собирает статистику по метрикам.

```csharp
collector.AddSink(new StatisticsSink());
```

## Стандартные метрики

Система автоматически собирает следующие метрики:

### Системные метрики

- `System_TotalTime` - общее время симуляции

### Метрики Source

- `{Id}_Source_Generated` - количество сгенерированных запросов
- `{Id}_Source_GenerationTime` - время между генерациями

### Метрики Queue

- `{Id}_Queue_Size` - размер очереди
- `{Id}_Queue_Size_History` - история размеров очереди
- `{Id}_Queue_WaitTime` - время ожидания в очереди
- `{Id}_Queue_Dropped` - количество отклоненных запросов

### Метрики Service

- `{Id}_Service_Completed` - количество обработанных запросов
- `{Id}_Service_Duration` - продолжительность обслуживания
- `{Id}_Service_BusyTime` - время занятости
- `{Id}_Service_IsBusy` - индикатор занятости

## Анализ метрик

После сбора метрик вы можете проанализировать их:

```csharp
var collector = simulation.Collector;
collector.Collect();

// Среднее время обслуживания
var serviceDurations = collector.GetList("Service_Duration");
var avgServiceTime = serviceDurations.Average();

// Средний размер очереди
var avgQueueSize = collector.GetGaugeAverage("Queue_Size");

// Количество обработанных запросов
var completedCount = collector.GetCounter("Service_Completed");

// Утилизация сервиса
var totalTime = collector.GetGauge("System_TotalTime");
var busyTime = collector.GetList("Service_BusyTime").Sum();
var utilization = busyTime / totalTime;
```

## Создание пользовательских метрик

Вы можете создавать собственные метрики в узлах:

```csharp
public override void Update(double deltaTime)
{
    // Ваша логика
    
    // Сбор метрик
    Context.Collector.CounterIncrement("my_custom_counter");
    Context.Collector.GaugeRecord("my_custom_gauge", value);
    Context.Collector.ListAdd("my_custom_list", value);
}
```
