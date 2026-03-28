namespace FrontEnd.Data;

public class ProductClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductClient> _logger;

    public ProductClient(HttpClient httpClient, ILogger<ProductClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Product[]> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<Product[]>("products") ?? Array.Empty<Product>();

    public async Task<Product?> GetByIdAsync(int id)
        => await _httpClient.GetFromJsonAsync<Product>($"products/{id}");

    public async Task<Product?> CreateAsync(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("products", product);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Falha ao criar produto: {StatusCode}", response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<bool> UpdateAsync(int id, Product product)
    {
        var response = await _httpClient.PutAsJsonAsync($"products/{id}", product);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"products/{id}");
        return response.IsSuccessStatusCode;
    }
}
