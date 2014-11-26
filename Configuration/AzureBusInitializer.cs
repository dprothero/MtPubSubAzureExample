using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Log4NetIntegration.Logging;
using MassTransit.Transports.AzureServiceBus;
using System;
using System.Configuration;

namespace Configuration
{
  public class AzureBusInitializer
  {
    public static IServiceBus CreateBus(string queueName, Action<ServiceBusConfigurator> moreInitialization)
    {
      Log4NetLogger.Use();
      var bus = ServiceBusFactory.New(sbc =>
      {
        var azureNameSpace = GetConfigValue("azure-namespace", "loosely");
        var queueUri = "azure-sb://" + azureNameSpace + "/MtPubSubAzureExample_" + queueName;

        sbc.ReceiveFrom(queueUri);
        SetupAzureServiceBus(sbc, azureNameSpace);

        moreInitialization(sbc);
      });

      return bus;
    }

    private static void SetupAzureServiceBus(ServiceBusConfigurator sbc, string azureNameSpace)
    {
      sbc.UseAzureServiceBus(a => a.ConfigureNamespace(azureNameSpace, h =>
      {
        h.SetKeyName(GetConfigValue("azure-keyname", "RootManageSharedAccessKey"));
        h.SetKey(GetConfigValue("azure-key", ""));
      }));
      sbc.UseAzureServiceBusRouting();
    }

    private static string GetConfigValue(string key, string defaultValue)
    {
      string value = ConfigurationManager.AppSettings[key];
      return string.IsNullOrEmpty(value) ? defaultValue : value;
    }

  }
}
