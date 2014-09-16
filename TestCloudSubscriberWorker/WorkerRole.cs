using Configuration;
using MassTransit;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TestCloudSubscriberWorker
{
  public class WorkerRole : RoleEntryPoint
  {
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
    private IServiceBus _bus;

    public override void Run()
    {
      Trace.TraceInformation("TestCloudSubscriberWorker is running");

      try
      {
        this.RunAsync(this.cancellationTokenSource.Token).Wait();
      }
      finally
      {
        this.runCompleteEvent.Set();
      }
    }

    public override bool OnStart()
    {
      // Set the maximum number of concurrent connections
      ServicePointManager.DefaultConnectionLimit = 12;

      // For information on handling configuration changes
      // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

      bool result = base.OnStart();

      if(result)
      {
        _bus = AzureBusInitializer.CreateBus("TestCloudSubscriber", sbc =>
        {
          sbc.SetConcurrentConsumerLimit(64);
          sbc.Subscribe(subs =>
          {
            subs.Consumer<SomethingHappenedConsumer>().Permanent();
          });
        });
      }
      
      Trace.TraceInformation("TestCloudSubscriberWorker has been started");

      return result;
    }

    public override void OnStop()
    {
      Trace.TraceInformation("TestCloudSubscriberWorker is stopping");

      this.cancellationTokenSource.Cancel();
      this.runCompleteEvent.WaitOne();

      if(_bus != null)
        _bus.Dispose();
      
      base.OnStop();

      Trace.TraceInformation("TestCloudSubscriberWorker has stopped");
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
      // TODO: Replace the following with your own logic.
      while (!cancellationToken.IsCancellationRequested)
      {
        Trace.TraceInformation("Working");
        await Task.Delay(1000);
      }
    }
  }
}
