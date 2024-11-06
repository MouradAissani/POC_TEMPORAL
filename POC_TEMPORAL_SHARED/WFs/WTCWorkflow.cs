using Microsoft.Extensions.Logging;

namespace POC_TEMPORAL_SHARED.WFs
{
    [Workflow]
    public class WTCWorkflow : IWTCWorkflow
    {
        public static Action<string, object?, object?>? ReportProgress { get; set; }

        [WorkflowRun]
        public async Task RunWTCWorkflowAsync()
        {
            try
            {
                // Step 1: Get Customer
                ReportProgress?.Invoke("GetCustomer", null, null);
                var customer = await Workflow.ExecuteActivityAsync(
                    (IWTCActivities a) => a.GetCustomerAsync(),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(10) }
                );
                ReportProgress?.Invoke("GetCustomer", null, customer);

                // Step 2: Get Units for the Customer
                ReportProgress?.Invoke("GetUnits", customer.Id, null);
                var units = await Workflow.ExecuteActivityAsync(
                    (IWTCActivities a) => a.GetUnitsAsync(customer.Id),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(10) }
                );
                ReportProgress?.Invoke("GetUnits", customer.Id, units);

                // Step 3-5: For each unit, execute Prefetch, Dispatch, and ChangeMode in sequence
                foreach (var unit in units)
                {
                    // Step 3: Prefetch
                    ReportProgress?.Invoke("Prefetch", new { unit.TenantId, unit.DeviceId }, null);
                    var prefetchResponse = await Workflow.ExecuteActivityAsync(
                        (IWTCActivities a) => a.PrefetchAsync(unit.TenantId, unit.DeviceId),
                        new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(10) }
                    );
                    ReportProgress?.Invoke("Prefetch", new { unit.TenantId, unit.DeviceId }, prefetchResponse);

                    // Step 4: Dispatch, only if Prefetch was successful
                    if (prefetchResponse.StatusCode == 0)
                    {
                        ReportProgress?.Invoke("Dispatch", new { unit.TenantId, unit.DeviceId }, null);
                        var dispatchResponse = await Workflow.ExecuteActivityAsync(
                            (IWTCActivities a) => a.DispatchAsync(unit.TenantId, unit.DeviceId, "CMEDCB1"),
                            new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(10) }
                        );
                        ReportProgress?.Invoke("Dispatch", new { unit.TenantId, unit.DeviceId }, dispatchResponse);

                        // Step 5: Change Mode, only if Dispatch was successful
                        if (dispatchResponse.StatusCode == 0)
                        {
                            ReportProgress?.Invoke("ChangeMode", new { unit.TenantId, unit.DeviceId }, null);
                            var modeResponse = await Workflow.ExecuteActivityAsync(
                                (IWTCActivities a) => a.ChangeModeAsync(unit.TenantId, unit.DeviceId, "CMEACT0"),
                                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(10) }
                            );
                            ReportProgress?.Invoke("ChangeMode", new { unit.TenantId, unit.DeviceId }, modeResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Workflow.Logger.LogError("Error in workflow: {0}", ex.Message);
                throw;
            }
        }
    }
}
