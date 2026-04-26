using System.Net.Http.Json;

namespace SocialGraph.AI;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly DataGenerator _dataGenerator;
    private bool _isSeedDataPushed = false;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
        _dataGenerator = new DataGenerator();
        
        var apiBase = _configuration["ApiBaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(apiBase);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SocialGraph AI Worker baslatildi.");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_isSeedDataPushed)
            {
                _logger.LogInformation("Sentetik Seed Data uretiliyor... (Topoloji: Dense)");
                var (nodes, edges) = _dataGenerator.GenerateDenseGraph();
                
                _logger.LogInformation($"Uretilen Dugum: {nodes.Count}, Uretilen Kenar: {edges.Count}. API'ye post ediliyor...");

                try
                {
                    var nodeResponse = await _httpClient.PostAsJsonAsync("/api/nodes/batch", nodes, stoppingToken);
                    if (nodeResponse.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Dugumler basariyla API'ye gonderildi.");
                    }
                    else 
                    {
                        _logger.LogWarning($"Dugum post hatasi: {nodeResponse.StatusCode}");
                    }

                    var edgeResponse = await _httpClient.PostAsJsonAsync("/api/edges/batch", edges, stoppingToken);
                    if (edgeResponse.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Kenarlar basariyla API'ye gonderildi.");
                    }

                    _isSeedDataPushed = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"API'ye veri gonderilirken hata olustu: {ex.Message}. (API calisiyor mu?)");
                }
            }
            else
            {
                _logger.LogInformation("AI Worker Heartbeat. Rutin simulasyon kontrolu: Zaman: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(15000, stoppingToken);
        }
    }
}
