using Microsoft.Extensions.Logging;
using POC_TEMPORAL_SHARED.WFs;
using Temporalio.Client;
using Temporalio.Worker;

// Create a Temporal client for the specified Temporal Cloud namespace with API Key
var client = await TemporalClient.ConnectAsync(new("cloudbaseinc-demo.jfb90.tmprl.cloud:7233")
{
    Namespace = "cloudbaseinc-demo.jfb90",
    LoggerFactory = LoggerFactory.Create(builder =>
        builder
            .AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ")
            .SetMinimumLevel(LogLevel.Information)),
    Tls = new TlsOptions
    {
        ClientCert = await File.ReadAllBytesAsync("ca.pem"),
        ClientPrivateKey = await File.ReadAllBytesAsync("ca.key")
    }
});

// Set up cancellation token for worker
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// Register and run the worker that manages workflows and activities in Temporal
Console.WriteLine("Running worker...");
var workerTask = Task.Run(async () =>
{
    using var worker = new TemporalWorker(
        client,
        new TemporalWorkerOptions("poc-task-queue")
            .AddAllActivities(new DailyActivities())
            .AddWorkflow<DailyWorkflow>());

    await worker.ExecuteAsync(tokenSource.Token);
});

// Start the workflow as a separate task
var workflowTask = Task.Run(async () =>
{
    var workflowId = $"workflow-{Guid.NewGuid()}";
    var handle = await client.StartWorkflowAsync(
        (IDailyWorkflow wf) => wf.RunDailyRoutineAsync(),
        new WorkflowOptions
        {
            Id = workflowId,
            TaskQueue = "poc-task-queue"
        });

    Console.WriteLine("Workflow started with ID: " + handle.Id);

    // Wait for workflow to complete and retrieve result
    await handle.GetResultAsync();
    Console.WriteLine("Daily routine workflow completed.");
});

// Wait for either task to complete (worker or workflow)
await Task.WhenAny(workerTask, workflowTask);
