using Contracts;
using MassTransit;
using System;
using System.Diagnostics;
using System.Threading;

namespace TestCloudSubscriberWorker
{
  class SomethingHappenedConsumer : Consumes<SomethingHappened>.Context
  {
    public void Consume(IConsumeContext<SomethingHappened> message)
    {
      Trace.TraceInformation("TXT: " + message.Message.What +
                             "  SENT: " + message.Message.When.ToString() +
                             "  PROCESSED: " + DateTime.Now.ToString() + 
                             " (" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + ")");

      // Simulate processing time
      Thread.Sleep(250);
    }
  }
}
