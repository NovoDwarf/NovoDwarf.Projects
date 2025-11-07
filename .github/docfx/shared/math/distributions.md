# Математика - Распределения и статистика

Библиотека Mathematics предоставляет набор математических распределений и статистических функций для использования в алгоритмах моделирования.

## Категории распределений

### Базовые распределения

#### Deterministic (Детерминированное)

Возвращает постоянное значение.

```csharp
var dist = new Deterministic(5.0);
double value = dist.Calculate(); // Всегда возвращает 5.0
```

#### Uniform (Равномерное)

Равномерное распределение на интервале [min, max].

```csharp
var dist = new Uniform(min: 0.0, max: 10.0);
double value = dist.Calculate(); // Случайное значение от 0 до 10
```

#### Exponential (Экспоненциальное)

Экспоненциальное распределение с заданным параметром rate.

```csharp
var dist = new Exponential(rate: 2.0);
double value = dist.Calculate(); // Экспоненциально распределенное значение
```

#### Normal (Нормальное)

Нормальное (гауссово) распределение.

```csharp
var dist = new Normal(mean: 0.0, stdDev: 1.0);
double value = dist.Calculate(); // Нормально распределенное значение
```

### Композитные распределения

#### Gamma (Гамма)

Гамма-распределение.

```csharp
var dist = new Gamma(shape: 2.0, scale: 1.0);
```

#### ExponentialHyper (Гиперэкспоненциальное)

Гиперэкспоненциальное распределение.

```csharp
var dist = new ExponentialHyper(rates: new[] { 1.0, 2.0, 3.0 });
```

#### ExponentialHypo (Гипоэкспоненциальное)

Гипоэкспоненциальное распределение.

```csharp
var dist = new ExponentialHypo(rates: new[] { 1.0, 2.0 });
```

### Производные распределения

#### Erlang

Распределение Эрланга.

```csharp
var dist = new Erlang(shape: 3, rate: 2.0);
```

#### Weibull

Распределение Вейбулла.

```csharp
var dist = new Weibull(shape: 2.0, scale: 1.0);
```

#### NormalLog (Логнормальное)

Логнормальное распределение.

```csharp
var dist = new NormalLog(mean: 0.0, stdDev: 1.0);
```

#### NormalTruncated (Усеченное нормальное)

Усеченное нормальное распределение.

```csharp
var dist = new NormalTruncated(mean: 0.0, stdDev: 1.0, min: -1.0, max: 1.0);
```

### Распределения с тяжелыми хвостами

#### Pareto

Распределение Парето.

```csharp
var dist = new Pareto(shape: 2.0, scale: 1.0);
```

#### Cauchy

Распределение Коши.

```csharp
var dist = new Cauchy(location: 0.0, scale: 1.0);
```

#### Levy

Распределение Леви.

```csharp
var dist = new Levy(location: 0.0, scale: 1.0);
```

#### StudentsT

Распределение Стьюдента (t-распределение).

```csharp
var dist = new StudentsT(degreesOfFreedom: 10);
```

### Специализированные распределения

#### Beta

Бета-распределение.

```csharp
var dist = new Beta(alpha: 2.0, beta: 3.0);
```

#### Triangular (Треугольное)

Треугольное распределение.

```csharp
var dist = new Triangular(min: 0.0, max: 10.0, mode: 5.0);
```

## Утилиты

### Статистические функции

Библиотека также предоставляет утилиты для статистических вычислений:

```csharp
var mean = StatsUtils.Mean(values);
var variance = StatsUtils.Variance(values);
var stdDev = StatsUtils.StandardDeviation(values);
```

### Генерация случайных чисел

```csharp
var random = RandomUtils.GetRandom();
double value = random.NextDouble();
```

## Использование в моделировании

Распределения используются в узлах моделирования для определения времени генерации запросов, времени обработки и других параметров:

```csharp
var sourceOptions = new SourceOptions
{
    Distribution = new Exponential(rate: 2.0)
};

var serviceOptions = new ServiceOptions
{
    Distribution = new Normal(mean: 5.0, stdDev: 1.0)
};
```
