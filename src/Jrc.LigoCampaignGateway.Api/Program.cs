using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Services;
using Jrc.LigoCampaignGateway.Infrastructure.LigoClients;
using Jrc.LigoCampaignGateway.Infrastructure.MediaStorage;
using Jrc.LigoCampaignGateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("JrcLigoGatewayDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Storage Service
builder.Services.AddSingleton<IMediaAssetStorage, FileMediaAssetStorage>();

// Registration of Application Services
builder.Services.AddScoped<ITemplateRegistryService, TemplateRegistryService>();
builder.Services.AddScoped<IMediaLeaseService, MediaLeaseService>();
builder.Services.AddScoped<IHsmDispatchService, HsmDispatchService>();
builder.Services.AddScoped<IStatusIngestionService, StatusIngestionService>();

// Configure Ligo Mode (Mock vs Real)
var ligoMode = builder.Configuration["Ligo:Mode"] ?? "Mock";

if (ligoMode.Equals("Real", StringComparison.OrdinalIgnoreCase))
{
    var authUrl = builder.Configuration["Ligo:AuthBaseUrl"];
    var login = builder.Configuration["Ligo:AuthLogin"];
    var password = builder.Configuration["Ligo:AuthPassword"];
    var mediaUrl = builder.Configuration["Ligo:MediaBaseUrl"];
    var messagingUrl = builder.Configuration["Ligo:MessagingBaseUrl"];

    if (string.IsNullOrEmpty(authUrl) || string.IsNullOrEmpty(login) || login == "PENDENTE_CONTRATO" ||
        string.IsNullOrEmpty(password) || password == "PENDENTE_CONTRATO" ||
        string.IsNullOrEmpty(mediaUrl) || string.IsNullOrEmpty(messagingUrl))
    {
        throw new InvalidOperationException(
            "FATAL: LIGO_MODE=Real is active but mandatory Ligo URLs/credentials are missing in configuration. Startup terminated.");
    }

    builder.Services.AddHttpClient<ILigoAuthClient, LigoAuthClient>();
    builder.Services.AddSingleton<ILigoTokenProvider, LigoTokenProvider>();
    builder.Services.AddHttpClient<ILigoMediaClient, LigoMediaClient>();
    builder.Services.AddHttpClient<ILigoHsmClient, LigoHsmClient>();
}
else
{
    builder.Services.AddSingleton<ILigoAuthClient, MockLigoAuthClient>();
    builder.Services.AddSingleton<ILigoTokenProvider, LigoTokenProvider>();
    builder.Services.AddSingleton<ILigoMediaClient, MockLigoMediaClient>();
    builder.Services.AddSingleton<ILigoHsmClient, MockLigoHsmClient>();
}

var app = builder.Build();

// Seed initial template for mock testing
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Templates.Any())
    {
        var template = new Jrc.LigoCampaignGateway.Domain.Entities.WhatsAppTemplate
        {
            Tenant = "grupojrc",
            NumberChip = "551148004100",
            ProviderTemplateId = "65f9dbce1fb4e9ac773bd386",
            Name = "jrc_marketing_imagem",
            Language = "pt_BR",
            Category = "MARKETING",
            HeaderType = "IMAGE",
            ParameterCount = 2,
            CallbackStatusUrl = "https://gateway-whatsapp.jrcws.cloud/api/v1/webhooks/ligo/status",
            CallbackResponsesUrl = "https://www.ligo.cloud/whatsapp/response",
            Status = "READY",
            Active = true
        };
        db.TemplatesSet.Add(template);

        var asset = new Jrc.LigoCampaignGateway.Domain.Entities.MediaAsset
        {
            Id = Guid.Parse("78a58d57-f384-48fa-9abc-7b0ef5ff0cde"),
            OriginalFileName = "campanha_promocao.png",
            StoredFileName = "campanha_promocao.png",
            ContentType = "image/png",
            SizeBytes = 102450,
            Sha256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            StorageProvider = "LocalDisk",
            StoragePath = "Storage/Media/sample.png",
            PublicUrl = "/public/media/78a58d57-f384-48fa-9abc-7b0ef5ff0cde",
            Active = true
        };
        db.MediaAssetsSet.Add(asset);

        var lease = new Jrc.LigoCampaignGateway.Domain.Entities.ProviderMediaLease
        {
            TemplateId = template.Id,
            MediaAssetId = asset.Id,
            Provider = "LIGO",
            ProviderMediaId = "630634f689573dbee01c84c5",
            ValidUntilRaw = "31/12/2099",
            ValidUntilParsed = new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            ParseSucceeded = true,
            Status = Jrc.LigoCampaignGateway.Domain.Enums.MediaLeaseState.Active
        };
        db.MediaLeasesSet.Add(lease);

        db.SaveChanges();
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }
