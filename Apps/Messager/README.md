# Messager.NET

## Overview

A lightweight and fast messaging system (pub/sub) for .NET 10 with support for:

- Keyed and unkeyed typed events;
- Synchronous and asynchronous senders/receivers;
- Lazy broker creation for each event type;
- Integration with DI ([**Autofac**](https://github.com/autofac/Autofac));
- Single point of contact — **Exchange**.

The goal is simple, transparent, and thread-safe event exchange between services/modules without unnecessary coupling.

## Roadmap

- Refactor the code to make it more readable.
- Add self-made code analyzers for preventing common mistakes.
- Consider decoupling from [**Autofac**](https://github.com/autofac/Autofac) and switching to the simpler [**Microsoft.Extensions.DependencyInjection**](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/).
- Support for middleware and event pipelines (logging, filtering, retrievals).
- Other message handling options (e.g., **Channel**, **IObservable**).
- Enhanced diagnostics and metrics for brokers and subscribers.
- Enhanced subscriber error handling policies.

## How it works

### Pub/Sub

#### Simple

  - **ISender<TEvent>**
  - **IReceiver<TEvent>**

#### Keyed

  - **ISender<TKey, TEvent>**
  - **IReceiver<TKey, TEvent>**

#### Async Simple

- **IAsyncSender<TEvent>**
- **IAsyncReceiver<TEvent>**

#### Async Keyed

- **IAsyncSender<TKey, TEvent>**
- **IAsyncReceiver<TKey, TEvent>**

### Request

#### Simple

- **IRequest<TIn, TOut>**

### Keyed

- **IRequest<TKey, TIn, TOut>**

### Async Simple

- **IAsyncRequest<TIn, TOut>**

### Async Keyed

- **IAsyncRequest<TKey, TIn, TOut>**

## Usage

[Посмотреть](.github/docs/en/Examples.md)

## Thanks

- Ideas and inspiration from [**MessagePipe**](https://github.com/Cysharp/MessagePipe) ([**Cysharp**](https://github.com/Cysharp)). My project is essentially a rewrite because I encountered that MessagePipe wasn't working for me.

## License

[**Messager.NET**]() is licensed under the [**MIT License**](), see [LICENSE](LICENSE) for more information.