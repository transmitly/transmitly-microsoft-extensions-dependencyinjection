# Transmitly.Microsoft.Extensions.DependencyInjection

Microsoft DI integration for [Transmitly](https://github.com/transmitly/transmitly).

This package wraps Transmitly registration so `ICommunicationsClient` and core Transmitly factories are wired through `IServiceCollection`, allowing constructor injection in your Transmitly components.

## What this package adds

- Registers `ICommunicationsClient` so it can be resolved from Microsoft DI.
- Bridges Transmitly factories to `IServiceProvider`, including channel provider dispatcher resolution and:
  - platform identity resolvers
  - content model enrichers
  - platform identity profile enrichers
- Supports class-based configuration via `ICommunicationsClientConfigurator`.
- Supports swapping in a custom `ICommunicationsClient` implementation.

## Install

```shell
dotnet add package Transmitly.Microsoft.Extensions.DependencyInjection
```

## Quick Start

Use inline builder configuration:

```csharp
using Transmitly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransmitly(tly =>
{
    // Configure channels, pipelines, templates, resolvers, enrichers, etc.
});

var app = builder.Build();

// Anywhere in your app:
var client = app.Services.GetRequiredService<ICommunicationsClient>();
```

## DI-Backed Components

If a resolver/enricher has constructor dependencies, register the type in DI first.

```csharp
builder.Services.AddSingleton<IMyDependency, MyDependency>();
builder.Services.AddTransient<MyIdentityResolver>();
builder.Services.AddTransient<MyContentModelEnricher>();
builder.Services.AddTransient<MyProfileEnricher>();

builder.Services.AddTransmitly(tly =>
{
    tly.AddPlatformIdentityResolver<MyIdentityResolver>();
    tly.AddContentModelEnricher<MyContentModelEnricher>();
    tly.AddPlatformIdentityProfileEnricher<MyProfileEnricher>();
});
```

If these types are added to the Transmitly builder but not registered in DI, they may fail to resolve at dispatch time.

## Class-Based Configuration

If you want the Transmitly setup itself to use DI dependencies, implement `ICommunicationsClientConfigurator`.

```csharp
public sealed class MyTransmitlyConfigurator : ICommunicationsClientConfigurator
{
    private readonly IOptions<MyTransmitlyOptions> _options;

    public MyTransmitlyConfigurator(IOptions<MyTransmitlyOptions> options)
    {
        _options = options;
    }

    public void ConfigureClient(CommunicationsClientBuilder builder)
    {
        // Build Transmitly config using _options and other injected services.
    }
}
```

Register it:

```csharp
builder.Services.AddTransmitly<MyTransmitlyConfigurator>();
```

## Custom Client Override

To provide your own `ICommunicationsClient` implementation:

```csharp
builder.Services.AddTransmitly<MyTransmitlyConfigurator, MyCommunicationsClient>();
```

## More Transmitly Docs

For full channel/pipeline/template configuration options, see the main [Transmitly project](https://github.com/transmitly/transmitly).

---
_Copyright (c) Code Impressions, LLC. This open-source project is sponsored and maintained by Code Impressions and is licensed under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
