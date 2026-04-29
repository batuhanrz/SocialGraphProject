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
        _logger.LogInformation("SocialGraph AI Worker baslatildi. [Muhammed Furkan - Faz 2]");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_isSeedDataPushed)
                {
                    await PushSeedData(stoppingToken);
                }
                else
                {
                    await PushIncrementalData(stoppingToken);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("API Baglanti Hatasi: {Message}. 10 saniye sonra tekrar denenecek...", ex.Message);
                await Task.Delay(10000, stoppingToken);
                continue; // Bir sonraki donguye gec, delay'i burada yaptik
            }
            catch (Exception ex)
            {
                _logger.LogError("Beklenmedik Hata: {Message}", ex.Message);
            }

            var interval = _configuration.GetValue<int>("SimulationSettings:SimulationIntervalMs", 15000);
            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task PushSeedData(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sentetik Seed Data uretiliyor... (Topoloji: Dense)");
        var (nodes, edges) = _dataGenerator.GenerateDenseGraph();
        
        _logger.LogInformation("Seed Data gonderiliyor: {NodeCount} dugum, {EdgeCount} kenar.", nodes.Count, edges.Count);

        var nodeResp = await _httpClient.PostAsJsonAsync("/api/nodes/batch", nodes, stoppingToken);
        nodeResp.EnsureSuccessStatusCode();

        var edgeResp = await _httpClient.PostAsJsonAsync("/api/edges/batch", edges, stoppingToken);
        edgeResp.EnsureSuccessStatusCode();

        _isSeedDataPushed = true;
        _logger.LogInformation("Seed Data basariyla API'ye yuklendi.");
    }

    private async Task PushIncrementalData(CancellationToken stoppingToken)
    {
        var nNodes = _configuration.GetValue<int>("SimulationSettings:NewNodesPerCycle", 2);
        var nEdges = _configuration.GetValue<int>("SimulationSettings:NewEdgesPerCycle", 3);

        _logger.LogInformation("Inkremental veri uretiliyor...");
        var (nodes, edges) = _dataGenerator.GenerateIncrementalData(nNodes, nEdges);

        if (nodes.Count > 0)
        {
            var nodeResp = await _httpClient.PostAsJsonAsync("/api/nodes/batch", nodes, stoppingToken);
            nodeResp.EnsureSuccessStatusCode();
        }

        if (edges.Count > 0)
        {
            var edgeResp = await _httpClient.PostAsJsonAsync("/api/edges/batch", edges, stoppingToken);
            edgeResp.EnsureSuccessStatusCode();
        }

        _logger.LogInformation("Simulasyon Dongusu Tamamlandi: +{NodeCount} dugum, +{EdgeCount} kenar API'ye eklendi.", nodes.Count, edges.Count);
    }
}
