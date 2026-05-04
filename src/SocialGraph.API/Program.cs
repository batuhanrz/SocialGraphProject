using System.Text.Json.Serialization;
using SocialGraph.API;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Servis Yapilandirmalari ---

// Controller destegi
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Graf yapisinda dongusel referanslari engelle
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SocialGraph API",
        Version = "v1",
        Description = "Property Graph tabanli sosyal ag modelleme sistemi icin REST API."
    });
});

// CORS — Frontend (React, port 5173, 3000 veya Docker 8080) erisimi icin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Singleton DI Kayitlari ---
// Veri yapilari uygulama omru boyunca tek instance olarak saklanir (Context.md B.1)
builder.Services.AddSingleton<CustomHashTable<string, Node>>(provider =>
{
    var store = new CustomHashTable<string, Node>();
    return store;
});

// Trie: Metin tabanli arama ve otomatik tamamlama icin (Isra - Sprint 1.5)
builder.Services.AddSingleton<CustomTrie>(provider =>
{
    var trie = new CustomTrie();
    return trie;
});

// PropertyGraph: Adjacency list tabanli cekirdek graf yapisi (Batuhan - Sprint 2.1)
// Tum dugum ve kenar verisi bu singleton instance uzerinde saklanir.
builder.Services.AddSingleton<PropertyGraph>();

var app = builder.Build();

// --- Middleware Pipeline ---

// Swagger her ortamda aktif (proje demo ve dogrulama icin gerekli)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialGraph API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");

app.MapControllers();

// Opsiyonel: Eski Sprint testlerini calistir
if (args.Contains("--run-tests"))
{
    TestRunner.RunAll();
}

Console.WriteLine("[SocialGraph API] Sunucu baslatildi. Swagger: http://localhost:{0}/swagger", 
    app.Urls.FirstOrDefault()?.Split(':').LastOrDefault() ?? "5000");

app.Run();
