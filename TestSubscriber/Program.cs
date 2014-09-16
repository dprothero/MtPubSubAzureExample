using Configuration;
using MassTransit;
using System;

namespace TestSubscriber
{
  class Program
  {
    static void Main(string[] args)
    {
      var bus = AzureBusInitializer.CreateBus("TestSubscriber", sbc =>
      {
        sbc.SetConcurrentConsumerLimit(64);
        sbc.Subscribe(subs =>
        {
          subs.Consumer<SomethingHappenedConsumer>().Permanent();
        });
      });

      Console.ReadKey();

      bus.Dispose();
    }
  }
}
