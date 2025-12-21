# Transmitly.Microsoft.Extensions.Dependencyinjection

A [Transmitly™](https://github.com/transmitly/transmitly) extension to help with configuring a communications client using Microsoft Dependency Injection.

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

### Advanced Configuration
If you need to access a database or service or just in general need to access services via dependency injection consdering registring an `ICommunicationsClientConfigurator` object to give yourself more control. 

First we'll create implement the `ICommunicationsClientConfigurator`

MyPlatformCommsConfig.cs

```csharp
public class MyPlatformCommsConfig : ICommunicationsClientConfigurator
{
    private readonly IOptions<TransmitlySettings> _options;
    private readonly ICommunicationsPipelineConfigurator[] _pipelineConfigurators;

    // ICommunicationsPipelineConfigurator would be a custom inteface and you would be responsible for registering these implementations.
    public MyPlatformCommsConfig(IOptions<TransmitlySettings> options, IEnumerable<ICommunicationsPipelineConfigurator> pipelineConfigurators) 
    {
        _options = options;
        _pipelineConfigurators = [.. pipelineConfigurators];
    }

    public void ConfigureClient(CommunicationsClientBuilder cfg)
    {
        var transmitlySettings = _options.Value;

        cfg
        .AddTwilioSupport(twilio =>
        {
            var twilioConfig = transmitlySettings.ChannelProviders.Twilio;

            twilio.AuthToken = twilioConfig.AuthToken;
            twilio.AccountSid = twilioConfig.AccountSid;
        })
        .AddSmtpSupport(smtp =>
        {
            var smtpConfig = transmitlySettings.ChannelProviders.Smtp;

            smtp.Host = smtpConfig.Host;
            smtp.Port = smtpConfig.Port;
            smtp.UserName = smtpConfig.UserName;
            smtp.Password = smtpConfig.Password;
        });
    }

    private void ConfigurePipelines(CommunicationsClientBuilder cfg)
    {
        foreach (var communicationsPipelineConfigurator in _pipelineConfigurators)
        {
            cfg.AddPipeline(communicationsPipelineConfigurator.Name, cfg =>
            {
                communicationsPipelineConfigurator.Configure(cfg);
            });
        }
    }

}
```
If you're familiiar with Transmitly, it's doing all the same things you're familiar with. The difference is you can now use registered classes and interfaces from your app's registration. This exmpale allows you to pull your config from jsut about anywhere. It also allows you to define custom pipeline configurators. These would allow you to spread your pipline configuration by domain, service, team or just about any way that makes sense for your team.

Example Pipline Configurator

```csharp
public class MyOrderPlacePipeline : ICommunicationsPipelineConfigurator
{
  public string Name => "ordering.placement.thankyou";

  public void Configure(IPipelineConfiguration pipeline)
  {
    pipeline
      .AddEmail("from@example.com".AsIdentityAddress(), email =>
      {
        email.Subject.AddStringTemplate("Transmit.ly order receipt: {{OrderId}}");
        email.HtmlBody.AddStringTemplate("<h1>Thank you for your order {{aud.FirstName}}</h1><p>Totaling ${{GrandTotal}}.  Your order id is {{OrderId}}.</p>");
        email.TextBody.AddStringTemplate("Thank you for your order {{aud.FirstName}}, totaling ${{GrandTotal}} dollars.  Your order id is {{OrderId}}.");
      })
      .AddVoice("18881234567".AsIdentityAddress(), sms =>
      {
        sms.Message.AddStringTemplate("Thank you for your order {{aud.FirstName}}, totaling {{GrandTotal}} dollars.  Your order id is {{OrderId}}");
      });
  }
}

```

Next, we'll need to let Transmitly know you're taking going to take control over the configuration in our `MyPlatformCommsConfig` class

Program.cs
```csharp
using Transmitly;
...

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransmitly<MyPlatformCommsConfig>();
```
Yup, it's that simple. Now when you resolve an `ICommunicationsClient` your custom `MyPlatformCommsConfig` will be called.



### Still need more control?
If implementing your own `ICommunicationsClientConfigurator` isn't enough control. Consider also registering your own custom `ICommunciationsClient` implementations

MyCustomClient
```csharp
public class InjectedConfiguredCommunicationsClient : ICommunicationsClient
{
  private readonly Lazy<ICommunicationsClient> _lazy;

  public InjectedConfiguredCommunicationsClient(ICommunicationsClientConfigurator configurator)
  {
    _lazy = new Lazy<ICommunicationsClient>(() =>
    {
      var cfg = new CommunicationsClientBuilder();
      configurator.ConfigureClient(cfg);
      return cfg.BuildClient();
    });
  }

  public ICommunicationsClient Client
  {
    get
    {
      return _lazy.Value;
    }
  }

  public void DeliverReport(DeliveryReport report)
  {
    Client.DeliverReport(report);
  }

  public void DeliverReports(IReadOnlyCollection<DeliveryReport> reports)
  {
    Client.DeliverReports(reports);
  }

  public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentityProfile> platformIdentities, ITransactionModel transactionalModel, IReadOnlyCollection<string> channelPreferences, string? cultureInfo = null, CancellationToken cancellationToken = default)
  {
    return Client.DispatchAsync(pipelineName, platformIdentities, transactionalModel, channelPreferences, cultureInfo, cancellationToken);
  }

  public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentityReference> identityReferences, ITransactionModel transactionalModel, IReadOnlyCollection<string> channelPreferences, string? cultureInfo = null, CancellationToken cancellationToken = default)
  {
    return Client.DispatchAsync(pipelineName, identityReferences, transactionalModel, channelPreferences, cultureInfo, cancellationToken);
  }
```


Registration example

```csharp
using Transmitly;
...

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransmitly<MyPlatformCommsConfig, InjectedConfiguredCommunicationsClient>();
```

Registering your own communications client gives you the ultimate flexibility and control over configuration, and execution of your notifications.




### Copyright and Trademark 

Copyright © 2024–2025 Code Impressions, LLC.

Transmitly™ is a trademark of Code Impressions, LLC.

This open-source project is sponsored and maintained by Code Impressions
and is licensed under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html).

The Apache License applies to the software code only and does not grant
permission to use the Transmitly name or logo, except as required to
describe the origin of the software.
