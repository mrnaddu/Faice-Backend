using Faice_Backend.Data;
using Faice_Backend.DataSeeder;
using Faice_Backend.Interfaces;
using Faice_Backend.Models;
using Faice_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace Faice_Backend;
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDatabase(services);
        ConfigureIdentity(services);
        ConfigureCors(services);
        ConfigureAuthentication(services);
        ConfigureSwagger(services);
        ConfigureControllers(services);
        ConfigureApplicationServices(services);
    }

    private void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<FaiceDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<FaiceDbContext>();
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<FaiceDbContext>()
            .AddDefaultTokenProviders();
    }

    private void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(Configuration["App:CorsOrigins"])
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])),
                ValidAudience = Configuration["JWT:ValidAudience"],
                ValidIssuer = Configuration["JWT:ValidIssuer"],
            };
        });
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Faice-Backend API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void ConfigureApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IEmailAppService, EmailAppService>();
    }

    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Faice-Backend v1");
            });
        }

        await SeedData.Initialize(app.ApplicationServices);

        app.UseHttpsRedirection();

        app.UseStatusCodePages();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
