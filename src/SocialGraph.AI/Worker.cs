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
                    // Subtract the 7s delay already spent inside PushIncrementalData
                    var interval = _configuration.GetValue<int>("SimulationSettings:SimulationIntervalMs", 15000);
                    await Task.Delay(Math.Max(0, interval - 7000), stoppingToken);
                    continue; // Skip the default delay at the end
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("API Baglanti Hatasi: {Message}. 10 saniye sonra tekrar denenecek...", ex.Message);
                await Task.Delay(10000, stoppingToken);
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogError("Beklenmedik Hata: {Message}", ex.Message);
            }

            var defaultInterval = _configuration.GetValue<int>("SimulationSettings:SimulationIntervalMs", 15000);
            await Task.Delay(defaultInterval, stoppingToken);
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

    private bool _lastWasUnfriend = false;

    private async Task PushIncrementalData(CancellationToken stoppingToken)
    {
        var rnd = new Random();
        var nNodes = 1; 
        var nEdges = 0; 

        _logger.LogInformation("Simulasyon fazi 1: Yeni Veriler...");
        var (nodes, edges) = _dataGenerator.GenerateIncrementalData(nNodes, nEdges);

        // 1. Yeni verileri gonder
        if (nodes.Count > 0)
            await _httpClient.PostAsJsonAsync("/api/nodes/batch", nodes, stoppingToken);
        if (edges.Count > 0)
            await _httpClient.PostAsJsonAsync("/api/edges/batch", edges, stoppingToken);

        // 2. Aksiyonlari Logla
        foreach (var node in nodes)
        {
            var name = node.Properties.TryGetValue("Name", out var n) ? n.ToString() : 
                       node.Properties.TryGetValue("Title", out var t) ? t.ToString() : node.Id;
            await LogAction(0, node.Id, name!, "", "", $"{node.Type} katildi: {name}", stoppingToken);
        }

        foreach (var edge in edges)
        {
            if (edge.Id.EndsWith("_reverse")) continue;
            var type = 1; // EdgeAdded
            var desc = $"{edge.SourceId} -> {edge.TargetId} ({edge.RelationType})";
            await LogAction(type, edge.SourceId, "", edge.TargetId, "", desc, stoppingToken);
        }

        await Task.Delay(7000, stoppingToken);

        _logger.LogInformation("Simulasyon fazi 2: Yikici Aksiyonlar...");
        // 3. Yikici Aksiyon (Unfriend/Unlike)
        await PerformDestructiveAction(stoppingToken);

        _logger.LogInformation("Simulasyon dongusu tamamlandi.");
    }

    private async Task LogAction(int type, string sId, string sName, string tId, string tName, string desc, CancellationToken ct)
    {
        var action = new {
            Type = type,
            SourceId = sId,
            SourceName = sName,
            TargetId = tId,
            TargetName = tName,
            Description = desc,
            Timestamp = DateTime.UtcNow
        };
        await _httpClient.PostAsJsonAsync("/api/simulation/actions", action, ct);
    }

    private async Task PerformDestructiveAction(CancellationToken ct)
    {
        try 
        {
            // 1. Mevcut tum kenarlari cek
            var edges = await _httpClient.GetFromJsonAsync<List<EdgeDto>>("/api/edges", ct);
            if (edges == null || edges.Count == 0) return;

            // 2. Her dugumun kac tane iliskisi oldugunu say (Koruma mekanizmasi)
            var relationCounts = new Dictionary<string, int>();
            foreach (var e in edges)
            {
                relationCounts[e.SourceId] = relationCounts.GetValueOrDefault(e.SourceId) + 1;
                relationCounts[e.TargetId] = relationCounts.GetValueOrDefault(e.TargetId) + 1;
            }

            // Esik degeri: 3'ten az iliskisi kalan dugumlerde yikici islem yapma
            const int SafetyThreshold = 3;

            _lastWasUnfriend = !_lastWasUnfriend;
            var rnd = new Random();

            if (_lastWasUnfriend)
            {
                // Unfriend icin: Hem kaynak hem hedef 3'ten fazla iliskisi olan FRIEND kenarlarini bul
                var eligibleEdges = edges.Where(e => 
                    e.RelationType == "FRIEND" && 
                    relationCounts.GetValueOrDefault(e.SourceId) > SafetyThreshold && 
                    relationCounts.GetValueOrDefault(e.TargetId) > SafetyThreshold
                ).ToList();

                if (eligibleEdges.Count > 0)
                {
                    var edge = eligibleEdges[rnd.Next(eligibleEdges.Count)];
                    var resp = await _httpClient.DeleteAsync($"/api/edges/{edge.SourceId}/{edge.TargetId}", ct);
                    if (resp.IsSuccessStatusCode)
                    {
                        await LogAction(2, edge.SourceId, "", edge.TargetId, "", "Arkadaslik sona erdi (Unfriend)", ct);
                    }
                }
                else 
                {
                    _lastWasUnfriend = false;
                }
            }
            
            if (!_lastWasUnfriend)
            {
                // Unlike icin: Benzer koruma mantigi
                var eligibleEdges = edges.Where(e => 
                    e.RelationType == "LIKES" && 
                    relationCounts.GetValueOrDefault(e.SourceId) > SafetyThreshold && 
                    relationCounts.GetValueOrDefault(e.TargetId) > SafetyThreshold
                ).ToList();

                if (eligibleEdges.Count > 0)
                {
                    var edge = eligibleEdges[rnd.Next(eligibleEdges.Count)];
                    var resp = await _httpClient.DeleteAsync($"/api/edges/{edge.SourceId}/{edge.TargetId}", ct);
                    if (resp.IsSuccessStatusCode)
                    {
                        await LogAction(3, edge.SourceId, "", edge.TargetId, "", "Begeni geri cekildi (Unlike)", ct);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Yikici aksiyon koruma filtresine takildi veya hata olustu: {Message}", ex.Message);
        }
    }

    // EdgeDto local class for deserialization
    public class EdgeDto {
        public string SourceId { get; set; } = "";
        public string TargetId { get; set; } = "";
        public string RelationType { get; set; } = "";
    }
}
