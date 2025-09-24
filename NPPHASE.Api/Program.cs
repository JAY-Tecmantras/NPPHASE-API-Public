using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPPHASE.Apis;
using NPPHASE.Apis.CustomMiddleware;
using NPPHASE.Data.Context;
using NPPHASE.Data.Model;
using NPPHASE.Data.Seeder;
using NPPHASE.Services;
using NPPHASE.Services.IRepositories;
using NPPHASE.Services.Repositories;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var db = ("MySql");

// Add services to the container.
var logDirectory = Path.Combine(builder.Environment.ContentRootPath, "RequestLogs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
        }
    });
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.User.AllowedUserNameCharacters = string.Empty;
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<NPPHASEApiDbContext>()
            .AddDefaultTokenProviders();

builder.Services.AddDbContext<NPPHASEApiDbContext>(option =>
{
    if (db == "MySql")
        option.UseMySql(builder.Configuration.GetConnectionString("NpphaseManagementDB"), new MySqlServerVersion(new Version(8, 0, 43)));
    else
        option.UseSqlServer(builder.Configuration.GetConnectionString("NpphaseManagementDB"));
});

var jwtSecurityKey = builder.Configuration.GetValue<string>("JwtTokenKeysValue:JWT_SECURRITY_KEY");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey)),
        ClockSkew = TimeSpan.Zero // remove delay of token when expire
    };
});
builder.Services.AddHttpContextAccessor();
RegisterServices.RegisterService(builder.Services);
builder.Services.Configure<StorageOptions>(options =>
{
    var env = builder.Environment; // IWebHostEnvironment available here
    var webRootPath = env.WebRootPath;
    if (string.IsNullOrEmpty(webRootPath))
    {
        // Default to "wwwroot" under ContentRoot if not set
        webRootPath = Path.Combine(env.ContentRootPath, "wwwroot");
        Directory.CreateDirectory(webRootPath);
        env.WebRootPath = webRootPath;
    }
    options.RootPath = Path.Combine(webRootPath, "Data");
});
builder.Services.AddHostedService<SetupIdentityDataSeeder>();
builder.Services.AddCors(op =>
{
    op.AddPolicy("NPOCCorsPolicy", builder => builder
    //.WithOrigins("http://localhost:3000", "http://localhost:3001")
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    //.AllowCredentials()
    );
});

//Log.Logger = new LoggerConfiguration()
//.MinimumLevel.Debug()
////.WriteTo.Console()
//.WriteTo.File(logDirectory + "/RequestResponse-.txt", rollingInterval: RollingInterval.Hour)
//.CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NPPhase API V1");
    });
}
app.UseCors("NPOCCorsPolicy");
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<RequestLoggingMiddleware>(logDirectory);
//app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
