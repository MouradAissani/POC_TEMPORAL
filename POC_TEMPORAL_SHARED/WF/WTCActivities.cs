using POC_TEMPORAL_SHARED.Models;
using POC_TEMPORAL_SHARED.WFs;

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
        // Simulated API Response for "Get Customer API"
        await Task.Delay(100); // Simulate async delay
        return new CustomerResponse
        {
            Id = "customer123",
            DisplayName = "Sample Customer"
        };

        // Uncomment for real API call
        /*
        var response = await _httpClient.GetAsync("customer-portal/Customer/master");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CustomerResponse>(content);
        */
    }

    [Activity]
    public async Task<UnitResponse[]> GetUnitsAsync(string customerId)
    {
        // Simulated API Response for "Get Units API"
        await Task.Delay(100); // Simulate async delay
        return new UnitResponse[]
        {
            new UnitResponse { TenantId = "tenant001", DeviceId = "1", Latitude = "35.6895", Longitude = "139.6917" },
            new UnitResponse { TenantId = "tenant002", DeviceId = "2", Latitude = "40.7128", Longitude = "-74.0060" },
            new UnitResponse { TenantId = "tenant003", DeviceId = "3", Latitude = "51.5074", Longitude = "-0.1278" }
        };

        // Uncomment for real API call
        /*
        var response = await _httpClient.GetAsync($"customer-portal/Assets/master?customerId={customerId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UnitResponse[]>(content);
        */
    }

    [Activity]
    public async Task<PrefetchResponse> PrefetchAsync(string tenantId, string deviceId)
    {
        // Simulated API Response for "Prefetch API"
        await Task.Delay(100); // Simulate async delay
        return new PrefetchResponse
        {
            StatusCode = 0,
            StatusMessage = "Cloud to Device Command Passthrough Success"
        };

        // Uncomment for real API call
        /*
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { functionArgument = "", functionKey = "Passthrough", latitude = "", longitude = "" }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"command/commands/{tenantId}/{deviceId}/callFunction", requestContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PrefetchResponse>(content);
        */
    }

    [Activity]
    public async Task<DispatchResponse> DispatchAsync(string tenantId, string deviceId, string functionArgument)
    {
        // Simulated API Response for "Dispatch API"
        await Task.Delay(100); // Simulate async delay
        return new DispatchResponse
        {
            StatusCode = 0,
            StatusMessage = "Cloud to Device Command Passthrough Success"
        };

        // Uncomment for real API call
        /*
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { functionArgument = functionArgument, functionKey = "Passthrough", latitude = "", longitude = "" }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"command/commands/{tenantId}/{deviceId}/callFunction", requestContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DispatchResponse>(content);
        */
    }

    [Activity]
    public async Task<ModeChangeResponse> ChangeModeAsync(string tenantId, string deviceId, string functionArgument)
    {
        // Simulated API Response for "Change Mode API"
        await Task.Delay(100); // Simulate async delay
        return new ModeChangeResponse
        {
            StatusCode = 0,
            StatusMessage = "Cloud to Device Command Passthrough Success"
        };

        // Uncomment for real API call
        /*
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { functionArgument = functionArgument, functionKey = "Passthrough", latitude = "", longitude = "" }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"command/commands/{tenantId}/{deviceId}/callFunction", requestContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ModeChangeResponse>(content);
        */
    }
}
