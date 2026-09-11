using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src;
using src.Interfaces;
using src.Repositories;
using src.Services;

var builder = WebApplication.CreateBuilder(args);

// --- DbContext (PostgreSQL) ------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Default")
                        ?? "Host=localhost;Port=5432;Database=conference_booking;Username=postgres;Password=postgres";
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// --- Repositories --------------------------------------------------------
builder.Services.AddScoped<IHallRepository, HallRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// --- Application services -------------------------------------------------
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddSingleton<IPricingService, PricingService>(); // без стану — безпечно як singleton

// --- Controllers / API behavior -------------------------------------------
builder.Services.AddControllers();

// A standardized format for model validation errors ([FromBody]).
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .SelectMany(kv => kv.Value!.Errors.Select(e => e.ErrorMessage));
        return new BadRequestObjectResult(new { error = string.Join(" ", errors) });
    };
});

// --- CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => builder.Environment.IsDevelopment());
    });
});

// --- Rate limiting: Basic protection against API abuse.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// --- Swagger ---------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Conference Room Booking API",
        Version = "v1",
        Description = "An API for managing conference rooms, reservations, and calculating rental costs."
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xmlFile))
        options.IncludeXmlComments(xmlFile);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Allows WebApplicationFactory<Program> to recognize this class in tests.
public partial class Program { }
