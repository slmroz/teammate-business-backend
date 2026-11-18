using ERP.Services.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace ERP.Infrastructure.Auth;
internal static class Extensions
{
    private const string OptionsSectionName = "auth";

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<AuthOptions>(OptionsSectionName);

        services
            .Configure<AuthOptions>(configuration.GetRequiredSection(OptionsSectionName))
            .AddSingleton<IAuthenticator, Authenticator>()
            .AddSingleton<ITokenStorage, HttpContextTokenStorage>()
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Audience = options.Audience;
                o.IncludeErrorDetails = true;
                o.MapInboundClaims = false;
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var details = context.Exception.Message;
                        if (!string.IsNullOrEmpty(options.SigningKey))
                        {
                            details = details.Replace(options.SigningKey, "[redacted]", StringComparison.Ordinal);
                        }
                        details = Regex.Replace(
                            details,
                            @"(?<![A-Za-z0-9_-])[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}(?![A-Za-z0-9_-])",
                            "[redacted JWT]");
                        details = details.Replace('\r', ' ').Replace('\n', ' ').Trim();
                        if (details.Length > 500)
                        {
                            details = details.Substring(0, 500);
                        }
                        Console.Error.WriteLine(
                            $"JWT validation failed at {DateTimeOffset.UtcNow:O}. FailureType={context.Exception.GetType().Name}. Details={details}");
                        return Task.CompletedTask;
                    }
                };
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization(authorization =>
        {
            authorization.AddPolicy("is-admin", policy =>
            {
                policy.RequireRole("Admin");
            });
        });

        return services;
    }
}