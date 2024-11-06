using POC_TEMPORAL_SHARED.Models;
using POC_TEMPORAL_SHARED.WFs;
using System.Text;
using System.Text.Json;

public class WTCActivities : IWTCActivities
{
    private readonly HttpClient _httpClient;

    public WTCActivities(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [Activity]
    public async Task<CustomerResponse> GetCustomerAsync()
    {
        var response = await _httpClient.GetAsync("customer-portal/Customer/master");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CustomerResponse>(content);
    }

    [Activity]
    public async Task<UnitResponse[]> GetUnitsAsync(string customerId)
    {
        var response = await _httpClient.GetAsync($"customer-portal/Assets/master?customerId={customerId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UnitResponse[]>(content);
    }

    [Activity]
    public async Task<PrefetchResponse> PrefetchAsync(string tenantId, string deviceId)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { TenantId = tenantId, DeviceId = deviceId }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("command/commands/tenantId/deviceId/callFunction", requestContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PrefetchResponse>(content);
    }

    [Activity]
    public async Task<DispatchResponse> DispatchAsync(string tenantId, string deviceId, string functionArgument)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { TenantId = tenantId, DeviceId = deviceId, FunctionArgument = functionArgument }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("command/commands/tenantId/deviceId/callFunction", requestContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DispatchResponse>(content);
    }

    [Activity]
    public async Task<ModeChangeResponse> ChangeModeAsync(string tenantId, string deviceId, string functionArgument)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { TenantId = tenantId, DeviceId = deviceId, FunctionArgument = functionArgument }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("command/commands/tenantId/deviceId/callFunction", requestContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ModeChangeResponse>(content);
    }
}
