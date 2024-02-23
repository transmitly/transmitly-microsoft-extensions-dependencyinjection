# Transmitly.Microsoft.Extensions.Dependencyinjection

A [Transmitly](https://github.com/transmitly/transmitly) extension to help with configuring a communications client using Microsoft Dependency Injection.

### Getting started

To use the dependency injection extension, first install the [NuGet package](https://nuget.org/packages/transmitly.microsoft.extensions.dependencyinjection):

```shell
dotnet add package Transmitly.Microsoft.Extensions.Dependencyinjection
```

Then start configuring...

```csharp
using Transmitly;
...

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransmitly(tly=>{
	//Check out the Transmit.ly project for details on configuration options!
});

```
* Check out the [Transmitly](https://github.com/transmitly/transmitly) project for more details on how to configure a communications client as well as how it can be used to improve how you manage your customer communications.


<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/transmitly/transmitly/assets/3877248/524f26c8-f670-4dfa-be78-badda0f48bfb">
  <img alt="an open-source project sponsored by CiLabs of Code Impressions, LLC" src="https://github.com/transmitly/transmitly/assets/3877248/34239edd-234d-4bee-9352-49d781716364" width="350" align="right">
</picture> 

---------------------------------------------------

_Copyright &copy; Code Impressions, LLC - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
