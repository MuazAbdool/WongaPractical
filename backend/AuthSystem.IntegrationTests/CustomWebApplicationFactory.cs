using System.Text;
using AuthSystem.Infrastructure.Data;
using AuthSystem.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestSecret = "test-secret-key-that-is-long-enough-for-hmac-sha256";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Add in-memory database (capture name outside lambda so all scopes share the same DB)
            var dbName = "TestDb_" + Guid.NewGuid();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            // Override JWT settings for testing
            services.Configure<JwtSettings>(opts =>
            {
                opts.Secret = TestSecret;
                opts.Issuer = TestIssuer;
                opts.Audience = TestAudience;
                opts.ExpirationMinutes = 60;
            });

            // Reconfigure JWT Bearer to use test settings
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = TestIssuer,
                    ValidAudience = TestAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecret))
                };
            });
        });

        builder.UseEnvironment("Testing");
    }
}
