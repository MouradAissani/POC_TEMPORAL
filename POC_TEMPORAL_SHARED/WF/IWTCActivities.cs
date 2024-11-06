using POC_TEMPORAL_SHARED.Models;

namespace POC_TEMPORAL_SHARED.WFs;

public interface IWTCActivities
{
    [Activity]
    Task<CustomerResponse> GetCustomerAsync();
    [Activity]
    Task<UnitResponse[]> GetUnitsAsync(string customerId);
    [Activity]
    Task<PrefetchResponse> PrefetchAsync(string tenantId, string deviceId);
    [Activity]
    Task<DispatchResponse> DispatchAsync(string tenantId, string deviceId, string functionArgument);
    [Activity]
    Task<ModeChangeResponse> ChangeModeAsync(string tenantId, string deviceId, string functionArgument);
}